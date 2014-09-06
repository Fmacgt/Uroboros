Shader "Custom/TestNormal" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Pass {
			CGPROGRAM
// Upgrade NOTE: excluded shader from OpenGL ES 2.0 because it does not contain a surface program or both vertex and fragment programs.
#pragma exclude_renderers gles
			
			#pragma vertx vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct v2f {
				float4 pos : SV_POSITION;
				float3 color : COLOR0;
			};

			v2f vert(appdata_base v)
			{
				v2f o;
				return o;
			}

			half4 frag(v2f i) : COLOR
			{
				return (0.5, 1, 1, 0.5);
			}

			ENDCG
		}
	}
	FallBack "Diffuse"
}
