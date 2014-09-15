Shader "Custom/ProgressSmallRing" {
	Properties {
		_BaseColor ("Base Color", Color) = (0, 0, 0, 1)
		_NeonColor ("Neon Color", Color) = (0.5, 0.5, 1, 1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Percent ("Percent", Range(0.1, 5)) = 0.5
		_Emissive ("Emissive", Range(0.1, 50)) = 1
	}
	SubShader {
		Tags { "RenderType"="Opaque" 
			"Queue" = "Transparent+1"		
		}
		//Tags { "RenderType"="Transparent" }
		LOD 200

		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#pragma surface surf Lambert

		half4 _BaseColor;
		half4 _NeonColor;
		sampler2D _MainTex;
		fixed _Percent;
		fixed _Emissive;
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
			half4 overlay = (
					c.r * _NeonColor.r,
					c.g * _NeonColor.g,
					c.b * _NeonColor.b);

			//o.Albedo = overlay.rgb * c.a * _Emissive;
			o.Albedo = _NeonColor.rgb * c.a * _Emissive;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
