// Subset of PostProcessing's stdlib.hlsl

#ifndef UNITY_CMWAVEFORM_STDLIB
#define UNITY_CMWAVEFORM_STDLIB

const float _RenderViewportScaleFactor = 1;

#define FLT_EPSILON     1.192092896e-07 // Smallest positive number, such that 1.0 + FLT_EPSILON != 1.0

float3 PositivePow(float3 base, float3 power)
{
	return pow(max(abs(base), float3(FLT_EPSILON, FLT_EPSILON, FLT_EPSILON)), power);
}

#if defined(UNITY_SINGLE_PASS_STEREO)
float2 TransformStereoScreenSpaceTex(float2 uv, float w)
{
	float4 scaleOffset = unity_StereoScaleOffset[unity_StereoEyeIndex];
	scaleOffset.xy *= _RenderViewportScaleFactor;
	return uv.xy * scaleOffset.xy + scaleOffset.zw * w;
}
#else
float2 TransformStereoScreenSpaceTex(float2 uv, float w)
{
	return uv * _RenderViewportScaleFactor;
}
#endif

// Vertex manipulation
float2 TransformTriangleVertexToUV(float2 vertex)
{
	float2 uv = (vertex + 1.0) * 0.5;
	return uv;
}

struct AttributesDefault
{
	float3 vertex : POSITION;
};

struct VaryingsDefault
{
	float4 vertex : SV_POSITION;
	float2 texcoord : TEXCOORD0;
	float2 texcoordStereo : TEXCOORD1;
};

VaryingsDefault VertDefault(AttributesDefault v)
{
	VaryingsDefault o;
	o.vertex = float4(v.vertex.xy, 0.0, 1.0);
	o.texcoord = TransformTriangleVertexToUV(v.vertex.xy);

#if UNITY_UV_STARTS_AT_TOP
	o.texcoord = o.texcoord * float2(1.0, -1.0) + float2(0.0, 1.0);
#endif

	o.texcoordStereo = TransformStereoScreenSpaceTex(o.texcoord, 1.0);

	return o;
}

#endif // UNITY_CMWAVEFORM_STDLIB
