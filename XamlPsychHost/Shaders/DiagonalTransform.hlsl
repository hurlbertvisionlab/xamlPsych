// shaders need to be compiled separately, using DirectX SDK or WDK, e.g.
// "C:\Program Files (x86)\Windows Kits\10\bin\10.0.19041.0\x64\fxc.exe" /T ps_2_0 /E main /FoDiagonalTransform.ps DiagonalTransform.hlsl

sampler2D implicitInput : register(s0);
float4 rgba : register(c0);

float4 main(float2 uv : TEXCOORD) : COLOR
{
	float4 color = tex2D(implicitInput, uv);
	float4 transform = { rgba.r, rgba.g, rgba.b, 1.0 };

	return color * transform;
}