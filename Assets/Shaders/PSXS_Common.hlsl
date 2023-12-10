#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl" 
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

#define PSX_VERTEX_JITTER_COEFFICIENT 120.0
#define PSX_ENABLE_AFFINE_TEXTURE_MAPPING // Only disable when PSX_ENABLE_TESSELLATION is set for best effect
//#define PSX_ENABLE_TESSELLATION // Enbales tessellation shader stage

float PSXS_getPerVertexFogLerpFactor(float view_distance, float fog_density) {
    return exp(-pow(fog_density * 0.2f * view_distance, 2.0));
}

float4 PSXS_posToClipSpaceJitter(float4 vertex_pos_os) {
    float3 vertex_pos_ws = TransformObjectToWorld(vertex_pos_os);
    // Round vertex position in world space to avoid most seams between adjacent edges.
    // This will not eliminate all seams but most of them
    vertex_pos_ws.x = round(vertex_pos_ws.x * 250.0) / 250.0;
    vertex_pos_ws.y = round(vertex_pos_ws.y * 250.0) / 250.0;
    float4 clip_pos = TransformWorldToHClip(vertex_pos_ws);
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

float3 PSXS_shadeVertexLightsDirectional(float3 vertex_norm_ws, float3 view_dir, float diffuse_modifier, float specular_modifier) {
    float4 light_color = float4(0, 0, 0, 1);
    Light light = GetMainLight();
    light_color.rgb += LightingLambert(light.color * light.distanceAttenuation * 1.0, light.direction, vertex_norm_ws) * diffuse_modifier;
    light_color.rgb += LightingSpecular(light.color * light.distanceAttenuation * 1.0, light.direction, vertex_norm_ws, view_dir, half4(light.color, 0), 16.0) * specular_modifier;
    return light_color;
}

float3 PSXS_shadeVertexLightsPoint(float4 vertex_pos_ws, float3 vertex_norm_ws, float3 view_dir, float diffuse_modifier, float specular_modifier) {
    float4 light_color = float4(0, 0, 0, 1);
    uint lightsCount = GetAdditionalLightsCount();
    for (int j = 0; j < lightsCount; j++) {
        Light light = GetAdditionalLight(j, vertex_pos_ws);
        light_color.rgb += LightingLambert(light.color * light.distanceAttenuation * 1.0, light.direction, vertex_norm_ws) * diffuse_modifier;
        light_color.rgb += LightingSpecular(light.color * light.distanceAttenuation * 1.0, light.direction, vertex_norm_ws, view_dir, half4(light.color, 0), 16.0) * specular_modifier;
    }
    return light_color;
}

float3 PSXS_shadeVertexLightsFull(float4 vertex_pos_os, float3 vertex_norm_os, float3 camera_pos_ws, float diffuse_modifier, float specular_modifier) {
    /*float4 vertex_pos_ws = mul(unity_ObjectToWorld, vertex_pos_os);
    float3 vertex_norm_ws = TransformObjectToWorldNormal(vertex_norm_os);
    float3 view_dir = normalize(camera_pos_ws - vertex_pos_ws);

    float4 light_color = float4(0, 0, 0, 1);
    uint lightsCount = GetAdditionalLightsCount();
    for (int j = 0; j < lightsCount; j++) {
        Light light = GetAdditionalLight(j, vertex_pos_ws);
        light_color.rgb += LightingLambert(light.color * light.distanceAttenuation * 1.0, light.direction, vertex_norm_ws) * diffuse_modifier;
        light_color.rgb += LightingSpecular(light.color * light.distanceAttenuation * 1.0, light.direction, vertex_norm_ws, view_dir, half4(light.color, 0), 32.0) * specular_modifier;
    }
    Light light = GetMainLight();
    light_color.rgb += LightingLambert(light.color * light.distanceAttenuation * 1.0, light.direction, vertex_norm_ws) * diffuse_modifier;
    light_color.rgb += LightingSpecular(light.color * light.distanceAttenuation * 1.0, light.direction, vertex_norm_ws, view_dir, half4(light.color, 0), 32.0) * specular_modifier;
    return light_color;*/
    float4 vertex_pos_ws = mul(unity_ObjectToWorld, vertex_pos_os);
    float3 vertex_norm_ws = TransformObjectToWorldNormal(vertex_norm_os);
    float3 view_dir = normalize(camera_pos_ws - vertex_pos_ws);
    float4 light_color = float4(0, 0, 0, 1);
    light_color.rgb += PSXS_shadeVertexLightsDirectional(vertex_norm_ws, view_dir, diffuse_modifier, specular_modifier);
    light_color.rgb += PSXS_shadeVertexLightsPoint(vertex_pos_ws, vertex_norm_ws, view_dir, diffuse_modifier, specular_modifier);
    return light_color.rgb;
}
