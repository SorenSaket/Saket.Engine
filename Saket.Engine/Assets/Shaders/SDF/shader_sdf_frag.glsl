#version 450

uniform sampler2D sdf;

layout(location = 0) in vec2 fsin_TexCoords;
layout(location = 1) in vec4 fsin_Tint;

layout(location = 0) out vec4 outputColor;

void main() {
	// Bilinear sampling of the distance field
	float d = texture2D(sdf, fsin_TexCoords).r;

	float smoothing = 0.005;

	outputColor = smoothstep(vec4(0.5-smoothing),vec4(0.5+smoothing),vec4(d));
}