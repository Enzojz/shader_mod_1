#version 150

uniform sampler2D tex;

in vec3 l0;
in float alpha;

out vec4 color;

vec3 applyFog(vec3 color);
vec3 toneMap(vec3 color);

void main() {
	color = vec4(toneMap(applyFog(l0)), texture(tex, gl_PointCoord).r * alpha);		
}
