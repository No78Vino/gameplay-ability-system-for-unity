Shader "Hidden/ProCamera2D/Letterbox"
{
	Properties
	{
			_MainTex("Base (RGB)", 2D) = "white" {}
			_Top("Top Bar", Range(0.0,1.0)) = 1.0
			_Bottom("Bottom Bar", Range(0.0,1.0)) = 1.0
			_Color("Base(RGB)", Color) = (1,1,1,1)
	}

	SubShader
	{
		ZTest Always Cull Off ZWrite Off Fog{ Mode Off }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc" 

			uniform sampler2D _MainTex;
			uniform float4 _Color;
			uniform float _Top;
			uniform float _Bottom;

			fixed4 frag(v2f_img i) : COLOR
			{
				fixed4 screen = tex2D(_MainTex, i.uv);
				if (i.uv.y < _Bottom || i.uv.y > _Top)
				{
					screen.xyz = _Color;
				}

				return screen;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}