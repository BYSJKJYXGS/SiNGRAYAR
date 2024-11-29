Shader "Custom/Grayscale"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}

	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100

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
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			sampler2D _UTex;
			sampler2D _VTex;
			//sampler2D _UVTex;
			float4 _MainTex_ST;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				//如果需要翻转画面，把下面的 进行对应的 1- 即可，例如（翻转左右则 1- i.uv.x）
				fixed2 uv = fixed2(1-i.uv.x, i.uv.y);
				fixed4 ycol = tex2D(_MainTex, uv);

				//如果是使用 Alpha8 的纹理格式写入各分量的值，各分量的值就可以直接取a通道的值
				float r = ycol.a ;
				float g = ycol.a ;
				float b = ycol.a ;
				return fixed4(r,g,b,1);
				}
				ENDCG
			}
	}
		FallBack "diffuse"
}