// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Building/Wall" {
	Properties {
		// Textures
		_WallTex ("Wall", 2D) = "white" {}
		_WallBumpTex ("Wall Bump", 2D) = "bump" {}
		_TerraceTex ("Terrace", 2D) = "white" {}
		_TerraceBumpTex ("Terrace Bump", 2D) = "bump" {}
		_CornerTex ("Corner", 2D) = "white" {}
		// Building
		_BuildingWidth ("Building Width", Float) = 0
		_BuildingHeight ("Building Height", Float) = 0
		_BuildingDepth ("Building Depth", Float) = 0
		// Hue
		_Hue("Hue", Color) = (1, 1, 1, 1)
		// Corner
		_CornerWidth ("Corner Width", Float) = 0
		_CornerHeight ("Corner Height", Float) = 0
		_CornerMap ("Corner Map", Vector) = (1, 1, 1, 1)
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
		 
		 	sampler2D _WallTex;
		 	sampler2D _WallBumpTex;
		 	sampler2D _TerraceTex;
		 	sampler2D _TerraceBumpTex;
		 	sampler2D _CornerTex;
		 	float4 _WallTex_ST;
		 	float4 _WallBumpTex_ST;
		 	float4 _TerraceTex_ST;
		 	float4 _TerraceBumpTex_ST;
		 	float4 _CornerTex_ST;
		 	float _BuildingWidth;
		 	float _BuildingHeight;
		 	float _BuildingDepth;
		 	float4 _Hue;
		 	float _CornerWidth;
		 	float _CornerHeight;
		 	float4 _CornerMap;
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
			    float2 texcoord1: TEXCOORD0;
			    float2 texcoord2: TEXCOORD1;
			    float4 tangent: TANGENT;
			};
			
			//////////////////////////////////////////////////
			struct v2f
		    {
		        float4 pos: SV_POSITION;
		        float3 normal: COLOR;
		        float height: TEXCOORD0;
		        float2 uv1: TEXCOORD1;
		        float2 uv2: TEXCOORD2;
		        float3 tangentSpaceLightDirection: TEXCOORD3; 
		        LIGHTING_COORDS (4, 5) 
		    };
		    
		    //////////////////////////////////////////////////
		    v2f _VertexShader (a2v v)
		    {
		        v2f o;
		        TANGENT_SPACE_ROTATION; 
		        
		        o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		        o.normal = v.normal;
		        o.height = mul(unity_ObjectToWorld, v.vertex).y;
		        
		        o.tangentSpaceLightDirection = mul(rotation, ObjSpaceLightDir(v.vertex));
		        
		        o.uv1 = TRANSFORM_TEX(v.texcoord1, _WallTex);
		        o.uv2 = TRANSFORM_TEX(v.texcoord2, _WallTex);
		        
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
		    bool IsWall(float3 normal)
		    {
		    	return (dot(normal, float3(0.0, 1.0, 0.0)) == 0.0);
		    }
		    
		    //////////////////////////////////////////////////
		    float GetCornerValue(float3 normal)
		    {
		    	if (normal.x == 0.0)
		    	{
		    		if (normal.z <= -0.9) 
		    		{
		    			return _CornerMap.x;
		    		}
		    		else
		    		{
		    			return _CornerMap.z;
		    		}
		    	}
		    	else
		    	{
		    		if (normal.x >= 0.9)
		    		{
		    			return _CornerMap.w;
		    		}
		    		else
		    		{
		    			return _CornerMap.y;
		    		}
		    	}
		    }
		    
		    //////////////////////////////////////////////////
		    bool HasCorner(float3 normal)
		    {
		    	return (GetCornerValue(normal) > 0.0);
		    }
		    
		    //////////////////////////////////////////////////
		    float EvaluateLeftCorner(float cornerValue)
		    {
		    	if (cornerValue == 0.3 || cornerValue == 1.0)
		    	{
		    		return 1.0;
		    	}
		    	else
		    	{
		    		return 0.0;
		    	}
		    }
		    
		    //////////////////////////////////////////////////
		    float EvaluateRightCorner(float cornerValue)
		    {
		    	if (cornerValue == 0.6 || cornerValue == 1.0)
		    	{
		    		return 1.0;
		    	}
		    	else
		    	{
		    		return 0.0;
		    	}
		    }
		    
		    //////////////////////////////////////////////////
		    float4 ApplyCornerColor(float2 uv, float3 normal, float4 diffuseColor)
		    {
		    	float surfaceWidth;
		    	if (normal.x == 0.0) // front or back walls
		    	{
    				surfaceWidth = _BuildingWidth;
    			}
    			else  // right or left walls
    			{
    				surfaceWidth = _BuildingDepth;
    			}
    			float surfaceHeight = _BuildingHeight;
	    		
	    		float2 cornerUv = uv * float2(surfaceWidth / _CornerWidth, surfaceHeight / _CornerHeight);
	    		
		    	float cutout = (_CornerWidth * 0.5) / surfaceWidth;
		    	
		    	float cornerValue = GetCornerValue(normal);
		    	float cornerCutout1 = 1.0 - step(cutout, uv.x);
		    	float cornerCutout2 = step(1.0 - cutout, uv.x);
		    	
		    	float4 cornerColor1 = tex2D(_CornerTex, float2(cornerUv.x - 0.5, cornerUv.y)) * cornerCutout1 * EvaluateLeftCorner(cornerValue);
		    	float4 cornerColor2 = tex2D(_CornerTex, float2(-cornerUv.x + 0.5, cornerUv.y)) * cornerCutout2 * EvaluateRightCorner(cornerValue);
		    	
		    	return lerp(lerp(diffuseColor, cornerColor1, cornerColor1.a), cornerColor2, cornerColor2.a); 
		    } 
		    
		    //////////////////////////////////////////////////
		    float4 FragmentShader (v2f IN): COLOR
		    { 
		    	float4 wallColor = tex2D (_WallTex, IN.uv1) * _Hue;
		    	float4 terraceColor = tex2D(_TerraceTex, IN.uv2);
		    	float4 cornerColor = ApplyCornerColor(IN.uv2, IN.normal, wallColor);
		    	
		    	float3 wallBumpedNormal = UnpackNormal(tex2D (_WallBumpTex, IN.uv1));
		    	float3 terraceBumpedNormal = UnpackNormal(tex2D (_TerraceBumpTex, IN.uv1));
		    	
		    	float4 diffuseColor;
		    	float3 normal;
		    	float3 lightDirection;
		    	if (IsWall(IN.normal))
		    	{
		    		if (HasCorner(IN.normal))
		    		{
		    			diffuseColor = cornerColor;
		    		}
		    		else
		    		{
		    			diffuseColor = wallColor;
		    		}
		    		normal = wallBumpedNormal;
		    		lightDirection = normalize(IN.tangentSpaceLightDirection);
		    	}
		    	else
		    	{
		    		diffuseColor = terraceColor;
		    		normal = terraceBumpedNormal;
		    		lightDirection = normalize(IN.tangentSpaceLightDirection);
		    	}
		        
		        float diffuseAttenuation = max(dot(normal, lightDirection), 0.0);
	        	float4 finalColor = float4(_LightColor0.rgb * diffuseAttenuation * diffuseColor.rgb * GetLightAttenuation(IN), 1.0);
	        
	        	return ApplyGradient(finalColor, IN.height);
		    }
		 	ENDCG
		}
	}
	FallBack "Diffuse"
}
