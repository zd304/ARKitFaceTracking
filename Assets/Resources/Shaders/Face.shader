Shader "ZDStudio/FaceShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags{ "Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout" }

		Cull Off

		CGPROGRAM
		#pragma surface surf Standard vertex:vert fullforwardshadows
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			 float2 uv_MainTex;
			 float y : TEXCOORD0;
		};

		void vert(inout appdata_full v,out Input o)
		{
			o.uv_MainTex = v.texcoord.xy;
			o.y = v.vertex.y;
		}

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Alpha = c.w;
			clip(o.Alpha - 0.1);
			o.Albedo = c.rgb;
		}
		ENDCG
	}
	FallBack "Diffuse"
}