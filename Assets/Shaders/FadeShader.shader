Shader "Unlit/FadeShader"
{
	Properties
	{
		_MainTex("MainTexture", 2D) = "white" {}
		_Rate("Rate", Range(0.0, 1.0)) = 0.5
		_RingRadius("RingRadius", Range(0.0, 1.0)) = 0.1
		_RingColor("RingColor", Color) = (1,1,1,1)
		_Color("Color", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag alpha:fade
			
			#include "UnityCG.cginc"

			struct VertexOut
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			sampler2D _MainTex;
			float _Rate;
			float _RingRadius;
			fixed4 _RingColor;
			fixed4 _Color;

			
			VertexOut vert(appdata_base input)
			{
				VertexOut output;
				output.pos = UnityObjectToClipPos(input.vertex);
				output.uv = input.texcoord.xy;
				return output;
			}
			
			fixed4 frag(VertexOut input) : SV_Target
			{
				fixed4 result = tex2D(_MainTex, input.uv) * _Color;

				///////////////////////////////////////////////////////////////////////////////////
				////// Ultra Ring Effect
				fixed distanceToCenter = (distance(0.5f, input.uv.x) + distance(0.5f, input.uv.y)) * 0.8f;

				// ring alpha
				fixed ringAlpha = saturate((distanceToCenter - _Rate + _RingRadius) / _RingRadius);

				// check if in range
				fixed isInRange = step(distanceToCenter, _Rate);

				result = fixed4(_RingColor.rgb * ringAlpha + result.rgb * (1.0f - ringAlpha), 0.0f);
				result.a = isInRange;
				///////////////////////////////////////////////////////////////////////////////////

				return result;
			}
			ENDCG
		}
	}
}
