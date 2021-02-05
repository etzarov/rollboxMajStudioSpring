// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "CutoutScroller"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 1
		_Speed1("Speed", Range( 0 , 0.01)) = 0.01
		_TextureSample1("Texture Sample 0", 2D) = "white" {}
		_Direction("Direction", Int) = -1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _TextureSample1;
		uniform float _Speed1;
		uniform int _Direction;
		uniform float _Cutoff = 1;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float mulTime1 = _Time.y * _Speed1;
			float4 appendResult6 = (float4(( mulTime1 * _Direction ) , 0.0 , 0.0 , 0.0));
			float2 uv_TexCoord3 = i.uv_texcoord + appendResult6.xy;
			float4 tex2DNode5 = tex2D( _TextureSample1, uv_TexCoord3 );
			o.Emission = tex2DNode5.rgb;
			o.Alpha = 1;
			clip( tex2DNode5.a - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18600
204;78;959;636;26.0025;508.8828;1;True;False
Node;AmplifyShaderEditor.RangedFloatNode;2;-1245.699,-203.8237;Inherit;False;Property;_Speed1;Speed;1;0;Create;True;0;0;False;0;False;0.01;0.01;0;0.01;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;1;-923.5223,-135.5659;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.IntNode;10;-1024.662,58.7637;Inherit;False;Property;_Direction;Direction;3;0;Create;True;2;Right;-1;Left;1;0;False;0;False;-1;-1;False;0;1;INT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-683.6622,-105.2363;Inherit;False;2;2;0;FLOAT;0;False;1;INT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;6;-505.9153,-182.6294;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;3;-322.068,-270.3271;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;4;42.93123,-116.72;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;5;226.5451,-226.5744;Inherit;True;Property;_TextureSample1;Texture Sample 0;2;0;Create;True;0;0;False;0;False;-1;d9c02d18c835942d78817230f94893fc;4b3118eefc69142d6bb229847d426f95;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;733.2255,-302.361;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;CutoutScroller;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;1;True;True;0;True;TransparentCutout;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;1;0;2;0
WireConnection;7;0;1;0
WireConnection;7;1;10;0
WireConnection;6;0;7;0
WireConnection;3;1;6;0
WireConnection;4;0;3;1
WireConnection;5;1;3;0
WireConnection;0;2;5;0
WireConnection;0;10;5;4
ASEEND*/
//CHKSM=FF9E900706B8D038E11883EBF18216CCEA998A81