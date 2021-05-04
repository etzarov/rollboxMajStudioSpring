// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "TransitionMaterial"
{
	Properties
	{
		_MainTexture("Main Texture", 2D) = "white" {}
		_BlendMainColor("Blend Main Color", Range( 0 , 1)) = 1
		_MainColor("Main Color", Color) = (1,0.8796263,0.06132078,0)
		_TransitionValue("Transition Value", Range( 0 , 1)) = 0.4931016
		_OffsetBetweenColors("Offset Between Colors", Range( 0 , 0.33)) = 0.1217634
		_TransitionColor1("Transition Color 1", Color) = (0.7264151,0.3460751,0.3460751,0)
		_TransitionColor2("Transition Color 2", Color) = (0.5704551,1,0.4481132,0)
		_TransitionColor3("Transition Color 3", Color) = (0.1179245,0.1988492,1,1)
		_NoiseTiling("Noise Tiling", Vector) = (1,1,0,0)
		_TimeFactor("Time Factor", Range( 0 , 2)) = 0.2352941
		_FadeMap("FadeMap", 2D) = "white" {}
		_UsesFadeMap("UsesFadeMap", Float) = 0
		_VoronoiScale("Voronoi Scale", Float) = 0
		_VoronoiTime("Voronoi Time", Float) = 0
		_Opacity("Opacity", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float4 _MainColor;
		uniform sampler2D _MainTexture;
		uniform float4 _MainTexture_ST;
		uniform float _BlendMainColor;
		uniform float _TransitionValue;
		uniform float _VoronoiScale;
		uniform float _VoronoiTime;
		uniform float2 _NoiseTiling;
		uniform float _TimeFactor;
		uniform sampler2D _FadeMap;
		uniform float _UsesFadeMap;
		uniform float4 _TransitionColor3;
		uniform float _OffsetBetweenColors;
		uniform float4 _TransitionColor1;
		uniform float4 _TransitionColor2;
		uniform float _Opacity;


		float2 voronoihash105( float2 p )
		{
			
			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
			return frac( sin( p ) *43758.5453);
		}


		float voronoi105( float2 v, float time, inout float2 id, inout float2 mr, float smoothness )
		{
			float2 n = floor( v );
			float2 f = frac( v );
			float F1 = 8.0;
			float F2 = 8.0; float2 mg = 0;
			for ( int j = -1; j <= 1; j++ )
			{
				for ( int i = -1; i <= 1; i++ )
			 	{
			 		float2 g = float2( i, j );
			 		float2 o = voronoihash105( n + g );
					o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
					float d = 0.5 * dot( r, r );
			 		if( d<F1 ) {
			 			F2 = F1;
			 			F1 = d; mg = g; mr = r; id = o;
			 		} else if( d<F2 ) {
			 			F2 = d;
			 		}
			 	}
			}
			return F1;
		}


		float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }

		float snoise( float2 v )
		{
			const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
			float2 i = floor( v + dot( v, C.yy ) );
			float2 x0 = v - i + dot( i, C.xx );
			float2 i1;
			i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			i = mod2D289( i );
			float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
			float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
			m = m * m;
			m = m * m;
			float3 x = 2.0 * frac( p * C.www ) - 1.0;
			float3 h = abs( x ) - 0.5;
			float3 ox = floor( x + 0.5 );
			float3 a0 = x - ox;
			m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot( m, g );
		}


		float3 RGBToHSV(float3 c)
		{
			float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
			float4 p = lerp( float4( c.bg, K.wz ), float4( c.gb, K.xy ), step( c.b, c.g ) );
			float4 q = lerp( float4( p.xyw, c.r ), float4( c.r, p.yzx ), step( p.x, c.r ) );
			float d = q.x - min( q.w, q.y );
			float e = 1.0e-10;
			return float3( abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_MainTexture = i.uv_texcoord * _MainTexture_ST.xy + _MainTexture_ST.zw;
			float4 lerpResult14 = lerp( tex2D( _MainTexture, uv_MainTexture ) , _MainColor , _BlendMainColor);
			float time105 = ( _Time.y * _VoronoiTime );
			float4 appendResult104 = (float4(0.0 , ( _Time.y * _TimeFactor ) , 0.0 , 0.0));
			float2 uv_TexCoord89 = i.uv_texcoord * _NoiseTiling + appendResult104.xy;
			float3 ase_objectScale = float3( length( unity_ObjectToWorld[ 0 ].xyz ), length( unity_ObjectToWorld[ 1 ].xyz ), length( unity_ObjectToWorld[ 2 ].xyz ) );
			float3 temp_output_116_0 = ( float3( uv_TexCoord89 ,  0.0 ) * ase_objectScale );
			float2 coords105 = temp_output_116_0.xy * _VoronoiScale;
			float2 id105 = 0;
			float2 uv105 = 0;
			float voroi105 = voronoi105( coords105, time105, id105, uv105, 0 );
			float simplePerlin2D93 = snoise( temp_output_116_0.xy );
			simplePerlin2D93 = simplePerlin2D93*0.5 + 0.5;
			float lerpResult96 = lerp( voroi105 , simplePerlin2D93 , 0.5);
			float4 temp_cast_5 = (lerpResult96).xxxx;
			float4 lerpResult102 = lerp( temp_cast_5 , tex2D( _FadeMap, i.uv_texcoord ) , _UsesFadeMap);
			float4 temp_output_78_0 = ( lerpResult102 * float4( 1,0,0,0 ) );
			float temp_output_3_0_g13 = ( _TransitionValue - temp_output_78_0.r );
			float temp_output_82_0 = saturate( ( temp_output_3_0_g13 / fwidth( temp_output_3_0_g13 ) ) );
			float temp_output_3_0_g2 = ( ( _TransitionValue + _OffsetBetweenColors ) - temp_output_78_0.r );
			float temp_output_69_0 = saturate( ( temp_output_3_0_g2 / fwidth( temp_output_3_0_g2 ) ) );
			float temp_output_3_0_g14 = ( ( ( _OffsetBetweenColors * 2.0 ) + _TransitionValue ) - temp_output_78_0.r );
			float temp_output_68_0 = saturate( ( temp_output_3_0_g14 / fwidth( temp_output_3_0_g14 ) ) );
			float4 blendOpSrc63 = ( ( 1.0 - temp_output_68_0 ) * _TransitionColor1 );
			float4 blendOpDest63 = ( ( _TransitionColor2 * ( 1.0 - temp_output_69_0 ) ) * temp_output_68_0 );
			float4 blendOpSrc88 = ( ( _TransitionColor3 * ( 1.0 - temp_output_82_0 ) ) * temp_output_69_0 );
			float4 blendOpDest88 = ( 1.0 - ( 1.0 - blendOpSrc63 ) * ( 1.0 - blendOpDest63 ) );
			float4 blendOpSrc51 = ( lerpResult14 * temp_output_82_0 );
			float4 blendOpDest51 = ( saturate( ( 1.0 - ( 1.0 - blendOpSrc88 ) * ( 1.0 - blendOpDest88 ) ) ));
			float4 blendOpSrc112 = _MainColor;
			float4 blendOpDest112 = ( saturate( ( 1.0 - ( 1.0 - blendOpSrc51 ) * ( 1.0 - blendOpDest51 ) ) ));
			float4 temp_output_112_0 = ( saturate( 	max( blendOpSrc112, blendOpDest112 ) ));
			o.Emission = temp_output_112_0.rgb;
			float3 hsvTorgb121 = RGBToHSV( _MainColor.rgb );
			float4 temp_cast_11 = (hsvTorgb121.z).xxxx;
			float3 hsvTorgb122 = RGBToHSV( _TransitionColor1.rgb );
			float4 temp_cast_13 = (hsvTorgb122.z).xxxx;
			float4 temp_output_120_0 = (float4( 0,0,0,0 ) + (temp_output_112_0 - temp_cast_11) * (float4( 1,1,1,1 ) - float4( 0,0,0,0 )) / (temp_cast_13 - temp_cast_11));
			o.Alpha = saturate( ( temp_output_120_0 + _Opacity ) ).r;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard alpha:fade keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18900
653;791;973;276;847.4731;363.3583;1.480188;True;False
Node;AmplifyShaderEditor.SimpleTimeNode;91;-3882.795,291.1702;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;98;-3982.82,413.7227;Inherit;False;Property;_TimeFactor;Time Factor;9;0;Create;True;0;0;0;False;0;False;0.2352941;0.665;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;99;-3652.795,340.1702;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;90;-3621.853,-191.4975;Inherit;False;Property;_NoiseTiling;Noise Tiling;8;0;Create;True;0;0;0;False;0;False;1,1;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.DynamicAppendNode;104;-3482.826,193.938;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleTimeNode;108;-3258.236,-230.5372;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;107;-3298.236,-145.5372;Inherit;False;Property;_VoronoiTime;Voronoi Time;13;0;Create;True;0;0;0;False;0;False;0;1.45;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;89;-3301.795,88.17023;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ObjectScaleNode;115;-3329.3,290.478;Inherit;False;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;116;-3167.61,237.2583;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;109;-3092.236,-144.5372;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;106;-3298.742,-51.62801;Inherit;False;Property;_VoronoiScale;Voronoi Scale;12;0;Create;True;0;0;0;False;0;False;0;4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.VoronoiNode;105;-2951.919,-85.37177;Inherit;False;0;0;1;0;1;False;1;False;False;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;3;FLOAT;0;FLOAT2;1;FLOAT2;2
Node;AmplifyShaderEditor.NoiseGeneratorNode;93;-2981.795,122.1702;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;100;-3092.453,-368.8164;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;96;-2641.548,-102.503;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;101;-2737.905,-407.3483;Inherit;True;Property;_FadeMap;FadeMap;10;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;103;-2473.849,12.62132;Inherit;False;Property;_UsesFadeMap;UsesFadeMap;11;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;15;-2538.151,248.0008;Inherit;False;Property;_TransitionValue;Transition Value;3;0;Create;True;0;0;0;False;0;False;0.4931016;0.379;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;22;-2592.608,442.7785;Inherit;False;Property;_OffsetBetweenColors;Offset Between Colors;4;0;Create;True;0;0;0;False;0;False;0.1217634;0.1217634;0;0.33;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;102;-2416.283,-221.141;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;29;-2081.439,409.3293;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;78;-2249.57,-119.5836;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;1,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;56;-2077.624,289.3329;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;69;-1389.928,239.3974;Inherit;True;Step Antialiasing;-1;;2;2a825e80dfb3290468194f83380797bd;0;2;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;30;-1910.571,396.2574;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;61;-1195.789,64.37627;Inherit;False;Property;_TransitionColor2;Transition Color 2;6;0;Create;True;0;0;0;False;0;False;0.5704551,1,0.4481132,0;0.634523,0.8809644,0.9150943,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;82;-1276.505,-200.2241;Inherit;True;Step Antialiasing;-1;;13;2a825e80dfb3290468194f83380797bd;0;2;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;68;-1399.454,469.7184;Inherit;True;Step Antialiasing;-1;;14;2a825e80dfb3290468194f83380797bd;0;2;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;59;-1167.071,260.1753;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;43;-1087.295,480.9671;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;86;-831.1896,-308.8995;Inherit;False;Property;_TransitionColor3;Transition Color 3;7;0;Create;True;0;0;0;False;0;False;0.1179245,0.1988492,1,1;0.5453453,0.8323784,0.8962264,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;83;-1048.104,-972.0635;Inherit;False;858.9406;515.9999;Main Texture;5;1;4;14;3;46;;1,1,1,1;0;0
Node;AmplifyShaderEditor.ColorNode;34;-964.9054,698.9844;Inherit;False;Property;_TransitionColor1;Transition Color 1;5;0;Create;True;0;0;0;False;0;False;0.7264151,0.3460751,0.3460751,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;62;-957.8037,212.5629;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;84;-1009.29,-160.6995;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;85;-693.3895,-122.9995;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;1;-998.1036,-922.0635;Inherit;True;Property;_MainTexture;Main Texture;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;3;-944.03,-635.2223;Inherit;False;Property;_MainColor;Main Color;2;0;Create;True;0;0;0;False;0;False;1,0.8796263,0.06132078,0;0.7126202,0.915664,0.9622642,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;67;-717.6515,361.6773;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;45;-802.9855,458.8384;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-938.1037,-733.0635;Inherit;False;Property;_BlendMainColor;Blend Main Color;1;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;87;-542.5873,-70.99978;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.BlendOpsNode;63;-550.3671,436.773;Inherit;True;Screen;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;14;-630.103,-690.0635;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.BlendOpsNode;88;-329.3831,207.1999;Inherit;False;Screen;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;46;-424.163,-729.2252;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.BlendOpsNode;51;-64.6637,120.1863;Inherit;True;Screen;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.BlendOpsNode;112;284.0205,68.23384;Inherit;False;Lighten;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.RGBToHSVNode;122;11.18143,-196.9462;Inherit;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RGBToHSVNode;121;16.15217,-334.7395;Inherit;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TFHCRemapNode;120;378.2635,-392.4127;Inherit;False;5;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,1;False;3;COLOR;0,0,0,0;False;4;COLOR;1,1,1,1;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;110;494.9857,-156.6524;Inherit;False;Property;_Opacity;Opacity;14;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;124;635.2144,-200.6297;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;111;-86.8461,459.6689;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;125;725.8107,-136.1782;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;126;672.5894,-309.6316;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;860.6942,-48.95617;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;TransitionMaterial;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;True;0;True;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;99;0;91;0
WireConnection;99;1;98;0
WireConnection;104;1;99;0
WireConnection;89;0;90;0
WireConnection;89;1;104;0
WireConnection;116;0;89;0
WireConnection;116;1;115;0
WireConnection;109;0;108;0
WireConnection;109;1;107;0
WireConnection;105;0;116;0
WireConnection;105;1;109;0
WireConnection;105;2;106;0
WireConnection;93;0;116;0
WireConnection;96;0;105;0
WireConnection;96;1;93;0
WireConnection;101;1;100;0
WireConnection;102;0;96;0
WireConnection;102;1;101;0
WireConnection;102;2;103;0
WireConnection;29;0;22;0
WireConnection;78;0;102;0
WireConnection;56;0;15;0
WireConnection;56;1;22;0
WireConnection;69;1;78;0
WireConnection;69;2;56;0
WireConnection;30;0;29;0
WireConnection;30;1;15;0
WireConnection;82;1;78;0
WireConnection;82;2;15;0
WireConnection;68;1;78;0
WireConnection;68;2;30;0
WireConnection;59;0;69;0
WireConnection;43;0;68;0
WireConnection;62;0;61;0
WireConnection;62;1;59;0
WireConnection;84;0;82;0
WireConnection;85;0;86;0
WireConnection;85;1;84;0
WireConnection;67;0;62;0
WireConnection;67;1;68;0
WireConnection;45;0;43;0
WireConnection;45;1;34;0
WireConnection;87;0;85;0
WireConnection;87;1;69;0
WireConnection;63;0;45;0
WireConnection;63;1;67;0
WireConnection;14;0;1;0
WireConnection;14;1;3;0
WireConnection;14;2;4;0
WireConnection;88;0;87;0
WireConnection;88;1;63;0
WireConnection;46;0;14;0
WireConnection;46;1;82;0
WireConnection;51;0;46;0
WireConnection;51;1;88;0
WireConnection;112;0;3;0
WireConnection;112;1;51;0
WireConnection;122;0;34;0
WireConnection;121;0;3;0
WireConnection;120;0;112;0
WireConnection;120;1;121;3
WireConnection;120;2;122;3
WireConnection;124;0;120;0
WireConnection;124;1;110;0
WireConnection;125;0;124;0
WireConnection;126;0;120;0
WireConnection;126;1;110;0
WireConnection;0;2;112;0
WireConnection;0;9;125;0
ASEEND*/
//CHKSM=86A40D3B6925671F7318BB2A14F19A6EE8A2B658