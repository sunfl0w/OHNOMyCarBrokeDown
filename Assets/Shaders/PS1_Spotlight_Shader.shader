Shader "Custom/PS1_Spotlight_Shader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Light_Color ("Light Color", Color) = (1, 1, 1, 1)
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
            #include "PSXS_Common.cginc"

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
            uniform half unity_FogDensity;

            v2f vert (appdata v) {
                // Emulate PSX style vertex jitter
                float4 clip_pos = PSXS_posToClipSpaceJitter(v.vertex);

                // Emulate PSX uv mapping. Value is later used in fragment shader texture lookup
                float w = PSXS_getUVMod(v.vertex);

                v2f o;
                o.vertex = clip_pos;
                o.uv = v.uv * w;
                float4 world_pos = mul(unity_ObjectToWorld, v.vertex);
                float fog_factor = PSXS_getPerVertexFogLerpFactor(distance(_WorldSpaceCameraPos, world_pos) * 0.7, unity_FogDensity);
                o.tan.x = w;
                o.tan.y = fog_factor;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 pre_fog_color = tex2D(_MainTex, i.uv / i.tan.x) * _Light_Color;
                return lerp(unity_FogColor, pre_fog_color, i.tan.y);
            }
            ENDCG
        }
    }
}
