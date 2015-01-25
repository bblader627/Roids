// Created By Aidan Wilson
// Date Started: 10/09/2013
// Additional Notes: Shader based on FX/Glass, from Unity's standard assets.

Shader "Shield FX/Advanced" {
Properties {
	_RefractionRate  ("Refraction", range (0,128)) = 10
	_MainTex ("Detail Map", 2D) = "white" {}
	_RimMap ("Rim Map *Note: Currently UV Offset and Tiling is taken from Detail Map", 2D) = "white" {}
	_RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0.0)
    _RimPower ("Rim Power", Range(0.1, 16.0)) = 3.0
    _ColourModifier ("Colour Modifier", Range(0.01, 1.0)) =0.075
}

Category {

	// The grab pass is an unmodified section from FX/Glass.

	// We must be transparent, so other objects are drawn before this one.
	Tags { "Queue"="Transparent" "RenderType"="Opaque" }


	SubShader {

		// This pass grabs the screen behind the object into a texture.
		// We can access the result in the next pass as _GrabTexture
		GrabPass {							
			Name "BASE"
			Tags { "LightMode" = "Always" }
 		}
 		
 		// Main pass: Take the texture grabbed above and use the bumpmap to perturb it
 		// on to the screen
		Pass {
			Name "BASE"
			Tags { "LightMode" = "Always" }

// Target is 4.0 for extra semantic support.
// Additional support will be released later.
Program "vp" {
// Vertex combos: 1
//   d3d11 - ALU: 12 to 12, TEX: 0 to 0, FLOW: 1 to 1
SubProgram "d3d11 " {
Keywords { }
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
Bind "normal" Normal
Bind "color" Color
ConstBuffer "$Globals" 96 // 48 used size, 7 vars
Vector 32 [_MainTex_ST] 4
ConstBuffer "UnityPerCamera" 128 // 76 used size, 8 vars
Vector 64 [_WorldSpaceCameraPos] 3
ConstBuffer "UnityPerDraw" 336 // 336 used size, 6 vars
Matrix 0 [glstate_matrix_mvp] 4
Matrix 256 [_World2Object] 4
Vector 320 [unity_Scale] 4
BindCB "$Globals" 0
BindCB "UnityPerCamera" 1
BindCB "UnityPerDraw" 2
// 17 instructions, 1 temp regs, 0 temp arrays:
// ALU 12 float, 0 int, 0 uint
// TEX 0 (0 load, 0 comp, 0 bias, 0 grad)
// FLOW 1 static, 0 dynamic
"vs_4_0
eefiecedcfcddcidbhjofocfkajkfkkajhbljnccabaaaaaahmaeaaaaadaaaaaa
cmaaaaaalmaaaaaaiaabaaaaejfdeheoiiaaaaaaaeaaaaaaaiaaaaaagiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaahbaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaahkaaaaaaaaaaaaaaaaaaaaaaadaaaaaaacaaaaaa
ahahaaaaibaaaaaaaaaaaaaaaaaaaaaaadaaaaaaadaaaaaaapapaaaafaepfdej
feejepeoaafeeffiedepepfceeaaeoepfcenebemaaedepemepfcaaklepfdeheo
lmaaaaaaagaaaaaaaiaaaaaajiaaaaaaaaaaaaaaabaaaaaaadaaaaaaaaaaaaaa
apaaaaaakeaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaapaaaaaakeaaaaaa
acaaaaaaaaaaaaaaadaaaaaaacaaaaaaadamaaaaknaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaadaaaaaaapaaaaaaknaaaaaaabaaaaaaaaaaaaaaadaaaaaaaeaaaaaa
ahaiaaaaldaaaaaaabaaaaaaaaaaaaaaadaaaaaaafaaaaaaahaiaaaafdfgfpfa
gphdgjhegjgpgoaafeeffiedepepfceeaaedepemepfcaafaepfdejfeejepeoaa
fdeieefcpeacaaaaeaaaabaalnaaaaaafjaaaaaeegiocaaaaaaaaaaaadaaaaaa
fjaaaaaeegiocaaaabaaaaaaafaaaaaafjaaaaaeegiocaaaacaaaaaabfaaaaaa
fpaaaaadpcbabaaaaaaaaaaafpaaaaaddcbabaaaabaaaaaafpaaaaadhcbabaaa
acaaaaaafpaaaaadpcbabaaaadaaaaaaghaaaaaepccabaaaaaaaaaaaabaaaaaa
gfaaaaadpccabaaaabaaaaaagfaaaaaddccabaaaacaaaaaagfaaaaadpccabaaa
adaaaaaagfaaaaadhccabaaaaeaaaaaagfaaaaadhccabaaaafaaaaaagiaaaaac
abaaaaaadiaaaaaipcaabaaaaaaaaaaafgbfbaaaaaaaaaaaegiocaaaacaaaaaa
abaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaacaaaaaaaaaaaaaaagbabaaa
aaaaaaaaegaobaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaacaaaaaa
acaaaaaakgbkbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaa
egiocaaaacaaaaaaadaaaaaapgbpbaaaaaaaaaaaegaobaaaaaaaaaaadgaaaaaf
pccabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaamdcaabaaaaaaaaaaaegaabaaa
aaaaaaaaaceaaaaaaaaaiadpaaaaialpaaaaaaaaaaaaaaaapgapbaaaaaaaaaaa
dgaaaaafmccabaaaabaaaaaakgaobaaaaaaaaaaadiaaaaakdccabaaaabaaaaaa
egaabaaaaaaaaaaaaceaaaaaaaaaaadpaaaaaadpaaaaaaaaaaaaaaaadcaaaaal
dccabaaaacaaaaaaegbabaaaabaaaaaaegiacaaaaaaaaaaaacaaaaaaogikcaaa
aaaaaaaaacaaaaaadgaaaaafpccabaaaadaaaaaaegbobaaaadaaaaaadgaaaaaf
hccabaaaaeaaaaaaegbcbaaaacaaaaaadiaaaaajhcaabaaaaaaaaaaafgifcaaa
abaaaaaaaeaaaaaaegiccaaaacaaaaaabbaaaaaadcaaaaalhcaabaaaaaaaaaaa
egiccaaaacaaaaaabaaaaaaaagiacaaaabaaaaaaaeaaaaaaegacbaaaaaaaaaaa
dcaaaaalhcaabaaaaaaaaaaaegiccaaaacaaaaaabcaaaaaakgikcaaaabaaaaaa
aeaaaaaaegacbaaaaaaaaaaaaaaaaaaihcaabaaaaaaaaaaaegacbaaaaaaaaaaa
egiccaaaacaaaaaabdaaaaaadcaaaaalhccabaaaafaaaaaaegacbaaaaaaaaaaa
pgipcaaaacaaaaaabeaaaaaaegbcbaiaebaaaaaaaaaaaaaadoaaaaab"
}

SubProgram "gles3 " {
Keywords { }
"!!GLES3#version 300 es


#ifdef VERTEX

#define gl_Vertex _glesVertex
in vec4 _glesVertex;
#define gl_Color _glesColor
in vec4 _glesColor;
#define gl_Normal (normalize(_glesNormal))
in vec3 _glesNormal;
#define gl_MultiTexCoord0 _glesMultiTexCoord0
in vec4 _glesMultiTexCoord0;

#line 151
struct v2f_vertex_lit {
    highp vec2 uv;
    lowp vec4 diff;
    lowp vec4 spec;
};
#line 187
struct v2f_img {
    highp vec4 pos;
    mediump vec2 uv;
};
#line 181
struct appdata_img {
    highp vec4 vertex;
    mediump vec2 texcoord;
};
#line 323
struct v2f {
    highp vec4 vertex;
    highp vec4 uvgrab;
    highp vec2 uvmain;
    lowp vec4 color;
    highp vec3 wnormal;
    highp vec3 ViewDir;
};
#line 315
struct appdata_t {
    highp vec4 vertex;
    highp vec2 texcoord;
    highp vec3 normal;
    lowp vec4 color;
};
uniform highp vec4 _Time;
uniform highp vec4 _SinTime;
#line 3
uniform highp vec4 _CosTime;
uniform highp vec4 unity_DeltaTime;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
#line 7
uniform highp vec4 _ScreenParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 unity_CameraWorldClipPlanes[6];
uniform highp vec4 _WorldSpaceLightPos0;
#line 11
uniform highp vec4 _LightPositionRange;
uniform highp vec4 unity_4LightPosX0;
uniform highp vec4 unity_4LightPosY0;
uniform highp vec4 unity_4LightPosZ0;
#line 15
uniform highp vec4 unity_4LightAtten0;
uniform highp vec4 unity_LightColor[8];
uniform highp vec4 unity_LightPosition[8];
uniform highp vec4 unity_LightAtten[8];
#line 19
uniform highp vec4 unity_SpotDirection[8];
uniform highp vec4 unity_SHAr;
uniform highp vec4 unity_SHAg;
uniform highp vec4 unity_SHAb;
#line 23
uniform highp vec4 unity_SHBr;
uniform highp vec4 unity_SHBg;
uniform highp vec4 unity_SHBb;
uniform highp vec4 unity_SHC;
#line 27
uniform highp vec3 unity_LightColor0;
uniform highp vec3 unity_LightColor1;
uniform highp vec3 unity_LightColor2;
uniform highp vec3 unity_LightColor3;
uniform highp vec4 unity_ShadowSplitSpheres[4];
uniform highp vec4 unity_ShadowSplitSqRadii;
uniform highp vec4 unity_LightShadowBias;
#line 31
uniform highp vec4 _LightSplitsNear;
uniform highp vec4 _LightSplitsFar;
uniform highp mat4 unity_World2Shadow[4];
uniform highp vec4 _LightShadowData;
#line 35
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp mat4 glstate_matrix_invtrans_modelview0;
#line 39
uniform highp mat4 _Object2World;
uniform highp mat4 _World2Object;
uniform highp vec4 unity_Scale;
uniform highp mat4 glstate_matrix_transpose_modelview0;
#line 43
uniform highp mat4 glstate_matrix_texture0;
uniform highp mat4 glstate_matrix_texture1;
uniform highp mat4 glstate_matrix_texture2;
uniform highp mat4 glstate_matrix_texture3;
#line 47
uniform highp mat4 glstate_matrix_projection;
uniform highp vec4 glstate_lightmodel_ambient;
uniform highp mat4 unity_MatrixV;
uniform highp mat4 unity_MatrixVP;
#line 51
uniform lowp vec4 unity_ColorSpaceGrey;
#line 77
#line 82
#line 87
#line 91
#line 96
#line 120
#line 137
#line 158
#line 166
#line 193
#line 206
#line 215
#line 220
#line 229
#line 234
#line 243
#line 260
#line 265
#line 291
#line 299
#line 307
#line 311
#line 333
uniform highp float _RefractionRate;
uniform highp vec4 _MainTex_ST;
uniform sampler2D _GrabTexture;
#line 349
uniform highp vec4 _GrabTexture_TexelSize;
uniform sampler2D _MainTex;
uniform sampler2D _RimMap;
uniform highp vec4 _RimColor;
#line 353
uniform highp float _RimPower;
uniform highp float _ColourModifier;
#line 91
highp vec3 ObjSpaceViewDir( in highp vec4 v ) {
    highp vec3 objSpaceCameraPos = ((_World2Object * vec4( _WorldSpaceCameraPos.xyz, 1.0)).xyz * unity_Scale.w);
    return (objSpaceCameraPos - v.xyz);
}
#line 335
v2f vert( in appdata_t v ) {
    #line 337
    v2f o;
    o.vertex = (glstate_matrix_mvp * v.vertex);
    highp float scale = 1.0;
    o.uvgrab.xy = ((vec2( o.vertex.x, (o.vertex.y * scale)) + o.vertex.w) * 0.5);
    #line 341
    o.uvgrab.zw = o.vertex.zw;
    o.uvmain = ((v.texcoord.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
    o.wnormal = v.normal;
    o.color = v.color;
    #line 345
    o.ViewDir = ObjSpaceViewDir( v.vertex);
    return o;
}
out highp vec4 xlv_TEXCOORD0;
out highp vec2 xlv_TEXCOORD2;
out lowp vec4 xlv_COLOR0;
out highp vec3 xlv_COLOR1;
out highp vec3 xlv_POSITION1;
void main() {
    v2f xl_retval;
    appdata_t xlt_v;
    xlt_v.vertex = vec4(gl_Vertex);
    xlt_v.texcoord = vec2(gl_MultiTexCoord0);
    xlt_v.normal = vec3(gl_Normal);
    xlt_v.color = vec4(gl_Color);
    xl_retval = vert( xlt_v);
    gl_Position = vec4(xl_retval.vertex);
    xlv_TEXCOORD0 = vec4(xl_retval.uvgrab);
    xlv_TEXCOORD2 = vec2(xl_retval.uvmain);
    xlv_COLOR0 = vec4(xl_retval.color);
    xlv_COLOR1 = vec3(xl_retval.wnormal);
    xlv_POSITION1 = vec3(xl_retval.ViewDir);
}


#endif
#ifdef FRAGMENT

#define gl_FragData _glesFragData
layout(location = 0) out mediump vec4 _glesFragData[4];
float xll_saturate_f( float x) {
  return clamp( x, 0.0, 1.0);
}
vec2 xll_saturate_vf2( vec2 x) {
  return clamp( x, 0.0, 1.0);
}
vec3 xll_saturate_vf3( vec3 x) {
  return clamp( x, 0.0, 1.0);
}
vec4 xll_saturate_vf4( vec4 x) {
  return clamp( x, 0.0, 1.0);
}
mat2 xll_saturate_mf2x2(mat2 m) {
  return mat2( clamp(m[0], 0.0, 1.0), clamp(m[1], 0.0, 1.0));
}
mat3 xll_saturate_mf3x3(mat3 m) {
  return mat3( clamp(m[0], 0.0, 1.0), clamp(m[1], 0.0, 1.0), clamp(m[2], 0.0, 1.0));
}
mat4 xll_saturate_mf4x4(mat4 m) {
  return mat4( clamp(m[0], 0.0, 1.0), clamp(m[1], 0.0, 1.0), clamp(m[2], 0.0, 1.0), clamp(m[3], 0.0, 1.0));
}
#line 151
struct v2f_vertex_lit {
    highp vec2 uv;
    lowp vec4 diff;
    lowp vec4 spec;
};
#line 187
struct v2f_img {
    highp vec4 pos;
    mediump vec2 uv;
};
#line 181
struct appdata_img {
    highp vec4 vertex;
    mediump vec2 texcoord;
};
#line 323
struct v2f {
    highp vec4 vertex;
    highp vec4 uvgrab;
    highp vec2 uvmain;
    lowp vec4 color;
    highp vec3 wnormal;
    highp vec3 ViewDir;
};
#line 315
struct appdata_t {
    highp vec4 vertex;
    highp vec2 texcoord;
    highp vec3 normal;
    lowp vec4 color;
};
uniform highp vec4 _Time;
uniform highp vec4 _SinTime;
#line 3
uniform highp vec4 _CosTime;
uniform highp vec4 unity_DeltaTime;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
#line 7
uniform highp vec4 _ScreenParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 unity_CameraWorldClipPlanes[6];
uniform highp vec4 _WorldSpaceLightPos0;
#line 11
uniform highp vec4 _LightPositionRange;
uniform highp vec4 unity_4LightPosX0;
uniform highp vec4 unity_4LightPosY0;
uniform highp vec4 unity_4LightPosZ0;
#line 15
uniform highp vec4 unity_4LightAtten0;
uniform highp vec4 unity_LightColor[8];
uniform highp vec4 unity_LightPosition[8];
uniform highp vec4 unity_LightAtten[8];
#line 19
uniform highp vec4 unity_SpotDirection[8];
uniform highp vec4 unity_SHAr;
uniform highp vec4 unity_SHAg;
uniform highp vec4 unity_SHAb;
#line 23
uniform highp vec4 unity_SHBr;
uniform highp vec4 unity_SHBg;
uniform highp vec4 unity_SHBb;
uniform highp vec4 unity_SHC;
#line 27
uniform highp vec3 unity_LightColor0;
uniform highp vec3 unity_LightColor1;
uniform highp vec3 unity_LightColor2;
uniform highp vec3 unity_LightColor3;
uniform highp vec4 unity_ShadowSplitSpheres[4];
uniform highp vec4 unity_ShadowSplitSqRadii;
uniform highp vec4 unity_LightShadowBias;
#line 31
uniform highp vec4 _LightSplitsNear;
uniform highp vec4 _LightSplitsFar;
uniform highp mat4 unity_World2Shadow[4];
uniform highp vec4 _LightShadowData;
#line 35
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp mat4 glstate_matrix_invtrans_modelview0;
#line 39
uniform highp mat4 _Object2World;
uniform highp mat4 _World2Object;
uniform highp vec4 unity_Scale;
uniform highp mat4 glstate_matrix_transpose_modelview0;
#line 43
uniform highp mat4 glstate_matrix_texture0;
uniform highp mat4 glstate_matrix_texture1;
uniform highp mat4 glstate_matrix_texture2;
uniform highp mat4 glstate_matrix_texture3;
#line 47
uniform highp mat4 glstate_matrix_projection;
uniform highp vec4 glstate_lightmodel_ambient;
uniform highp mat4 unity_MatrixV;
uniform highp mat4 unity_MatrixVP;
#line 51
uniform lowp vec4 unity_ColorSpaceGrey;
#line 77
#line 82
#line 87
#line 91
#line 96
#line 120
#line 137
#line 158
#line 166
#line 193
#line 206
#line 215
#line 220
#line 229
#line 234
#line 243
#line 260
#line 265
#line 291
#line 299
#line 307
#line 311
#line 333
uniform highp float _RefractionRate;
uniform highp vec4 _MainTex_ST;
uniform sampler2D _GrabTexture;
#line 349
uniform highp vec4 _GrabTexture_TexelSize;
uniform sampler2D _MainTex;
uniform sampler2D _RimMap;
uniform highp vec4 _RimColor;
#line 353
uniform highp float _RimPower;
uniform highp float _ColourModifier;
#line 355
mediump vec4 frag( in v2f i ) {
    #line 357
    mediump vec2 bump = i.wnormal.xy;
    highp vec2 offset = ((bump * _RefractionRate) * _GrabTexture_TexelSize.xy);
    i.uvgrab.xy = ((offset * i.uvgrab.z) + i.uvgrab.xy);
    mediump float rim = (1.0 - xll_saturate_f(dot( normalize(i.ViewDir), i.wnormal)));
    #line 361
    mediump vec4 rimCol = (((_RimColor * pow( rim, _RimPower)) * texture( _RimMap, i.uvmain)) * smoothstep( 0.25, 1.0, rim));
    mediump vec4 col = textureProj( _GrabTexture, i.uvgrab);
    mediump vec4 tint = texture( _MainTex, i.uvmain);
    return ((col * tint) + (rimCol + (i.color * _ColourModifier)));
}
in highp vec4 xlv_TEXCOORD0;
in highp vec2 xlv_TEXCOORD2;
in lowp vec4 xlv_COLOR0;
in highp vec3 xlv_COLOR1;
in highp vec3 xlv_POSITION1;
void main() {
    mediump vec4 xl_retval;
    v2f xlt_i;
    xlt_i.vertex = vec4(0.0);
    xlt_i.uvgrab = vec4(xlv_TEXCOORD0);
    xlt_i.uvmain = vec2(xlv_TEXCOORD2);
    xlt_i.color = vec4(xlv_COLOR0);
    xlt_i.wnormal = vec3(xlv_COLOR1);
    xlt_i.ViewDir = vec3(xlv_POSITION1);
    xl_retval = frag( xlt_i);
    gl_FragData[0] = vec4(xl_retval);
}


#endif"
}

}
Program "fp" {
// Fragment combos: 1
//   d3d11 - ALU: 22 to 22, TEX: 3 to 3, FLOW: 1 to 1
SubProgram "d3d11 " {
Keywords { }
ConstBuffer "$Globals" 96 // 88 used size, 7 vars
Float 16 [_RefractionRate]
Vector 48 [_GrabTexture_TexelSize] 4
Vector 64 [_RimColor] 4
Float 80 [_RimPower]
Float 84 [_ColourModifier]
BindCB "$Globals" 0
SetTexture 0 [_RimMap] 2D 2
SetTexture 1 [_GrabTexture] 2D 0
SetTexture 2 [_MainTex] 2D 1
// 26 instructions, 3 temp regs, 0 temp arrays:
// ALU 22 float, 0 int, 0 uint
// TEX 3 (0 load, 0 comp, 0 bias, 0 grad)
// FLOW 1 static, 0 dynamic
"ps_4_0
eefiecedceanghbjhcilbinblmjngnfeiohoolkbabaaaaaaomaeaaaaadaaaaaa
cmaaaaaapaaaaaaaceabaaaaejfdeheolmaaaaaaagaaaaaaaiaaaaaajiaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaakeaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaapapaaaakeaaaaaaacaaaaaaaaaaaaaaadaaaaaaacaaaaaa
adadaaaaknaaaaaaaaaaaaaaaaaaaaaaadaaaaaaadaaaaaaapapaaaaknaaaaaa
abaaaaaaaaaaaaaaadaaaaaaaeaaaaaaahahaaaaldaaaaaaabaaaaaaaaaaaaaa
adaaaaaaafaaaaaaahahaaaafdfgfpfagphdgjhegjgpgoaafeeffiedepepfcee
aaedepemepfcaafaepfdejfeejepeoaaepfdeheocmaaaaaaabaaaaaaaiaaaaaa
caaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapaaaaaafdfgfpfegbhcghgf
heaaklklfdeieefcmaadaaaaeaaaaaaapaaaaaaafjaaaaaeegiocaaaaaaaaaaa
agaaaaaafkaaaaadaagabaaaaaaaaaaafkaaaaadaagabaaaabaaaaaafkaaaaad
aagabaaaacaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaafibiaaaeaahabaaa
abaaaaaaffffaaaafibiaaaeaahabaaaacaaaaaaffffaaaagcbaaaadpcbabaaa
abaaaaaagcbaaaaddcbabaaaacaaaaaagcbaaaadpcbabaaaadaaaaaagcbaaaad
hcbabaaaaeaaaaaagcbaaaadhcbabaaaafaaaaaagfaaaaadpccabaaaaaaaaaaa
giaaaaacadaaaaaabaaaaaahbcaabaaaaaaaaaaaegbcbaaaafaaaaaaegbcbaaa
afaaaaaaeeaaaaafbcaabaaaaaaaaaaaakaabaaaaaaaaaaadiaaaaahhcaabaaa
aaaaaaaaagaabaaaaaaaaaaaegbcbaaaafaaaaaabacaaaahbcaabaaaaaaaaaaa
egacbaaaaaaaaaaaegbcbaaaaeaaaaaaaaaaaaaldcaabaaaaaaaaaaaagaabaia
ebaaaaaaaaaaaaaaaceaaaaaaaaaiadpaaaaeadpaaaaaaaaaaaaaaaacpaaaaaf
bcaabaaaaaaaaaaaakaabaaaaaaaaaaadiaaaaahccaabaaaaaaaaaaabkaabaaa
aaaaaaaaabeaaaaaklkkkkdpdeaaaaahccaabaaaaaaaaaaabkaabaaaaaaaaaaa
abeaaaaaaaaaaaaadiaaaaaibcaabaaaaaaaaaaaakaabaaaaaaaaaaaakiacaaa
aaaaaaaaafaaaaaabjaaaaafbcaabaaaaaaaaaaaakaabaaaaaaaaaaadiaaaaai
pcaabaaaabaaaaaaagaabaaaaaaaaaaaegiocaaaaaaaaaaaaeaaaaaaefaaaaaj
pcaabaaaacaaaaaaegbabaaaacaaaaaaeghobaaaaaaaaaaaaagabaaaacaaaaaa
diaaaaahpcaabaaaabaaaaaaegaobaaaabaaaaaaegaobaaaacaaaaaadcaaaaaj
bcaabaaaaaaaaaaabkaabaaaaaaaaaaaabeaaaaaaaaaaamaabeaaaaaaaaaeaea
diaaaaahccaabaaaaaaaaaaabkaabaaaaaaaaaaabkaabaaaaaaaaaaadiaaaaah
bcaabaaaaaaaaaaabkaabaaaaaaaaaaaakaabaaaaaaaaaaadiaaaaaipcaabaaa
acaaaaaaegbobaaaadaaaaaafgifcaaaaaaaaaaaafaaaaaadcaaaaajpcaabaaa
aaaaaaaaegaobaaaabaaaaaaagaabaaaaaaaaaaaegaobaaaacaaaaaadiaaaaai
dcaabaaaabaaaaaaegbabaaaaeaaaaaaagiacaaaaaaaaaaaabaaaaaadiaaaaai
dcaabaaaabaaaaaaegaabaaaabaaaaaaegiacaaaaaaaaaaaadaaaaaadcaaaaaj
dcaabaaaabaaaaaaegaabaaaabaaaaaakgbkbaaaabaaaaaaegbabaaaabaaaaaa
aoaaaaahdcaabaaaabaaaaaaegaabaaaabaaaaaapgbpbaaaabaaaaaaefaaaaaj
pcaabaaaabaaaaaaegaabaaaabaaaaaaeghobaaaabaaaaaaaagabaaaaaaaaaaa
efaaaaajpcaabaaaacaaaaaaegbabaaaacaaaaaaeghobaaaacaaaaaaaagabaaa
abaaaaaadcaaaaajpccabaaaaaaaaaaaegaobaaaabaaaaaaegaobaaaacaaaaaa
egaobaaaaaaaaaaadoaaaaab"
}

SubProgram "gles3 " {
Keywords { }
"!!GLES3"
}

}

#LINE 117

		}
	}

	// ------------------------------------------------------------------
	// Fallback for older cards and Unity non-Pro
	
	FallBack "Shield FX/Basic"
}

}
