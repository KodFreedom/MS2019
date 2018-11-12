Shader "Custom/IlluminObjectShader" 
{
	Properties 
	{
		_MainTex ("MainTexture", 2D) = "white" {}
		_IlluminTex("IlluminTexture", 2D) = "white" {}
		//_Glossiness ("Smoothness", Range(0,1)) = 0.5
		//_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _IlluminTex;
		float3 _IlluminAwakerPosition;
		float  _IlluminAwakerRadius;
		float  _RingRadius;

		struct Input 
		{
			float2 uv_MainTex;
			float3 worldPos;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
		// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input input, inout SurfaceOutputStandard output) 
		{
			fixed4 color = tex2D (_MainTex, input.uv_MainTex);
			output.Albedo = color.rgb;
			output.Alpha = 1.0f;

			float distanceToAwaker = distance(_IlluminAwakerPosition, input.worldPos);
			if (distanceToAwaker >= _IlluminAwakerRadius
				&& distanceToAwaker <= _IlluminAwakerRadius + _RingRadius)
			{
				output.Albedo = fixed4(1, 1, 1, 1);
			}

			// Metallic and smoothness come from slider variables
			//output.Metallic = _Metallic;
			//output.Smoothness = _Glossiness;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
