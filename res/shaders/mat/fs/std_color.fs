#version 150

in vec4 color;

out vec4 color0;

vec3 applyFog(vec3 color);

void main() {
	color0 = vec4(applyFog(color.rgb), color.a);
}
