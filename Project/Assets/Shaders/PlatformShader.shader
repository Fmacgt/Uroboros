Shader "Custom/PlatformShader" {
	Properties {
		_BaseColor ("Base Color", Color) = (0, 0, 0, 1)
		_NeonColor ("Neon Color", Color) = (0.5, 0.5, 1, 1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Emissive ("Emissive", Range(0.5, 8)) = 1
		_WhiteRatio ("White Ratio", Range(0.01, 0.5)) = 0.1
		_Alpha ("Alpha", Range(0, 1)) = 1
	}
	SubShader {
		Tags {
			"RenderType"="Transparent"
			"Queue" = "Transparent+1"
		}

		LOD 200
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#pragma surface surf Lambert

		half4 _BaseColor;
		half4 _NeonColor;
		sampler2D _MainTex;
		fixed _Percent;
		fixed _Emissive;
		fixed _WhiteRatio;
		fixed _Alpha;
		float2 realUV;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);

			o.Albedo = (c.rgb * _WhiteRatio + _NeonColor.rgb * length(c.rgb)) * _Emissive * (1 + c.a) * _Alpha;
			//o.Albedo = c.rgb * _Emissive * (1 + c.a) * _Alpha;
			o.Alpha = c.a * _Alpha;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
