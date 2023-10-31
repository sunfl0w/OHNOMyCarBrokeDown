#include "UnityCG.cginc"
#include "UnityLightingCommon.cginc"

#define PSX_VERTEX_JITTER_COEFFICIENT 75.0

float PSXS_getPerVertexFogLerpFactor(float view_distance, float fog_density) {
    return exp(-pow(fog_density * 0.2f * view_distance, 2.0));
}

float4 PSXS_posToClipSpaceJitter(float4 vertex_pos_os) {
    float4 clip_pos = UnityObjectToClipPos(vertex_pos_os);
    clip_pos.xyz = clip_pos.xyz / clip_pos.w;
    clip_pos.xy = floor(clip_pos.xy * PSX_VERTEX_JITTER_COEFFICIENT + 0.5) / PSX_VERTEX_JITTER_COEFFICIENT;
    clip_pos.xyz *= clip_pos.w;
    return clip_pos;
}

float PSXS_getUVMod(float4 vertex_pos_os) {
    float4 view_pos = mul(UNITY_MATRIX_MV, vertex_pos_os);
    return min(view_pos.z * 1.0, -0.1);
}

float PSXS_vertexDiffuseLighting(float3 vertex_norm_os, float3 light_dir, float diffuse_modifier) {
    float3 vertex_norm_ws = UnityObjectToWorldNormal(vertex_norm_os);
    float angle = max(dot(vertex_norm_ws, light_dir), 0.0);
    return angle * diffuse_modifier;
}

float PSXS_vertexSpecularLighting(float4 vertex_pos_os, float3 vertex_norm_os, float3 camera_pos_ws, float3 light_dir, float specular_modifier) {
    float4 vertex_pos_ws = mul(unity_ObjectToWorld, vertex_pos_os);
    float3 view_dir = normalize(camera_pos_ws - vertex_pos_ws.xyz);
    float3 vertex_norm_ws = UnityObjectToWorldNormal(vertex_norm_os);
    float3 reflect_dir = reflect(-light_dir, vertex_norm_ws);
    float spec = pow(max(dot(view_dir, reflect_dir), 0.0), 16);
    return spec * specular_modifier;
}

float3 PSXS_shadeVertexLightsFull(float4 vertex_pos_os, float3 vertex_norm_os, float3 camera_pos_ws, float diffuse_modifier, float specular_modifier, int lightCount, bool spotLight) {
    /*float3 viewpos = UnityObjectToViewPos (vertex_pos_os.xyz);
    float3 viewN = normalize (mul ((float3x3)UNITY_MATRIX_IT_MV, vertex_norm_os));

    float3 lightColor = float4(0, 0, 0, 1);
    for (int i = 0; i < lightCount; i++) {
        float3 toLight = unity_LightPosition[i].xyz - viewpos.xyz * unity_LightPosition[i].w;
        float lengthSq = dot(toLight, toLight);

        // don't produce NaNs if some vertex position overlaps with the light
        lengthSq = max(lengthSq, 0.000001);

        toLight *= rsqrt(lengthSq);

        float atten = 1.0 / (1.0 + lengthSq * unity_LightAtten[i].z);
        if (spotLight)
        {
            float rho = max (0, dot(toLight, unity_SpotDirection[i].xyz));
            float spotAtt = (rho - unity_LightAtten[i].x) * unity_LightAtten[i].y;
            atten *= saturate(spotAtt);
        }

        float diff = max (0, dot (viewN, toLight));
        lightColor += unity_LightColor[i].rgb * (diff * atten);
    }
    return lightColor;*/
    float4 light_color = float4(0, 0, 0, 1);
    for (int i = 0; i < lightCount; i++) {
        float3 vertex_pos_vs = UnityObjectToViewPos(vertex_pos_os);

        // Copy of Unity's shading code in view space to calculate attenuation
        float3 to_light_vs = 1.0 * (unity_LightPosition[i].xyz - vertex_pos_vs.xyz * unity_LightPosition[i].w);
        float lengthSq = dot(to_light_vs, to_light_vs);
        // don't produce NaNs if some vertex position overlaps with the light
        lengthSq = max(lengthSq, 0.000001);
        to_light_vs *= rsqrt(lengthSq);
        float atten = 1.0 / (1.0 + lengthSq * unity_LightAtten[i].z);
        if (spotLight) {
            float rho = max (0, dot(to_light_vs, unity_SpotDirection[i].xyz));
            float spotAtt = (rho - unity_LightAtten[i].x) * unity_LightAtten[i].y;
            atten *= saturate(spotAtt);
        }

        // Calculate diffuse and specular lighting for each light source in world space and apply attenuation
        float3 light_pos_ws = mul(UNITY_MATRIX_I_V, float4(unity_LightPosition[i].xyz, 1));
        float4 vertex_pos_ws = mul(unity_ObjectToWorld, vertex_pos_os);
        float3 light_dir_ws = normalize(light_pos_ws - vertex_pos_ws);
        if (unity_LightPosition[i].w < 0.1f) { // Check if light is directional and set direction accordingly
            //light_dir_ws = normalize(light_pos_ws);
            light_dir_ws = normalize(_WorldSpaceLightPos0);
            atten = 1.0f;
        }
        float diffuse = PSXS_vertexDiffuseLighting(vertex_norm_os, light_dir_ws, diffuse_modifier);
        float specular = PSXS_vertexSpecularLighting(vertex_pos_os, vertex_norm_os, camera_pos_ws, light_dir_ws, specular_modifier);
        light_color += (diffuse + specular) * atten * float4(unity_LightColor[i].rgb, 1);
    }
    return light_color;
}
