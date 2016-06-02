Shader "Road/Block Ground" {
	Properties {
		_AsphaltTex ("Asphalt", 2D) = "white" {}
		_SidewalkTex ("Sidewalk", 2D) = "white" {}
		_BlockInteriorTex ("Block Interior", 2D) = "white" {}
		_HalfRoadWidth ("Half Road Width", Float) = 0
		_SidewalkWidth ("Sidewalk Width", Float) = 0
		_BlockGroundWidth ("Block Ground Width", Float) = 0
		_BlockGroundHeight ("Block Ground Height", Float) = 0
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }
		Cull Back 
	  
	  	Pass {
	  		Tags { "LightMode"="ForwardBase" }
	  	
			CGPROGRAM
			#pragma target 3.0
			#pragma multi_compile_fwdbase
			#pragma vertex _VertexShader
			#pragma fragment _FragmentShader
			 
		    #include "UnityCG.cginc"
		    #include "AutoLight.cginc"
		  
			sampler2D _AsphaltTex;
			sampler2D _SidewalkTex;
			sampler2D _BlockInteriorTex;
			float2 _AsphaltTex_ST;
			float2 _SidewalkTex_ST;
			float2 _BlockInteriorTex_ST;
			float _HalfRoadWidth;
			float _SidewalkWidth;
			float _BlockGroundWidth;
			float _BlockGroundHeight;
			float4 _LightColor0;
			//uniform half4 unity_FogColor;
		    uniform half4 unity_FogStart;
		    uniform half4 unity_FogEnd;			
			
			//////////////////////////////////////////////////
		 	struct a2v
			{
			    float4 vertex: POSITION;
			    float3 normal: NORMAL;
			    float2 texcoord1: TEXCOORD0;
			};
			
			//////////////////////////////////////////////////
			struct v2f
		    {
		        float4 pos: SV_POSITION;
		        float3 normal: TEXCOORD1;
		        float2 uv: TEXCOORD2;
		        float3 lightDirection: TEXCOORD3; 
		        float fog: TEXCOORD4;
		        LIGHTING_COORDS (5, 6) 
		    };
		    
		    //////////////////////////////////////////////////
		    v2f _VertexShader (a2v v)
		    {
		        v2f o;
		        
		        o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		        o.normal = v.normal;
		        o.lightDirection = ObjSpaceLightDir(v.vertex);
		        o.uv = v.texcoord1;
		        
		        float dist = length(mul(UNITY_MATRIX_MV, v.vertex).xyz);
				float diff = unity_FogEnd.x - unity_FogStart.x;
				float invDiff = 1.0f / diff;
				o.fog = clamp ((unity_FogEnd.x - dist) * invDiff, 0.0, 1.0);
		        
		        TRANSFER_VERTEX_TO_FRAGMENT(o);
		        
		        return o;
		    }
		    
		    //////////////////////////////////////////////////
		    float4 _FragmentShader (v2f IN): COLOR
		    { 
		    	float4 asphaltTexel = tex2D(_AsphaltTex, IN.uv * _AsphaltTex_ST);
		    	float4 sidewalkTexel = tex2D(_SidewalkTex, IN.uv * _SidewalkTex_ST);
		    	float4 blockInteriorTexel = tex2D(_BlockInteriorTex, IN.uv * _BlockInteriorTex_ST);
		    
		    	float4 diffuseColor;
		    
		    	if (IN.uv.x >= _HalfRoadWidth && IN.uv.x <= (_BlockGroundWidth - _HalfRoadWidth) &&
			    	IN.uv.y >= _HalfRoadWidth && IN.uv.y <= (_BlockGroundHeight - _HalfRoadWidth)) {
					diffuseColor = blockInteriorTexel; //float4(0, 0.333, 0, 1);
				}
				else {
					if (IN.uv.x >= (_HalfRoadWidth - _SidewalkWidth) && IN.uv.x <= (_BlockGroundWidth - _HalfRoadWidth + _SidewalkWidth) && 
						IN.uv.y >= (_HalfRoadWidth - _SidewalkWidth) && IN.uv.y <= (_BlockGroundHeight - _HalfRoadWidth + _SidewalkWidth)) {
						if ((IN.uv.x <= _HalfRoadWidth && IN.uv.y <= _HalfRoadWidth) || 
						    (IN.uv.x >= _BlockGroundWidth - _HalfRoadWidth && IN.uv.y <= _HalfRoadWidth) ||
						    (IN.uv.x <= _HalfRoadWidth && IN.uv.y >= _BlockGroundHeight - _HalfRoadWidth) ||
						    (IN.uv.x >= _BlockGroundWidth - _HalfRoadWidth && IN.uv.y >= _BlockGroundHeight - _HalfRoadWidth)) {
							diffuseColor = sidewalkTexel; //float4(1, 0, 0, 1);
						} else {
							diffuseColor = sidewalkTexel; //float4(0.666, 0.666, 0.666, 1);
						}
					} else {
						diffuseColor = asphaltTexel; //float4(0.333, 0.333, 0.333, 1);
					}
				}
				
				float diffuseAttenuation = max(0.0, dot(normalize(IN.normal), normalize(IN.lightDirection)));
				
				float4 finalColor = _LightColor0 * diffuseColor * diffuseAttenuation * (LIGHT_ATTENUATION(IN) * 2);
				
				float4 fogColor = unity_FogColor;
				#ifdef UNITY_PASS_FORWARDADD
				fogColor = 0;
				#endif
				
				return lerp(fogColor, finalColor, IN.fog);
		    }
		  
			ENDCG
		}
	}
	Fallback "Diffuse"
}
