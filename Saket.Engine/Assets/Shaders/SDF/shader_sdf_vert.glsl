#version 450

uniform mat4 view;
uniform mat4 projection;
uniform float time;

// Generic data
layout(location = 0) in vec2 quad;
layout(location = 1) in vec2 UV;


layout(std430, binding = 0) buffer boxBuffer
{
	vec4 boxes[];
};

// Instance Data
layout(location = 2) in vec3 pos;
layout(location = 3) in float rotation;
layout(location = 4) in vec2 size;
layout(location = 5) in int	box;
layout(location = 6) in vec4 color;

// Output
layout(location = 0) out vec2 fsin_TexCoords;
layout(location = 1) out vec4 fsin_Tint;
layout(location = 2) out float fsin_Time;

void main()
{
    vec2 srcQuad = vec2(
        quad.x * cos(rotation) - quad.y * sin(rotation),
        quad.x * sin(rotation) + quad.y * cos(rotation));

    gl_Position = projection * view * vec4(pos.xy + srcQuad * size, pos.z, 1);
	fsin_Tint = color;
	fsin_TexCoords = (UV * boxes[box].zw) + boxes[box].xy;
    fsin_Time = time;
}