#version 450

layout(location = 0) in vec2 Position;
out vec4 vertexColor;
void main()
{
    gl_Position = vec4(Position, 0, 1);
    vertexColor = vec4(0.5, 0.0, 0.0, 1.0);
}