Shader "Custom/ProgressOuterRing" {
	Properties {
		_BaseColor ("Base Color", Color) = (0, 0, 0, 1)
		_NeonColor ("Neon Color", Color) = (0.5, 0.5, 1, 1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Percent ("Percent", Range(0.1, 5)) = 0.5
		_Emissive ("Emissive", Range(0.1, 50)) = 1
		_WhiteRatio ("White Ratio", Range(0.01, 0.5)) = 0.1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		//Tags { "RenderType"="Transparent" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		half4 _BaseColor;
		half4 _NeonColor;
		sampler2D _MainTex;
		fixed _Percent;
		fixed _Emissive;
		fixed _WhiteRatio;
		float2 realUV;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			realUV = IN.uv_MainTex;
			realUV -= float2(0.5, 0.5);
			realUV *= _Percent;
			realUV += float2(0.5, 0.5);

			half4 c = tex2D (_MainTex, realUV);
/*
			half4 overlay = (
					c.r * _NeonColor.r,
					c.g * _NeonColor.g,
					c.b * _NeonColor.b);
*/

			//o.Albedo = (overlay.rgb * c.a + _BaseColor.rgb * _BaseColor.a) * _Emissive * (1 + c.a);
			o.Albedo = ((c.rgb * _WhiteRatio + _NeonColor.rgb) * c.a + _BaseColor.rgb * _BaseColor.a) * _Emissive * (1 + c.a);
			//o.Albedo = ( _NeonColor.rgb * c.a + _BaseColor.rgb * _BaseColor.a) * _Emissive * (1 + c.a);
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
