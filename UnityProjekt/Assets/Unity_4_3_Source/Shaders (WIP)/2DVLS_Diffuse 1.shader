Shader "2DVLS/Sprites/DiffuseS"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="TransparentCutout" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Back
		Lighting On
        ColorMaterial Emission
		ZWrite On
		Fog { Mode Off }
		//Blend DstColor DstAlpha
		Blend One OneMinusSrcAlpha

		CGPROGRAM
		#pragma surface surf Lambert vertex:vert alphatest:_Cutoff
        #include "UnityCG.cginc"
		sampler2D _MainTex;
		fixed4 _Color;

		struct Input
		{
			float2 uv_MainTex;
			fixed4 color;
		};
		
		void vert (inout appdata_full v, out Input o)
		{
			v.normal = float3(0,0,-1);
			
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.color = v.color * _Color;
		}

		void surf (Input IN, inout SurfaceOutput o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * IN.color;
			o.Albedo = c.rgb * c.a;
			o.Alpha = c.a;
			
			//o.Emission = c.rgb * tex2D(_MainTex, IN.uv_MainTex + float2(0.5, 0)).a * IN.color;
            o.Emission = c.rgb * tex2D(_MainTex, IN.uv_MainTex + float2(0.5, 0)).a;

			fixed4 packednormal = tex2D(_MainTex, IN.uv_MainTex + float2(0.25, 0));
			fixed3 normal;
			normal.xy = packednormal.xy * 2 - 1;
			normal.z = sqrt(1 - saturate(dot(normal.xy, normal.xy)));
			o.Normal = normal;
		}
		ENDCG
	}
}