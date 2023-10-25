Shader "Custom/PS1_Spotlight_Shader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Light_Color ("Light Color", Color) = (1, 1, 1, 1)
        _Fog_Color ("Fog Color", Color) = (1, 1, 1, 1)
        _Fog_Coefficient ("Fog Coefficient", Range(0, 1)) = 0.01
        _Vertex_Jitter_Coefficient ("Vertex Jitter Coefficient", Float) = 85.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {   
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR0;
                float4 tan : TANGENT;
            };

            sampler2D _MainTex;
            float4 _Light_Color;
            float4 _Fog_Color;
            float _Fog_Coefficient;
            float _Vertex_Jitter_Coefficient;

            v2f vert (appdata v) {
                // Emulate PS1 style vertex jitter
                float4 clip_pos = UnityObjectToClipPos(v.vertex);
                clip_pos.xyz = clip_pos.xyz / clip_pos.w;
                clip_pos.xy = floor(clip_pos.xy * _Vertex_Jitter_Coefficient + 0.5) / _Vertex_Jitter_Coefficient;
                clip_pos.xyz *= clip_pos.w;

                // Experimental emulation of PS1 uv mapping
                float4 view_pos = mul(UNITY_MATRIX_MV, v.vertex);
                float w = min(view_pos.z * 0.1, -0.1);

                v2f o;
                o.vertex = clip_pos;
                o.uv = v.uv * w;
                float4 world_pos = mul(unity_ObjectToWorld, v.vertex);
                float fog_factor = exp(-pow(_Fog_Coefficient * 0.5f * 0.01f * distance(_WorldSpaceCameraPos, world_pos), 2.0));
                o.tan.x = w;
                o.tan.y = fog_factor;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 pre_fog_color = tex2D(_MainTex, i.uv / i.tan.x) * _Light_Color;
                return lerp(_Fog_Color, pre_fog_color, i.tan.y);
            }
            ENDCG
        }
    }
}
