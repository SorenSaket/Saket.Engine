



## WebGPU
All graphics are based on the webgpu apis. The goal is to create a useful abstractionlayer without hindering direct use of webgpu.

being able to use the api without unsafe

Learn WebGPU: https://sotrh.github.io/learn-wgpu



## Shaders

WebGPU supports consumation of both SPIR-V and wgsl. 
wgsl is another propriatary shader language.
The spec can be found here: 
https://gpuweb.github.io/gpuweb/wgsl 
https://www.w3.org/TR/WGSL/


```wgsl
@binding(0) @group(0) var<uniform> frame : u32;


@vertex
fn vtx_main(@builtin(vertex_index) vertex_index : u32) -> @builtin(position) vec4f {
  const pos = array(
    vec2( 0.0,  0.5),
    vec2(-0.5, -0.5),
    vec2( 0.5, -0.5)
  );

  return vec4f(pos[vertex_index], 0, 1);
}

@fragment
fn frag_main() -> @location(0) vec4f {
	return vec4(1, sin(f32(frame) / 128), 0, 1);
}
```








## Materials


Shader graph


Common Shape Abstraction layer for use with Fonts/Vector graphcis

Bézier 
https://www.youtube.com/watch?v=aVwxzDHniEw&t=1253s&ab_channel=FreyaHolm%C3%A9r
https://www.shadertoy.com/view/4sKyzW
https://www.shadertoy.com/view/ltXSDB
https://www.shadertoy.com/view/MlKcDD
https://www.pouet.net/topic.php?which=9119&page=2
https://www.reddit.com/r/gamedev/comments/ruyigx/can_someone_explain_how_a_signed_distance_field/
https://blog.gludion.com/2009/08/distance-to-quadratic-bezier-curve.html