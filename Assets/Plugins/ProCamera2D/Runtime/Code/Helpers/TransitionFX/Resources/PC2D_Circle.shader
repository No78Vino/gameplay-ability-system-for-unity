Shader "Hidden/ProCamera2D/TransitionsFX/Circle" 
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Step ("Step", Range(0, 1)) = 0
        _BackgroundColor ("Background Color", Color) = (0, 0, 0, 1)
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

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 colour = _BackgroundColor;
                float aspectRatio = _ScreenParams.y / _ScreenParams.x;
                if (sqrt((float)(pow(i.uv.x - 0.5, 2) + pow((i.uv.y - 0.5) * aspectRatio, 2) < 0.5 - (_Step / 2))))
                    colour = tex2D(_MainTex, i.uv);
                
                return colour;
            }

            ENDCG
        }
    }
}