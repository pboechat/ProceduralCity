Shader "Building/Bumped Diffuse" {
	Properties {
		// Textures
		_MainTex ("Main Texture", 2D) = "white" {}
		// Gradient
		_GradientColor ("Gradient Color", Color) = (1, 1, 1, 1)
		_GradientStart ("Gradient Start", Float) = 0
		_GradientEnd ("Gradient End", Float) = 1
		_GradientHeight ("Gradient Height", Float) = 0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		Cull Back 
	
		////////////////////////////////////////////////////////////////////////////////////////////////////
		Pass {
			Tags { "LightMode"="ForwardBase" }
		
		    CGPROGRAM

			#pragma target 3.0
			#pragma multi_compile_fwdbase 
			#pragma exclude_renderers flash 
		    #pragma vertex _VertexShader
		    #pragma fragment FragmentShader
		 
		    #include "UnityCG.cginc"
		    #include "AutoLight.cginc"
		 
		 	sampler2D _MainTex;
		 	float4 _MainTex_ST;
		 	float4 _GradientColor;
		 	float _GradientEnd;
		 	float _GradientStart;
		 	float _GradientHeight;
		 	float4 _LightColor0;
		 
		 	//////////////////////////////////////////////////
		 	struct a2v
			{
			    float4 vertex: POSITION;
			    float3 normal: NORMAL;
			    float4 color: COLOR;
			    float2 texcoord1: TEXCOORD0;
			    float2 texcoord2: TEXCOORD1;
			    float4 tangent: TANGENT;
			};
			
			//////////////////////////////////////////////////
			struct v2f
		    {
		        float4 pos: SV_POSITION;
		        float4 color: COLOR;
		        float height: TEXCOORD0;
		        float2 uv1: TEXCOORD1;
		        float2 uv2: TEXCOORD2;
		        float3 lightDirection: TEXCOORD3; 
		        LIGHTING_COORDS (4, 5) 
		    };
		    
		    //////////////////////////////////////////////////
		    v2f _VertexShader (a2v v)
		    {
		        v2f o;
		        TANGENT_SPACE_ROTATION;
		        
		        o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		        o.color = v.color;
		        o.height = mul(_Object2World, v.vertex).y;
		        
		        o.lightDirection = mul(rotation, ObjSpaceLightDir(v.vertex));
		        
		        o.uv1 = TRANSFORM_TEX(v.texcoord1, _MainTex);
		        o.uv2 = TRANSFORM_TEX(v.texcoord2, _MainTex);
		        
		        TRANSFER_VERTEX_TO_FRAGMENT(o);
		        
		        return o;
		    }
		    
		    //////////////////////////////////////////////////
		    float GetLightAttenuation(v2f IN)
		    {
#if SHADER_API_D3D11
				return LIGHT_ATTENUATION(IN);
#else
				return LIGHT_ATTENUATION(IN) * 2.0;
#endif
		    }
		    
		    //////////////////////////////////////////////////
		    float4 ApplyGradient(float4 color, float height)
		    {
		    	return lerp(_GradientColor, color, lerp(_GradientStart, _GradientEnd, clamp(height, 0, _GradientHeight) / _GradientHeight));
		    }
		    
		    //////////////////////////////////////////////////
		    float4 FragmentShader (v2f IN): COLOR
		    { 
		    	float4 diffuseColor = tex2D (_MainTex, IN.uv1) * IN.color;
		    	float3 normal;
		    	if (IN.uv2.x == 0 || IN.uv2.y == 0) {
		    		normal = float3(0.0, 0.0, 1.0);
		    	} else {
		    		normal = UnpackNormal(tex2D (_MainTex, IN.uv2));
		    	}
		    	float3 lightDirection = normalize(IN.lightDirection);
		        float diffuseAttenuation = max(dot(normalize(normal), lightDirection), 0.0);
	        	float4 finalColor = float4(_LightColor0.rgb * diffuseAttenuation * diffuseColor.rgb * GetLightAttenuation(IN), 1.0);
	        	return ApplyGradient(finalColor, IN.height);
		    }
		 	ENDCG
		}
	}
	FallBack "Diffuse"
}
