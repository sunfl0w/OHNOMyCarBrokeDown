#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl" 
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

#define PSX_VERTEX_JITTER_COEFFICIENT 120.0
#define PSX_ENABLE_AFFINE_TEXTURE_MAPPING // Only disable when PSX_ENABLE_TESSELLATION is set for best effect
//#define PSX_ENABLE_TESSELLATION // Enbales tessellation shader stage

float PSXS_getPerVertexFogLerpFactor(float view_distance, float fog_density) {
    return exp(-pow(fog_density * 0.2f * view_distance, 2.0));
}

float4 PSXS_posToClipSpaceJitter(float4 vertex_pos_os) {
    float4 clip_pos = TransformObjectToHClip(vertex_pos_os);
    clip_pos.xyz = clip_pos.xyz / clip_pos.w;
    clip_pos.xy = floor(clip_pos.xy * PSX_VERTEX_JITTER_COEFFICIENT + 0.5) / PSX_VERTEX_JITTER_COEFFICIENT;
    clip_pos.xyz *= clip_pos.w;
    return clip_pos;
}

float PSXS_getUVMod(float4 vertex_pos_os) {
    #ifdef PSX_ENABLE_AFFINE_TEXTURE_MAPPING
    return 1.0f;
    #else
    float4 view_pos = mul(UNITY_MATRIX_MV, vertex_pos_os);
    return min(view_pos.z * 1.0, -0.1);
    #endif
}

float3 PSXS_shadeVertexLightsFull(float4 vertex_pos_os, float3 vertex_norm_os, float3 camera_pos_ws, float diffuse_modifier, float specular_modifier) {
    float4 vertex_pos_ws = mul(unity_ObjectToWorld, vertex_pos_os);
    float3 vertex_norm_ws = TransformObjectToWorldNormal(vertex_norm_os);
    float3 view_dir = normalize(camera_pos_ws - vertex_pos_ws);

    float4 light_color = float4(0, 0, 0, 1);
    uint lightsCount = GetAdditionalLightsCount();
    for (int j = 0; j < lightsCount; j++) {
        Light light = GetAdditionalLight(j, vertex_pos_ws);
        light_color.rgb += LightingLambert(light.color * light.distanceAttenuation * 1.0, light.direction, vertex_norm_ws) * diffuse_modifier;
        light_color.rgb += LightingSpecular(light.color * light.distanceAttenuation * 1.0, light.direction, vertex_norm_ws, view_dir, half4(light.color, 0), 8.0) * specular_modifier;
    }
    Light light = GetMainLight();
    light_color.rgb += LightingLambert(light.color * light.distanceAttenuation * 1.0, light.direction, vertex_norm_ws) * diffuse_modifier;
    light_color.rgb += LightingSpecular(light.color * light.distanceAttenuation * 1.0, light.direction, vertex_norm_ws, view_dir, half4(light.color, 0), 8.0) * specular_modifier;
    return light_color;
}
