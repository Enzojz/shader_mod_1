#version 150

in vec4 normalDepth;

out vec4 color;

void main() {
	color = vec4(vec3(.5) + .5 * normalDepth.xyz * (gl_FrontFacing ? 1.0 : -1.0), sqrt(normalDepth.w));
}
