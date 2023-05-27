// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/UI/YUVRender"
{
	Properties
	{
		_MainTex("Y", 2D) = "white" {}
		_MainTexU("U", 2D) = "white" {}
		_MainTexV("V", 2D) = "white" {}
	}
		SubShader
	{
		// No culling or depth
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			ZWrite off
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
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
			sampler2D _MainTex;
			sampler2D _MainTexU;
			sampler2D _MainTexV;
			float4 _ColorTin;
			float4 _MainTex_ST;
			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				//UNITY_TRANSFER_FOG(o, o.vertex);
				return o;
			}
			fixed4 frag(v2f i) : SV_Target
			{
				fixed2 uv = fixed2(i.uv.x,1 - i.uv.y);
			    float y = tex2D(_MainTex, uv).a;
			    float u = tex2D(_MainTexU, uv).a;
			    float v = tex2D(_MainTexV, uv).a;
				float r = y - 1.6  *(v - 128. / 255.);
				float g = y - 1.6 *(u - 128. / 255.) - 1.6 *(v - 128. / 255.);
				float b = y - 1.6  *(u - 128. / 255.);
				float4 col = float4(r, g, b, 1.f);
				return col;
			}
			ENDCG
		}
	}
}
