Shader "Custom/DisplayNormals" {
	Properties {
		_Strength ("Strength", Float) = 1.0
		_Period ("Period", Float) = 1.0
		_CurT ("Current Time", Float) = 0.0
		_Center ("Center", Vector) = (0,0,0,0)
		_BaseColor ("BaseColor", Color) = (1, 1, 1, 1)
	}

	SubShader {
		Pass {
			CGPROGRAM
// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct v2f members outPos)
#pragma exclude_renderers d3d11 xbox360
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			float _Strength;
			float _Period;
			float _CurT;
			float4 _Center;
			fixed4 _BaseColor;

			struct v2f {
				float4 pos : SV_POSITION;
				float3 color : COLOR0;
				float4 outPos;
			};

			v2f vert(appdata_base v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.normal * _Strength + 0.5;
				o.outPos = o.pos;

				return o;
			}

			half4 frag(v2f i) : COLOR
			{
				float d = distance(i.outPos, _Center);

			//	return _BaseColor * _Strength * exp(-d) * (1 - sin(_CurT + d));
				//return _BaseColor * _Strength * exp(-d) * (1 - sin(_CurT * d/(1+_Strength)));
				return _BaseColor * _Strength * exp(-d) * (1 - sin(_CurT * _Strength /(1+d)));
				//return _BaseColor * _Strength * exp(-d) * (1 - sin(d * _CurT * 3.14/_Period));
			}

			ENDCG
		}
	}
}
