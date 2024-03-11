// Adapted from a tutorial by https://twitter.com/DanielJMoran

Shader "Hidden/ProCamera2D/TransitionsFX/Texture" 
{
    Properties 
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Step ("Step", Range(0, 1)) = 0
        _BackgroundColor ("Background Color", Color) = (0, 0, 0, 1)
        _TransitionTex("Transition Texture", 2D) = "white" {}
        _Smoothing ("Smoothing", Range(0, 1)) = .1
    }
    SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

            sampler2D _MainTex;
            float _Step;
            float4 _BackgroundColor;
            sampler2D _TransitionTex;
            float _Smoothing;

            fixed4 frag (v2f i) : SV_Target
            {
            	if(_Step == 1)
            		return _BackgroundColor;
            	
                fixed4 transitTex = tex2D(_TransitionTex, i.uv);

                fixed4 colour = tex2D(_MainTex, i.uv);

				if (_Step >= transitTex.r)
				{
					float alpha = 1;

					if(_Step > 1 - _Smoothing)
						alpha = (_Step - transitTex.r) / (1 - _Step);
					else
						alpha = (_Step - transitTex.r) / _Smoothing;

					alpha = clamp(alpha, 0, 1);
					return lerp(colour, _BackgroundColor, alpha);
				}

				return colour;
            }

            ENDCG
        }
    }
}