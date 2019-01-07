Shader "Custom/FakeVolumetricShader"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_MainTexScrollSpeeds("MainTex Scroll Speeds", vector) = (0, 0, 0, 0)

		_SecondTex("Albedo (RGB)", 2D) = "white" {}
		_SecondTexScrollSpeeds("MainTex Scroll Speeds", vector) = (0, 0, 0, 0)

		_LightningConcentration("Lightning concentration", Float) = 1.0
		_MinLightningValue("Min Lightning value", Range(0.0,1.0)) = 0.0

		_Intensity("Color Intensity", Float) = 1.0
		_RimPower("Rim Power", Float) = 1.0
		_CloudDensity("Cloud Density", Float) = 1.0

	}
		SubShader
		{
			Tags {"Queue" = "Transparent" "RenderType" = "Transparent" }
			LOD 200

			CGPROGRAM
			half4 _Color;
			#pragma surface surf SimpleVolumetric alpha:blend

			struct Input
			{
				float2 uv_MainTex;
				float2 uv_SecondTex;
				float3 viewDir;
				float3 worldNormal;
			};

			sampler2D _MainTex;
			sampler2D _SecondTex;

			float _LightningConcentration;
			float _MinLightningValue;


			float _Intensity;
			float _CloudDensity;
			float _RimPower;
			float4 _MainTexScrollSpeeds;
			float4 _SecondTexScrollSpeeds;

			//Custom Volumetric light model
			half4 LightingSimpleVolumetric(SurfaceOutput s, half3 lightDir, half3 viewDir, half atten) {
				half3 h = normalize(lightDir + viewDir);

				float NdotL = abs(dot(s.Normal, lightDir));
				float diff = max(_MinLightningValue, pow(NdotL, _LightningConcentration));

				//negative normals are transformed to positive. To simulate the fact that light run through the object
				float nh = max(0, dot(s.Normal, h));

				half4 c;
				c.rgb = (s.Albedo * _LightColor0.rgb * diff * _LightColor0.rgb*atten);
				c.a = s.Alpha;
				return c;
			}


			void surf(Input IN, inout SurfaceOutput o) {

				//Texture translations
				IN.uv_MainTex += _MainTexScrollSpeeds * _Time.x;
				IN.uv_SecondTex += _SecondTexScrollSpeeds * _Time.x;

				fixed4 mainTextureColor = tex2D(_MainTex, IN.uv_MainTex);
				fixed4 secondTextureColor = tex2D(_SecondTex, IN.uv_SecondTex);

				//(text1 + text2) + RimEffect
				fixed4 textureColor = (mainTextureColor + secondTextureColor) + abs(pow(dot(IN.viewDir, IN.worldNormal), _RimPower));

				fixed4 c = saturate(textureColor)  * _Color;
				o.Albedo = textureColor.rgb * _Intensity;

				//change alpha pixel based on cloud density parameter
				c.a = saturate(textureColor.r - _CloudDensity);
				o.Alpha = c.a;
			}
			ENDCG
		}
			FallBack "Diffuse"
}
