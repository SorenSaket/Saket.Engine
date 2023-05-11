#version 450

uniform sampler2D sdf;

layout(location = 0) in vec2 fsin_TexCoords;
layout(location = 1) in vec4 fsin_Tint;

layout(location = 0) out vec4 outputColor;
// Computation of the median value using minima and maxima
float median(float a, float b, float c) {
	return max(min(a, b), min(max(a, b), c));
}

void main() {
	// Bilinear sampling of the distance field
	vec3 s = texture2D(sdf, fsin_TexCoords).rgb;
	// Acquiring the signed distance
	float d = median(s.r, s.g, s.b) - 0.5;
	// The anti-aliased measure of how "inside" the fragment lies
	float w = clamp(d / fwidth(d) + 0.5, 0.0, 1.0);
	// Combining the two colors
	outputColor = mix(vec4(0, 0, 0, 0), fsin_Tint, w);
}