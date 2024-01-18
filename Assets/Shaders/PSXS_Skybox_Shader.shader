Shader "Custom/PSXS_Skybox_Shader" {
    Properties {
        _Tint ("Tint Color", Color) = (.5, .5, .5, .5)
        [Gamma] _Exposure ("Exposure", Range(0, 8)) = 1.0
        _Fog_YCoefficient ("Fog YCoefficient", Range(0, 1)) = 1.0
        [NoScaleOffset] _FrontTex ("Front [+Z]", 2D) = "grey" {}
        [NoScaleOffset] _BackTex ("Back [-Z]", 2D) = "grey" {}
        [NoScaleOffset] _LeftTex ("Left [+X]", 2D) = "grey" {}
        [NoScaleOffset] _RightTex ("Right [-X]", 2D) = "grey" {}
        [NoScaleOffset] _UpTex ("Up [+Y]", 2D) = "grey" {}
        [NoScaleOffset] _DownTex ("Down [-Y]", 2D) = "grey" {}
    }

    SubShader {
        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
        Cull Off ZWrite Off

        HLSLINCLUDE

        #include "PSXS_Common.hlsl"

        half4 _Tint;
        half _Exposure;
        float _Fog_YCoefficient;
        uniform half unity_FogDensity;

        struct appdata_t {
            float4 vertex : POSITION;
            float2 texcoord : TEXCOORD0;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };
        struct v2f {
            float4 vertex : SV_POSITION;
            float2 texcoord : TEXCOORD0;
            float3 tan : TANGENT;
            UNITY_VERTEX_OUTPUT_STEREO
        };

        v2f vert(appdata_t v) {
            v2f o;
            UNITY_SETUP_INSTANCE_ID(v);
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
            o.vertex = TransformObjectToHClip(v.vertex);
            o.texcoord = v.texcoord;
            o.tan = v.vertex;
            return o;
        }
        half4 skybox_frag(v2f i, sampler2D smp, half4 smp_decode) {
            half3 c = tex2D(smp, i.texcoord).rgb;
            c = c * _Tint.rgb;
            c *= _Exposure;
            float fog_factor = PSXS_getPerVertexFogLerpFactor(1000.0f, unity_FogDensity); // Pretend that the skybox always is 1000 units away from the camera
            c = lerp(unity_FogColor, c, fog_factor + max(i.tan.y * 6.0, 0) * _Fog_YCoefficient);
            return float4(c, 1);
        }
        ENDHLSL

        Pass {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            sampler2D _FrontTex;
            half4 _FrontTex_HDR;
            half4 frag (v2f i) : SV_Target { return skybox_frag(i,_FrontTex, _FrontTex_HDR); }
            ENDHLSL
        }
        Pass {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            sampler2D _BackTex;
            half4 _BackTex_HDR;
            half4 frag (v2f i) : SV_Target { return skybox_frag(i,_BackTex, _BackTex_HDR); }
            ENDHLSL
        }
        Pass {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            sampler2D _LeftTex;
            half4 _LeftTex_HDR;
            half4 frag (v2f i) : SV_Target { return skybox_frag(i,_LeftTex, _LeftTex_HDR); }
            ENDHLSL
        }
        Pass {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            sampler2D _RightTex;
            half4 _RightTex_HDR;
            half4 frag (v2f i) : SV_Target { return skybox_frag(i,_RightTex, _RightTex_HDR); }
            ENDHLSL
        }
        Pass {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            sampler2D _UpTex;
            half4 _UpTex_HDR;
            half4 frag (v2f i) : SV_Target { return skybox_frag(i,_UpTex, _UpTex_HDR); }
            ENDHLSL
        }
        Pass {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            sampler2D _DownTex;
            half4 _DownTex_HDR;
            half4 frag (v2f i) : SV_Target { return skybox_frag(i,_DownTex, _DownTex_HDR); }
            ENDHLSL
        }
    }
}