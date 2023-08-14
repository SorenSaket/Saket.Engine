struct SystemUniform {
	projectionMatrix: mat4x4<f32>,
	viewMatrix: mat4x4<f32>,
	frame : u32,
};


@group(0) binding(0) var<uniform> uniforms: SystemUniform;


@group(1) @binding(0) var<uniform> tiles: array<vec4>;
@group(1) @binding(1) var myTexture: texture_2d<f32>;
@group(1) @binding(2) var mySampler: sampler;

//
struct VertexInput {
	@location(0) position: vec3<f32>,
	@location(1) rotation: f32,
	@location(2) size: vec2<f32>,
	@location(3) box: i32,
	@location(4) color: vec4<f32>,
	@builtin(vertex_index) vertex_index : u32
};

struct FragmentInput {
	@builtin(position) position : vec4<f32>,
	@location(0) uv : vec2<f32>,
	@location(1) tint: vec4<f32>,
}

// Counter ClockWise (CCW) winding order
// TL---TR
// | \  |
// |  \ |
// BL---BR
const QUAD_VERTICES = array(
  	vec2(-0.5, -0.5),// BL
  	vec2(0.5, -0.5),// BR
	vec2(-0.5., 0.5),// TL
	vec2(-0.5., 0.5),// TL
	vec2(0.5, 0.5),// BR
	vec2(0.5, 0.5),// TR
);
const QUAD_UVS = array(
  	vec2(0., 0.),
	vec2(1., 0.),
	vec2(0., 1.),
	vec2(0., 1.),
	vec2(1., 0.),
	vec2(1., 1.),
);

@vertex
fn vtx_main(in: VertexInput) -> FragmentInput
{
	var output : FragmentInput;
	

	vec2 vert = QUAD_VERTICES[in.vertex_index];
	vec2 rotatedVert = vec2(
       	vert.x * cos(in.rotation) - vert.y * sin(in.rotation),
        vert.x * sin(in.rotation) + vert.y * cos(in.rotation));
	// 
    output.position = uniforms.projectionMatrix * uniforms.viewMatrix  * vec4(in.position.xy + rotatedVert * size, in.position.z, 1);
	//
	output.uv = (QUAD_UVS[in.vertex_index] * tiles[in.box].zw) + tiles[in.box].xy
	// Pass color tint directly
	output.tint = in.color;

	return output;
}


@fragment
fn frag_main( in: FragmentInput) -> @location(0) vec4f {
	return textureSample(myTexture, mySampler, fragUV) * fragPosition;
}