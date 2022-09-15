#version 450

uniform mat4 view;
uniform mat4 projection;

// Generic data
layout(location = 0) in vec2 quad;
layout(location = 1) in vec2 UV;

// Instance Data
layout(location = 2) in vec2 pos;
layout(location = 3) in float rotation;
layout(location = 4) in vec2 size;
layout(location = 5) in vec4 color;

//layout(location = 0) out vec2 fsin_TexCoords;
layout(location = 0) out vec4 fsin_Tint;

vec2 rotate(vec2 v, float a)
{
    float s = sin(a);
    float c = cos(a);
    mat2 m = mat2(c, -s, s, c);
    return m * v;
}

void main()
{
    vec2 srcQuad = vec2(
        quad.x * cos(rotation) - (quad.y)*sin(rotation),
        quad.x*  sin(rotation) + quad.y* cos(rotation));


    gl_Position = projection * view * vec4(pos + srcQuad * size, 0, 1);

    //fsin_TexCoords = UV;
    fsin_Tint = color;
}