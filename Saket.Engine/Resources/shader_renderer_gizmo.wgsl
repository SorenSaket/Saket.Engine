struct VertexInput {
	@location(0) position: vec2<f32>,
	@location(1) uv: vec2<f32>,
	@location(2) color: vec4<f32>,
};

struct VertexOutput {
	// Fragment position
	@builtin(position) position: vec4<f32>,
	// world position of fragment
	@location(0) world_position : vec2<f32>,
	@location(1) color: vec4<f32>,
	@location(2) uv: vec2<f32>,
};

struct Uniforms {
	mvp: mat4x4<f32>,
};

// ---- System ----
@group(0) @binding(0) var<uniform> uniforms: Uniforms;

@vertex
fn vert(in: VertexInput) -> VertexOutput {
	var out: VertexOutput;
	out.position = uniforms.mvp * vec4<f32>(in.position, 0.0, 1.0);
	out.world_position = in.position;
	out.color = in.color;
	out.uv = in.uv;
	return out;
}

@fragment
fn frag(in: VertexOutput) -> @location(0) vec4<f32> {
	return in.color; 
}
