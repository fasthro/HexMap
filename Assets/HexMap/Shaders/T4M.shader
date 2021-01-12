// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "T4M_templet"
{
	Properties
	{
		_MaskTex("MaskTex", 2D) = "white" {}
		_TexNum1_R("TexNum1_R", 2D) = "white" {}
		_TexNum3_B("TexNum3_B", 2D) = "white" {}
		_TexNum4_A("TexNum4_A", 2D) = "white" {}
		_TexNum2_G("TexNum2_G", 2D) = "white" {}
		_Color0("Color 0", Color) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows exclude_path:deferred 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _TexNum1_R;
		uniform float4 _TexNum1_R_ST;
		uniform sampler2D _TexNum2_G;
		uniform float4 _TexNum2_G_ST;
		uniform sampler2D _MaskTex;
		uniform float4 _MaskTex_ST;
		uniform sampler2D _TexNum3_B;
		uniform float4 _TexNum3_B_ST;
		uniform sampler2D _TexNum4_A;
		uniform float4 _TexNum4_A_ST;
		uniform float4 _Color0;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_TexNum1_R = i.uv_texcoord * _TexNum1_R_ST.xy + _TexNum1_R_ST.zw;
			float2 uv_TexNum2_G = i.uv_texcoord * _TexNum2_G_ST.xy + _TexNum2_G_ST.zw;
			float2 uv_MaskTex = i.uv_texcoord * _MaskTex_ST.xy + _MaskTex_ST.zw;
			float4 tex2DNode26 = tex2D( _MaskTex, uv_MaskTex );
			float4 lerpResult24 = lerp( tex2D( _TexNum1_R, uv_TexNum1_R ) , tex2D( _TexNum2_G, uv_TexNum2_G ) , tex2DNode26.g);
			float2 uv_TexNum3_B = i.uv_texcoord * _TexNum3_B_ST.xy + _TexNum3_B_ST.zw;
			float4 lerpResult25 = lerp( lerpResult24 , tex2D( _TexNum3_B, uv_TexNum3_B ) , tex2DNode26.b);
			float2 uv_TexNum4_A = i.uv_texcoord * _TexNum4_A_ST.xy + _TexNum4_A_ST.zw;
			float4 lerpResult16 = lerp( lerpResult25 , tex2D( _TexNum4_A, uv_TexNum4_A ) , tex2DNode26.a);
			o.Albedo = ( lerpResult16 * ( _Color0 * 1.3 ) ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17101
1924;48;1906;1004;804.3105;358.8271;1.3;True;True
Node;AmplifyShaderEditor.TexturePropertyNode;7;-940.2062,-636.6745;Inherit;True;Property;_TexNum1_R;TexNum1_R;1;0;Create;True;0;0;False;0;None;b5f185c223088fb499fa4d5bac5d4e77;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.TexturePropertyNode;2;-1067.873,195.4908;Inherit;True;Property;_MaskTex;MaskTex;0;0;Create;True;0;0;False;0;None;747e253a9fecfb045aa017df005d36f8;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.TexturePropertyNode;10;-933.9854,-444.2736;Inherit;True;Property;_TexNum2_G;TexNum2_G;4;0;Create;True;0;0;False;0;None;b5f185c223088fb499fa4d5bac5d4e77;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SamplerNode;12;-676.3062,-636.7905;Inherit;True;Property;_TextureSample0;Texture Sample 0;5;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;26;-676.8102,189.183;Inherit;True;Property;_TextureSample4;Texture Sample 4;5;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;13;-674.9063,-441.7972;Inherit;True;Property;_TextureSample1;Texture Sample 1;5;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;8;-933.9518,-253.8114;Inherit;True;Property;_TexNum3_B;TexNum3_B;2;0;Create;True;0;0;False;0;None;d4962243582d3c546a231a35e41fe98c;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.LerpOp;24;-110.3825,-546.28;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TexturePropertyNode;9;-933.2136,-58.74817;Inherit;True;Property;_TexNum4_A;TexNum4_A;3;0;Create;True;0;0;False;0;None;e50b96b2d5ff8c549a6f6b8711a40958;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SamplerNode;14;-673.9063,-253.7972;Inherit;True;Property;_TextureSample2;Texture Sample 2;6;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;15;-673.9063,-56.79724;Inherit;True;Property;_TextureSample3;Texture Sample 3;7;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;31;329.2826,602.2391;Inherit;False;Constant;_Float0;Float 0;6;0;Create;True;0;0;False;0;1.3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;25;91.29633,-374.392;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;28;302.3497,355.0946;Inherit;False;Property;_Color0;Color 0;5;0;Create;True;0;0;False;0;0,0,0,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;638.2138,332.915;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;16;318.2863,-209.9575;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;29;565.3371,-269.1038;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;751.8077,-274.112;Float;False;True;2;ASEMaterialInspector;0;0;Standard;T4M_templet;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;12;0;7;0
WireConnection;26;0;2;0
WireConnection;13;0;10;0
WireConnection;24;0;12;0
WireConnection;24;1;13;0
WireConnection;24;2;26;2
WireConnection;14;0;8;0
WireConnection;15;0;9;0
WireConnection;25;0;24;0
WireConnection;25;1;14;0
WireConnection;25;2;26;3
WireConnection;30;0;28;0
WireConnection;30;1;31;0
WireConnection;16;0;25;0
WireConnection;16;1;15;0
WireConnection;16;2;26;4
WireConnection;29;0;16;0
WireConnection;29;1;30;0
WireConnection;0;0;29;0
ASEEND*/
//CHKSM=DC7D3607AB624974274E5C4B693AF60CDD076343