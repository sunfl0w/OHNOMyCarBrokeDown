Shader "Custom/PS1_Shader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Alpha_Cutoff  ("Alpha Cutoff", Float) = 0.5
        _Vertex_Jitter_Coefficient ("Vertex Jitter Coefficient", Float) = 85.0
        _Fog_Color ("Fog Color", Color) = (1, 1, 1, 1)
        _Fog_Coefficient ("Fog Coefficient", Range(0, 1)) = 0.01
        _Diffuse_Strength ("Diffuse Strength", Float) = 1.0
        _Specular_Strength ("Specular Strength", Float) = 1.0
        _Ambient_Light_Color ("Ambient Light Color", Color) = (1, 1, 1, 1)
        _Ambient_Light_Strength ("Ambient Light Strength", Float) = 1.0
    }
    SubShader
    {
        Pass
        {
            Tags { "RenderType"="Opaque" "LightMode" = "ForwardBase" }
            LOD 100
            Cull Off // Very important for some textures used
            CGPROGRAM
            
            //#pragma vertex vertex_shader
            #pragma fragment fragment_shader
            #pragma vertex pre_tess_vertex_shader
            #pragma hull hull_shader
            #pragma domain domain_shader

            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"

            struct attributes {
                float4 pos : POSITION;
                float3 norm : NORMAL;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR0;
            };

            struct control_point {
                float4 pos : INTERNALTESPOS;
                float3 norm : NORMAL;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR0;
            };

            struct varyings {
                float4 pos : SV_POSITION;
                float3 norm : NORMAL;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR0;
                float3 tan : TANGENT;
            };

            struct hs_tess_factors {
       			float tess_factors[4] : SV_TessFactor;
        		float inside_tess_factors[2] : SV_InsideTessFactor;
    		};

            // Lighting model is based on Gouraud Shading (per vertex lighting)
            // The Shader is limited to an ambient light source and a single static light source for now

            sampler2D _MainTex;
            float _Alpha_Cutoff;
            float _Vertex_Jitter_Coefficient;
            float4 _Fog_Color;
            float _Fog_Coefficient;
            float _Diffuse_Strength;
            float _Specular_Strength;
            float4 _Ambient_Light_Color;
            float _Ambient_Light_Strength;

            control_point pre_tess_vertex_shader(attributes v) {
                control_point o;
                o.pos = v.pos;
                o.norm = v.norm;
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            varyings vertex_shader (attributes v) {
                // Emulate PS1 style vertex jitter
                float4 clip_pos = UnityObjectToClipPos(v.pos);;
                clip_pos.xyz = clip_pos.xyz / clip_pos.w;
                clip_pos.xy = floor(clip_pos.xy * _Vertex_Jitter_Coefficient + 0.5) / _Vertex_Jitter_Coefficient;
                clip_pos.xyz *= clip_pos.w;

                float4 world_pos = mul(unity_ObjectToWorld, v.pos);
                float4 view_pos = mul(UNITY_MATRIX_MV, v.pos);

                // Ambient Lighting
                float4 ambient = _Ambient_Light_Strength * _Ambient_Light_Color;

                // Diffuse Lighting
                float3 norm = UnityObjectToWorldNormal(v.norm);
                float3 light_dir = _WorldSpaceLightPos0.xyz; // Actually direction of directional light source in this case! //-normalize(_WorldSpaceLightPos0.xyz - world_pos.xyz);
                float angle = max(dot(norm, light_dir), 0.0);
                float4 diffuse = angle * _Diffuse_Strength * _LightColor0;

                // Specular Lighting
                float3 view_dir = normalize(_WorldSpaceCameraPos - world_pos);
                float3 reflect_dir = reflect(-light_dir, norm);
                float spec_coefficient = pow(max(dot(view_dir, reflect_dir), 0.0), 32);
                float4 specular = spec_coefficient * _Specular_Strength * _LightColor0;

                // Experimental emulation of PS1 uv mapping
                float w = min(view_pos.z * 0.1, -0.1);

                // Pass position in clip space and uv coords to fragment shader
                varyings o;
                o.pos = clip_pos;
                o.uv = v.uv * w;
                // Pass final color to fragment shader
                o.color = ambient + diffuse + specular + v.color;
                float fog_factor = exp(-pow(_Fog_Coefficient * 0.01f * distance(_WorldSpaceCameraPos, world_pos), 2.0));
                //lerp(_Fog_Color, o.color, fog_factor);
                o.tan.x = w; // Pass w into tan.x as there is no other way to get this float into the fragment shader stage
                o.tan.y = fog_factor;
                return o;
            }

            float getTessLevel(float dist0, float dist1) {
                float avgDist = (dist0 + dist1) / 2.0;

                if (avgDist <= 2.0) {
                    return 4.0;
                } else if (avgDist <= 6.0) {
                    return 2.0;
                } else {
                    return 1.0;
                }
            }

            hs_tess_factors patch_constants(InputPatch<control_point, 4> i) {
                //float4 patch_center_pos = lerp(lerp(i[0].pos, i[1].pos, 0.5), lerp(i[3].pos, i[2].pos, 0.5), 0.5);
                float dst0 = distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld, i[0].pos));
                float dst1 = distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld, i[1].pos));
                float dst2 = distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld, i[2].pos));
                float dst3 = distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld, i[3].pos));

                float edge0 = getTessLevel(dst3, dst0);
                float edge1 = getTessLevel(dst0, dst1);
                float edge2 = getTessLevel(dst1, dst2);
                float edge3 = getTessLevel(dst2, dst3);
                float avg = (edge0 + edge1 +edge2 + edge3) / 4.0;

        		hs_tess_factors o;
        		o.tess_factors[0] = edge0;
                o.tess_factors[1] = edge1;
                o.tess_factors[2] = edge2;
                o.tess_factors[3] = edge3;
        		o.inside_tess_factors[0] = avg;
                o.inside_tess_factors[1] = avg;
        		return o;
    		}

            [UNITY_domain("quad")]
    		[UNITY_partitioning("integer")]
    		[UNITY_outputtopology("triangle_cw")]
    		[UNITY_patchconstantfunc("patch_constants")]
    		[UNITY_outputcontrolpoints(4)]
    		control_point hull_shader(InputPatch<control_point, 4> input_patch, uint id : SV_OutputControlPointID) {
        		return input_patch[id];
    		}
    
    		[UNITY_domain("quad")]
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

            fixed4 fragment_shader (varyings i) : SV_Target {
                fixed4 pre_fog_color = tex2D(_MainTex, i.uv / i.tan.x) * i.color;
                clip(pre_fog_color.a - _Alpha_Cutoff); // Cutoff alpha for binary transparency
                return lerp(_Fog_Color, pre_fog_color, i.tan.y);
            }
            ENDCG
        }
    }
}
