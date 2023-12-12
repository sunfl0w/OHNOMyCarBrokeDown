Shader "Custom/Pixelize" {
    Properties {
        _MainTex ("Texture", 2D) = "white"
    }
    SubShader {
        Tags {
            "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"
        }

        HLSLINCLUDE
        #pragma vertex vert
        #pragma fragment frag

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        struct Attributes {
            float4 positionOS : POSITION;
            float2 uv : TEXCOORD0;
        };

        struct Varyings {
            float4 positionHCS : SV_POSITION;
            float2 uv : TEXCOORD0;
        };

        TEXTURE2D(_MainTex);
        SamplerState sampler_point_clamp;

        Varyings vert(Attributes IN) {
            Varyings OUT;
            OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
            OUT.uv = IN.uv;
            return OUT;
        }

        ENDHLSL

        Pass {
            HLSLPROGRAM
            half4 frag(Varyings IN) : SV_TARGET {
                float4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_point_clamp, IN.uv);
                return tex;
            }
            ENDHLSL
        }
    }
}