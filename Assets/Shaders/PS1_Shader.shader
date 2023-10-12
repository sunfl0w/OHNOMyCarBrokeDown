Shader "Custom/PS1_Shader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Alpha_Cutoff  ("Alpha Cutoff", Float) = 0.5
        _Diffuse_Strength ("Diffuse Strength", Float) = 1.0
        _Specular_Strength ("Specular Strength", Float) = 1.0
        _Ambient_Light_Color ("Ambient Light Color", Color) = (1, 1, 1, 1)
        _Ambient_Light_Strength ("Ambient Light Strength", Float) = 1.0
        _Static_Light_0_Direction ("Static Light 0 Direction", Vector) = (0, 0, 0, 0)
        _Static_Light_0_Color ("Static Light 0 Color", Color) = (1, 1, 1, 1)
        _Static_Light_0_Strength ("Static Light 0 Strength", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Cull Off // Very important for some textures used
            CGPROGRAM
            #pragma vertex vertex_shader
            #pragma fragment fragment_shader

            #include "UnityCG.cginc"

            struct appdata {
                float4 pos : POSITION;
                float3 norm : NORMAL;
                float4 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
                fixed4 color : COLOR;
            };

            // Lighting model is based on Gouraud Shading (per vertex lighting)
            // The Shader is limited to an ambient light source and a single static light source for now

            sampler2D _MainTex;
            float _Alpha_Cutoff;
            float _Diffuse_Strength;
            float _Specular_Strength;
            float4 _Ambient_Light_Color;
            float _Ambient_Light_Strength;
            float4 _Static_Light_0_Direction;
            float4 _Static_Light_0_Color;
            float _Static_Light_0_Strength;

            v2f vertex_shader (appdata v) {
                // Transform vertex position to clip space and pass texture coords to fragment shader
                v2f o;
                o.pos = UnityObjectToClipPos(v.pos);
                o.uv = v.uv;

                // Transform vertex position to world space
                float3 world_pos = mul(unity_ObjectToWorld, v.pos);

                // Ambient Lighting
                float4 ambient = _Ambient_Light_Strength * _Ambient_Light_Color;

                // Diffuse Lighting
                float3 norm = UnityObjectToWorldNormal(v.norm);
                float3 light_dir = -normalize(_Static_Light_0_Direction.xyz);//normalize(_Static_Light_0_Position.xyz - world_pos);
                float angle = max(dot(norm, light_dir), 0.0);
                float4 diffuse = angle * _Diffuse_Strength * _Static_Light_0_Color * _Static_Light_0_Strength;

                // Specular Lighting
                float3 view_dir = normalize(_WorldSpaceCameraPos - world_pos);
                float3 reflect_dir = reflect(-light_dir, norm);
                float spec_coefficient = pow(max(dot(view_dir, reflect_dir), 0.0), 32);
                float4 specular = spec_coefficient * _Specular_Strength * _Static_Light_0_Color * _Static_Light_0_Strength;

                // Pass final color to fragment shader
                o.color = (ambient + diffuse + specular) * v.color;
                return o;
            }

            fixed4 fragment_shader (v2f i) : SV_Target {
                fixed4 color = tex2D(_MainTex, i.uv) * i.color;
                clip(color.a - _Alpha_Cutoff); // Cutoff alpha for binary transparency
                return color;
            }
            ENDCG
        }
    }
}
