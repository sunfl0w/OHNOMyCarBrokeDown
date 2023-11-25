Shader "Custom/PSXS_Shader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Diffuse_Strength ("Diffuse Strength", Float) = 1.0
        _Specular_Strength ("Specular Strength", Float) = 1.0
    }
    SubShader
    {
        Pass
        {
            Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalRenderPipeline" }
            LOD 100
            Cull Off // Very important for some textures used
            HLSLPROGRAM
            
            #pragma target 5.0

            #pragma fragment fragment_shader
            #ifdef PSX_ENABLE_TESSELLATION
            #pragma vertex pre_tess_vertex_shader
            #pragma hull hull_shader
            #pragma domain domain_shader
            #else
            #pragma vertex vertex_shader
            #endif

            #include "PSXS_Common.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl" 
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct attributes {
                float4 pos : POSITION;
                float3 norm : NORMAL;
                float2 uv : TEXCOORD0;
                float4 color : COLOR0;
            };

            struct control_point {
                float4 pos : INTERNALTESPOS;
                float3 norm : NORMAL;
                float2 uv : TEXCOORD0;
                float4 color : COLOR0;
            };

            struct varyings {
                float4 pos : SV_POSITION;
                float3 norm : NORMAL;
                float2 uv : TEXCOORD0;
                float4 color : COLOR0;
                float3 tan : TANGENT;
            };

            struct hs_tess_factors {
       			float tess_factors[4] : SV_TessFactor;
        		float inside_tess_factors[2] : SV_InsideTessFactor;
    		};

            // Lighting model is based on Gouraud Shading (per vertex lighting)
            // The Shader is limited to a maximum of four active light sources in a scene

            sampler2D _MainTex;
            float _Diffuse_Strength;
            float _Specular_Strength;
            uniform half unity_FogDensity;

            control_point pre_tess_vertex_shader(attributes v) {
                control_point o;
                o.pos = v.pos;
                o.norm = v.norm;
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            varyings vertex_shader (attributes v) {
                // Emulate PSX style vertex jitter
                float4 clip_pos = PSXS_posToClipSpaceJitter(v.pos);

                // Ambient Lighting
                float4 ambient = half4(unity_SHAr.w, unity_SHAg.w, unity_SHAb.w, 0);

                // Emulate PSX uv mapping. Value is later used in fragment shader texture lookup
                float w = PSXS_getUVMod(v.pos);

                // Pass position in clip space and uv coords to fragment shader
                varyings o;
                o.pos = clip_pos;
                o.uv = v.uv * w;
                // Use up to eight lights for now
                o.color = float4(PSXS_shadeVertexLightsFull(v.pos, v.norm, _WorldSpaceCameraPos, _Diffuse_Strength, _Specular_Strength), 1.0);
                o.color += ambient * v.color;
                float4 world_pos = mul(unity_ObjectToWorld, v.pos);
                float fog_factor = PSXS_getPerVertexFogLerpFactor(distance(world_pos, _WorldSpaceCameraPos), unity_FogDensity);
                o.tan.x = w; // Pass w into tan.x as there is no other way to get this float into the fragment shader stage
                o.tan.y = fog_factor;
                return o;
            }

            float getTessLevel(float dist0, float dist1) {
                float avgDist = (dist0 + dist1) / 2.0;

                if (avgDist <= 4.0) {
                    return 4.0;
                } else if (avgDist <= 8.0) {
                    return 2.0;
                } else {
                    return 1.0;
                }
            }

            hs_tess_factors patch_constants(InputPatch<control_point, 4> i) {
                float dst0 = distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld, i[0].pos));
                float dst1 = distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld, i[1].pos));
                float dst2 = distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld, i[2].pos));
                float dst3 = distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld, i[3].pos));

                float edge0 = getTessLevel(dst3, dst0);
                float edge1 = getTessLevel(dst0, dst1);
                float edge2 = getTessLevel(dst1, dst2);
                float edge3 = getTessLevel(dst2, dst3);
                float avg = (edge0 + edge1 + edge2 + edge3) / 4.0;

        		hs_tess_factors o;
        		o.tess_factors[0] = edge0;
                o.tess_factors[1] = edge1;
                o.tess_factors[2] = edge2;
                o.tess_factors[3] = edge3;
        		o.inside_tess_factors[0] = avg;
                o.inside_tess_factors[1] = avg;
        		return o;
    		}

            [domain("quad")]
    		[partitioning("fractional_odd")]
    		[outputtopology("triangle_cw")]
    		[patchconstantfunc("patch_constants")]
    		[outputcontrolpoints(4)]
    		control_point hull_shader(InputPatch<control_point, 4> input_patch, uint id : SV_OutputControlPointID) {
        		return input_patch[id];
    		}
    
    		[domain("quad")]
    		varyings domain_shader(hs_tess_factors tess_factor_data, const OutputPatch<control_point, 4> patch, float2 barycentricCoordinates : SV_DomainLocation) {
        		#define DomainPos(fieldName) v.fieldName = \
                lerp(lerp(patch[0].fieldName, patch[1].fieldName, barycentricCoordinates.x), lerp(patch[3].fieldName, patch[2].fieldName, barycentricCoordinates.x), barycentricCoordinates.y);
                
                attributes v;

                DomainPos(pos)
                DomainPos(uv)
                DomainPos(color)
                DomainPos(norm)

        		return vertex_shader(v);
    		}

            float4 fragment_shader (varyings i) : SV_Target {
                float4 pre_fog_color = tex2D(_MainTex, i.uv / i.tan.x) * i.color;
                clip(pre_fog_color.a - 0.5f); // Cutoff alpha for binary transparency
                return lerp(unity_FogColor, pre_fog_color, i.tan.y);
            }
            ENDHLSL
        }
    }
}
