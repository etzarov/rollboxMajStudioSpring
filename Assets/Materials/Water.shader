// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Water"
{
	Properties
	{
		_CameraView("Camera View", 2D) = "white" {}
		_WaterColor("Water Color", Color) = (0,0,0,0)
		_OutlineColor("Outline Color", Color) = (0.3174618,0.6921997,0.7735849,0)
		_WaterLine("Water Line", Float) = 0.4
		_WaterFadeDistance("Water Fade Distance", Range( 0 , 0.5)) = 0.02352941
		_OutlineOpacity("Outline Opacity", Range( 0 , 1)) = 0
		_WaterOpaqueness("Water Opaqueness", Range( 0 , 1)) = 0
		_NoiseScale("Noise Scale", Range( 0 , 20)) = 0
		_NoiseAmp("Noise Amp", Range( 0 , 1)) = 0
		_NoiseTime("Noise Time", Range( 0 , 1)) = 0
		_BubbleSize("Bubble Size", Float) = 11.1
		_BubbleSolidness("Bubble Solidness", Float) = 0
		_BubbleSpeed("Bubble Speed", Float) = 0
		_BubbleRiseSpeed("Bubble Rise Speed", Float) = 0
		_BubbleOpacity("Bubble Opacity", Float) = 0
		_BubbleNoiseSize("Bubble Noise Size", Float) = 9.981029
		_BubbleNoiseScale("Bubble Noise Scale", Float) = 0
		_WaterFlowX("Water Flow X", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 4.0
		#pragma surface surf Unlit keepalpha addshadow fullforwardshadows exclude_path:deferred 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
		};

		uniform sampler2D _CameraView;
		uniform float _NoiseTime;
		uniform float _NoiseScale;
		uniform float _NoiseAmp;
		uniform float4 _WaterColor;
		uniform float _WaterOpaqueness;
		uniform float4 _OutlineColor;
		uniform float _WaterLine;
		uniform float _WaterFadeDistance;
		uniform float _BubbleOpacity;
		uniform float _BubbleSize;
		uniform float _BubbleSpeed;
		uniform float _WaterFlowX;
		uniform float _BubbleRiseSpeed;
		uniform float _BubbleNoiseScale;
		uniform float _BubbleNoiseSize;
		uniform float _BubbleSolidness;
		uniform float _OutlineOpacity;


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


		float2 voronoihash92( float2 p )
		{
			
			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
			return frac( sin( p ) *43758.5453);
		}


		float voronoi92( float2 v, float time, inout float2 id, inout float2 mr, float smoothness )
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
			 		float2 o = voronoihash92( n + g );
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


		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 break84 = i.uv_texcoord;
			float4 appendResult88 = (float4(( break84.x + ( ( _NoiseTime / 40.0 ) * _Time.y ) ) , break84.y , 0.0 , 0.0));
			float simplePerlin2D77 = snoise( appendResult88.xy*_NoiseScale );
			simplePerlin2D77 = simplePerlin2D77*0.5 + 0.5;
			float2 temp_cast_1 = (simplePerlin2D77).xx;
			float2 lerpResult82 = lerp( i.uv_texcoord , temp_cast_1 , ( _NoiseAmp / 40.0 ));
			float4 lerpResult74 = lerp( ( tex2D( _CameraView, lerpResult82 ) * _WaterColor ) , _WaterColor , _WaterOpaqueness);
			float clampResult68 = clamp( (0.0 + (i.uv_texcoord.y - ( _WaterLine - _WaterFadeDistance )) * (1.0 - 0.0) / (_WaterLine - ( _WaterLine - _WaterFadeDistance ))) , 0.0 , 1.0 );
			float ifLocalVar63 = 0;
			if( i.uv_texcoord.y > _WaterLine )
				ifLocalVar63 = 1.0;
			else if( i.uv_texcoord.y < _WaterLine )
				ifLocalVar63 = clampResult68;
			float time92 = ( _BubbleSpeed * _Time.y );
			float3 ase_worldPos = i.worldPos;
			float3 break110 = ase_worldPos;
			float4 appendResult113 = (float4(( ( _Time.y * _WaterFlowX ) + break110.x ) , ( ( _Time.y * _BubbleRiseSpeed ) + break110.y ) , break110.z , 0.0));
			float2 coords92 = appendResult113.xy * _BubbleSize;
			float2 id92 = 0;
			float2 uv92 = 0;
			float voroi92 = voronoi92( coords92, time92, id92, uv92, 0 );
			float simplePerlin2D123 = snoise( ase_worldPos.xy*_BubbleNoiseScale );
			simplePerlin2D123 = simplePerlin2D123*0.5 + 0.5;
			float ifLocalVar99 = 0;
			if( pow( ( 1.0 - voroi92 ) , ( ( ( simplePerlin2D123 * _BubbleNoiseSize ) + _BubbleSolidness ) * 20.0 ) ) > 0.5 )
				ifLocalVar99 = 1.0;
			else if( pow( ( 1.0 - voroi92 ) , ( ( ( simplePerlin2D123 * _BubbleNoiseSize ) + _BubbleSolidness ) * 20.0 ) ) < 0.5 )
				ifLocalVar99 = 0.0;
			float blendOpSrc117 = ifLocalVar63;
			float blendOpDest117 = ( _BubbleOpacity * ifLocalVar99 );
			float4 lerpResult65 = lerp( lerpResult74 , _OutlineColor , ( ( saturate( 	max( blendOpSrc117, blendOpDest117 ) )) * _OutlineOpacity ));
			o.Emission = lerpResult65.rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18712
0;6;1920;1013;253.8808;347.5682;1.210605;True;False
Node;AmplifyShaderEditor.SimpleTimeNode;89;259.3128,-83.97769;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;112;251.301,119.2986;Inherit;False;Property;_BubbleRiseSpeed;Bubble Rise Speed;13;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;125;252.3036,262.1548;Inherit;False;Property;_WaterFlowX;Water Flow X;17;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;118;632.2704,447.0824;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;85;-0.6871948,-367.9777;Inherit;False;Property;_NoiseTime;Noise Time;9;0;Create;True;0;0;0;False;0;False;0;0.613;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;114;554.3011,113.2986;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;124;618.5646,844.679;Inherit;False;Property;_BubbleNoiseScale;Bubble Noise Scale;16;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;110;886.301,347.2986;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;126;562.0215,245.6151;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;127;1090.021,230.6151;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;111;1082.301,339.2986;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;123;877.2644,810.8786;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;120;1568.411,670.4241;Inherit;False;Property;_BubbleNoiseSize;Bubble Noise Size;15;0;Create;True;0;0;0;False;0;False;9.981029;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;75;391.9783,-742.1749;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleDivideOpNode;90;291.3128,-377.9777;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;40;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;107;837.7013,632.2986;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;109;868.3011,539.2986;Inherit;False;Property;_BubbleSpeed;Bubble Speed;12;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;94;1227.124,707.9907;Inherit;False;Property;_BubbleSize;Bubble Size;10;0;Create;True;0;0;0;False;0;False;11.1;20.57;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;119;1765.411,537.4243;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;113;1252.301,340.2986;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;108;1057.301,574.2986;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;84;612.3128,-598.9777;Inherit;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.RangedFloatNode;103;1503.849,780.4981;Inherit;False;Property;_BubbleSolidness;Bubble Solidness;11;0;Create;True;0;0;0;False;0;False;0;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;87;436.3128,-331.9777;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;62;988.246,58.39243;Inherit;False;Property;_WaterFadeDistance;Water Fade Distance;4;0;Create;True;0;0;0;False;0;False;0.02352941;0.004;0;0.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;52;1179.246,-45.60759;Inherit;False;Property;_WaterLine;Water Line;3;0;Create;True;0;0;0;False;0;False;0.4;0.184;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;86;626.3128,-432.9777;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VoronoiNode;92;1419.831,424.5827;Inherit;True;0;0;1;0;1;False;1;False;False;4;0;FLOAT2;0,0;False;1;FLOAT;3.87;False;2;FLOAT;1;False;3;FLOAT;0;False;3;FLOAT;0;FLOAT2;1;FLOAT2;2
Node;AmplifyShaderEditor.SimpleAddOpNode;122;1879.092,647.4051;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;105;1980.849,537.4981;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;20;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;88;769.3128,-590.9777;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;61;1222.246,-189.6076;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;96;1737.124,343.9905;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;69;1339.531,193.818;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;78;599.3127,-196.9777;Inherit;False;Property;_NoiseScale;Noise Scale;7;0;Create;True;0;0;0;False;0;False;0;20;0;20;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;80;694.313,-48.97771;Inherit;False;Property;_NoiseAmp;Noise Amp;8;0;Create;True;0;0;0;False;0;False;0;0.395;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;101;2316.25,751.0981;Inherit;False;Constant;_Float2;Float 2;11;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;102;2313.25,669.0981;Inherit;False;Constant;_Float3;Float 3;11;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;100;2320.25,575.0981;Inherit;False;Constant;_Float1;Float 1;11;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;91;1010.313,-160.9777;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;40;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;77;934.313,-587.9777;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;95;2135.924,375.3906;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;58.67;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;66;1557.191,48.24643;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;64;1669.246,-9.607591;Inherit;False;Constant;_Float0;Float 0;5;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;68;1786.191,116.2464;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;82;1282.313,-653.9777;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;115;2474.557,397.8051;Inherit;False;Property;_BubbleOpacity;Bubble Opacity;14;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;99;2501.25,517.0981;Inherit;False;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;46;1629.173,-857.2809;Inherit;True;Property;_CameraView;Camera View;0;0;Create;True;0;0;0;False;0;False;-1;None;278bad528eacc2b46937763a0c12f388;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;48;1650.245,-635.6708;Inherit;False;Property;_WaterColor;Water Color;1;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.3001513,0.6838319,0.8962264,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;116;2694.119,404.8499;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;63;1850.246,-161.6076;Inherit;False;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;70;2925.368,352.3045;Inherit;False;Property;_OutlineOpacity;Outline Opacity;5;0;Create;True;0;0;0;False;0;False;0;0.58;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.BlendOpsNode;117;2858.955,124.2313;Inherit;False;Lighten;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;73;1635.129,-454.886;Inherit;False;Property;_WaterOpaqueness;Water Opaqueness;6;0;Create;True;0;0;0;False;0;False;0;0.793;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;47;1976.614,-820.9824;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;71;3133.368,201.3046;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;50;1815.056,-345.3499;Inherit;False;Property;_OutlineColor;Outline Color;2;0;Create;True;0;0;0;False;0;False;0.3174618,0.6921997,0.7735849,0;0.6745283,0.9090593,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;74;2172.686,-730.7437;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;65;3313.083,7.878933;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;3631.577,-37.67949;Float;False;True;-1;4;ASEMaterialInspector;0;0;Unlit;Water;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0.55;0.2355375,0.7192774,0.745283,0.7647059;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;114;0;89;0
WireConnection;114;1;112;0
WireConnection;110;0;118;0
WireConnection;126;0;89;0
WireConnection;126;1;125;0
WireConnection;127;0;126;0
WireConnection;127;1;110;0
WireConnection;111;0;114;0
WireConnection;111;1;110;1
WireConnection;123;0;118;0
WireConnection;123;1;124;0
WireConnection;90;0;85;0
WireConnection;119;0;123;0
WireConnection;119;1;120;0
WireConnection;113;0;127;0
WireConnection;113;1;111;0
WireConnection;113;2;110;2
WireConnection;108;0;109;0
WireConnection;108;1;107;0
WireConnection;84;0;75;0
WireConnection;87;0;90;0
WireConnection;87;1;89;0
WireConnection;86;0;84;0
WireConnection;86;1;87;0
WireConnection;92;0;113;0
WireConnection;92;1;108;0
WireConnection;92;2;94;0
WireConnection;122;0;119;0
WireConnection;122;1;103;0
WireConnection;105;0;122;0
WireConnection;88;0;86;0
WireConnection;88;1;84;1
WireConnection;96;0;92;0
WireConnection;69;0;52;0
WireConnection;69;1;62;0
WireConnection;91;0;80;0
WireConnection;77;0;88;0
WireConnection;77;1;78;0
WireConnection;95;0;96;0
WireConnection;95;1;105;0
WireConnection;66;0;61;2
WireConnection;66;1;69;0
WireConnection;66;2;52;0
WireConnection;68;0;66;0
WireConnection;82;0;75;0
WireConnection;82;1;77;0
WireConnection;82;2;91;0
WireConnection;99;0;95;0
WireConnection;99;1;100;0
WireConnection;99;2;102;0
WireConnection;99;4;101;0
WireConnection;46;1;82;0
WireConnection;116;0;115;0
WireConnection;116;1;99;0
WireConnection;63;0;61;2
WireConnection;63;1;52;0
WireConnection;63;2;64;0
WireConnection;63;4;68;0
WireConnection;117;0;63;0
WireConnection;117;1;116;0
WireConnection;47;0;46;0
WireConnection;47;1;48;0
WireConnection;71;0;117;0
WireConnection;71;1;70;0
WireConnection;74;0;47;0
WireConnection;74;1;48;0
WireConnection;74;2;73;0
WireConnection;65;0;74;0
WireConnection;65;1;50;0
WireConnection;65;2;71;0
WireConnection;0;2;65;0
ASEEND*/
//CHKSM=6582B4B8146F9F068C3EE818908C1734A351CA49