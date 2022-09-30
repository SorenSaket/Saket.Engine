#version 450

uniform sampler2D texture0;

layout(location = 0) in vec2 fsin_TexCoords;
layout(location = 1) in vec4 fsin_Tint;

layout(location = 0) out vec4 outputColor;

void main()
{
    outputColor = texture(texture0, fsin_TexCoords) * fsin_Tint;
}