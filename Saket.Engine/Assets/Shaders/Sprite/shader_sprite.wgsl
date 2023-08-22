@group(0) @binding(0) var<uniform> uniforms: SystemUniform;

@group(1) @binding(0) var<storage, read> tiles: array<vec4<f32>>;
@group(1) @binding(1) var myTexture: texture_2d<f32>;
@group(1) @binding(2) var mySampler: sampler;

struct SystemUniform {
	projectionMatrix: mat4x4<f32>,
	viewMatrix: mat4x4<f32>,
	frame: u32,
};
//
struct VertexInput {
	@location(0) position: vec3<f32>,
	@location(1) rotation: f32,
	@location(2) size: vec2<f32>,
	@location(3) box: i32,
	@location(4) color: vec4<f32>,
	@builtin(vertex_index) vertex_index: u32
};

struct FragmentInput {
	@builtin(position) position: vec4<f32>,
	@location(0) uv: vec2<f32>,
	@location(1) tint: vec4<f32>,
}

// Counter ClockWise (CCW) winding order
// TL---TR
// | \  |
// |  \ |
// BL---BR
const QUAD_VERTICES: array<vec2<f32>, 6> = array<vec2<f32>, 6>(
		vec2(-0.5, -0.5),// BL
		vec2(0.5, -0.5),// BR
		vec2(-0.5, 0.5),// TL
		vec2(-0.5, 0.5),// TL
		vec2(0.5, -0.5), // BR
		vec2(0.5, 0.5), // TR
	);

const QUAD_UVS:  array<vec2<f32>, 6> = array<vec2<f32>, 6>(
	vec2(0., 0.),
	vec2(1., 0.),
	vec2(0., 1.),
	vec2(0., 1.),
	vec2(1., 0.),
	vec2(1., 1.),
);


@vertex
fn vtx_main(in: VertexInput) -> FragmentInput {
	
    var output: FragmentInput;

	
    var selected_vert : vec2<f32> = QUAD_VERTICES[0];
	var selected_uv : vec2<f32> = QUAD_UVS[0];
	switch in.vertex_index{
		case 1u: {
			selected_vert = QUAD_VERTICES[1];
			selected_uv = QUAD_UVS[1];
		}
		case 2u: {
			selected_vert = QUAD_VERTICES[2];
			selected_uv = QUAD_UVS[2];
		}
		case 3u: {
			selected_vert = QUAD_VERTICES[3];
			selected_uv = QUAD_UVS[3];
		}
		case 4u: {
			selected_vert = QUAD_VERTICES[4];
			selected_uv = QUAD_UVS[4];
		}
		case 5u: {
			selected_vert = QUAD_VERTICES[5];
			selected_uv = QUAD_UVS[5];
		} default: {
		}
	}

    var rotatedVert : vec2<f32> = vec2(
        selected_vert.x * cos(in.rotation) - selected_vert.y * sin(in.rotation),
        selected_vert.x * sin(in.rotation) + selected_vert.y * cos(in.rotation)
    );
	// 
    output.position = vec4<f32>(in.position.xy + rotatedVert * in.size, in.position.z, 1.0) * uniforms.projectionMatrix * uniforms.viewMatrix;
	//
    output.uv = (selected_uv * tiles[in.box].zw) + tiles[in.box].xy;
	// Pass color tint directly
    output.tint = in.color;

    return output;
}

@fragment
fn frag_main(in: FragmentInput) -> @location(0) vec4f {
    return textureSample(myTexture, mySampler, in.uv) * in.position;
}

	//switch in.vertex_index{
	//	case 0u: {
	//		selected_vert = QUAD_VERTICES[0];
	//	}
	//}
	