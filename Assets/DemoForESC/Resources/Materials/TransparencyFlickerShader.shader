Shader "Custom/TransparencyFlickerShader" {
    Properties {
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _FlickerSpeed ("Flicker Speed", Range(0.1, 10)) = 1
        _FlickerAmount ("Flicker Amount", Range(0, 1)) = 0.5
    }
    SubShader {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct appdata {
                float4 vertex : POSITION;
            };

            struct v2f {
                float4 pos : SV_POSITION;
            };

            uniform float4 _BaseColor;
            uniform float _FlickerSpeed;
            uniform float _FlickerAmount;

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float flicker = sin(_Time.y * _FlickerSpeed) * _FlickerAmount;
                fixed4 finalColor = _BaseColor;
                finalColor.a += flicker;
                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}