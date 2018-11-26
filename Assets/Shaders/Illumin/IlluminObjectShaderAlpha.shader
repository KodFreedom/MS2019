Shader "Unlit/IlluminObjectShaderAlpha"
{
	Properties
	{
		_MainTexture("MainTexture", 2D) = "white" {}
		_IlluminTexture("IlluminTexture", 2D) = "white" {}
		_Specular("Specular", Color) = (1,1,1,1)
		_Gloss("Gloss", Range(8.0,256)) = 20
		_RingRadius("RingRadius", Range(0.0, 5.0)) = 0.2
		_RingColor("RingColor", Color) = (1,1,1,1)
	}

	SubShader
	{
		Pass
		{
			Tags {"LightMode" = "ForwardBase" "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off
			CGPROGRAM
			#pragma vertex vert  
			#pragma fragment frag  
			#pragma multi_compile_fwdbase

			#include "Lighting.cginc"  
			#include "AutoLight.cginc"

			sampler2D _MainTexture;
			sampler2D _IlluminTexture;
			fixed4	  _Specular;
			float	  _Gloss;
			float3	  _IlluminAwakerPosition;
			float	  _IlluminAwakerRadius;
			float	  _RingRadius;
			fixed4	  _RingColor;
			float     _IlluminStrength;

			struct VertexOut
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 worldNormal : TEXCOORD3;
				float3 worldPos : TEXCOORD4;
				SHADOW_COORDS(2)
			};

			VertexOut vert(appdata_base input)
			{
				VertexOut output;
				output.pos = UnityObjectToClipPos(input.vertex);
				output.uv = input.texcoord.xy;
				output.worldNormal = mul(input.normal, (float3x3)unity_WorldToObject);
				output.worldPos = mul(unity_ObjectToWorld, input.vertex).xyz;
				TRANSFER_SHADOW(output);
				return output;
			}

			fixed4 frag(VertexOut input) : SV_Target
			{
				///////////////////////////////////////////////////////////////////////////////////
				////// Light and shadow
				fixed4 color = tex2D(_MainTexture, input.uv);
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;

				fixed3 worldNormal = normalize(input.worldNormal);
				fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
				fixed3 diffuse = _LightColor0.rgb * (saturate(dot(worldNormal, worldLightDir)));

				fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - input.worldPos.xyz);
				fixed3 halfDir = normalize(viewDir + worldLightDir);
				fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(saturate(dot(worldNormal,halfDir)),_Gloss);

				// compute shadow attenuation (1.0 = fully lit, 0.0 = fully shadowed)
				fixed shadow = SHADOW_ATTENUATION(input);

				// darken light's illumination with shadow, keep ambient intact
				fixed3 lighting = (diffuse + specular) * shadow + ambient;
				fixed4 result = fixed4(color.rgb * lighting, color.a);
				///////////////////////////////////////////////////////////////////////////////////

				///////////////////////////////////////////////////////////////////////////////////
				////// Ultra Ring Effect
				fixed distanceToAwaker = distance(_IlluminAwakerPosition, input.worldPos);

				// ring alpha
				fixed ringAlpha = saturate((distanceToAwaker - _IlluminAwakerRadius + _RingRadius) / _RingRadius);

				// illumin
				fixed3 illumin = tex2D(_IlluminTexture, input.uv).a;

				// check if in range
				fixed isInRange = step(distanceToAwaker, _IlluminAwakerRadius);

				result += fixed4(_RingColor * ringAlpha + illumin, 0.0) * isInRange * _IlluminStrength;
				///////////////////////////////////////////////////////////////////////////////////

				return result;
			}
			ENDCG
		}

		//	//Additional Pass,渲染其他光源
		//	Pass
		//	{
		//			//指明光照模式为前向渲染模式
		//			Tags{ "LightMode" = "ForwardAdd" }

		//			//开启混合模式，将计算结果与之前的光照结果进行叠加
		//			Blend One One

		//			CGPROGRAM
		//	#pragma vertex vert  
		//	#pragma fragment frag  
		//			//确保光照衰减等光照变量可以被正确赋值
		//	#pragma multi_compile_fwdadd

		//			//包含引用的内置文件  
		//	#include "Lighting.cginc"  

		//			//声明properties中定义的属性  
		//			fixed4 _Diffuse;
		//			fixed4 _Specular;
		//			float _Gloss;

		//			//定义输入与输出的结构体  
		//			struct a2v
		//			{
		//				float4 vertex : POSITION;
		//				float3 normal : NORMAL;
		//			};

		//			struct VertexOut
		//			{
		//				float4 pos : SV_POSITION;
		//				//存储世界坐标下的法线方向和顶点坐标  
		//				float3 worldNormal : TEXCOORD0;
		//				float3 worldPos : TEXCOORD1;
		//			};

		//			//在顶点着色器中，计算世界坐标下的法线方向和顶点坐标，并传递给片元着色器  
		//			VertexOut vert(a2v v)
		//			{
		//				VertexOut o;
		//				//转换顶点坐标到裁剪空间  
		//				o.pos = UnityObjectToClipPos(v.vertex);
		//				//转换法线坐标到世界空间，直接使用_Object2World转换法线，不能保证转换后法线依然与模型垂直  
		//				o.worldNormal = mul(v.normal, (float3x3)unity_WorldToObject);
		//				//转换顶点坐标到世界空间  
		//				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
		//				return o;
		//			}

		//			//在片元着色器中计算光照模型  
		//			fixed4 frag(VertexOut i) : SV_Target
		//			{
		//			fixed3 worldNormal = normalize(i.worldNormal);

		//			//计算不同的光源方向
		//		#ifdef USING_DIRECTIONAL_LIGHT
		//			fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
		//		#else
		//			fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz - i.worldPos.xyz);
		//		#endif


		//			//计算漫反射光照  
		//			fixed3 diffuse = _LightColor0.rgb * _Diffuse.rgb * saturate(dot(worldNormal,worldLightDir));

		//			//获取视角方向 = 摄像机的世界坐标 - 顶点的世界坐标  
		//			fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);
		//			//计算新矢量h  
		//			fixed3 halfDir = normalize(viewDir + worldLightDir);
		//			//计算高光光照  
		//			fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(saturate(dot(worldNormal,halfDir)),_Gloss);

		//			//处理不同的光源衰减
		//		#ifdef USING_DIRECTIONAL_LIGHT
		//			fixed atten = 1.0;
		//		#else
		//			/*float3 lightCoord = mul(unity_WorldToLight, float4(i.worldPos, 1)).xyz;
		//			fixed atten = tex2D(_LightTexture0, dot(lightCoord, lightCoord).rr).UNITY_ATTEN_CHANNEL;*/
		//			float distance = length(_WorldSpaceLightPos0.xyz - i.worldPos.xyz);
		//			//线性衰减
		//			fixed atten = 1.0 / distance;
		//		#endif

		//			return fixed4((diffuse + specular) * atten,1.0);

		//			}
		//				ENDCG
		//			}
	}
	Fallback"Specular"
}
