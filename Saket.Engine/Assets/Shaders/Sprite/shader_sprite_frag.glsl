#version 450

//layout(location = 0) in vec2 fsin_TexCoords;
layout(location = 0) in vec4 fsin_Tint;

layout(location = 0) out vec4 outputColor;

void main()
{
    outputColor = fsin_Tint;
}