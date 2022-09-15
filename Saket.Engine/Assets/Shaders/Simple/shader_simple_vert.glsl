#version 450

layout(location = 0) in vec2 quad;

void main()
{
    gl_Position = vec4(quad, 0, 1);
}