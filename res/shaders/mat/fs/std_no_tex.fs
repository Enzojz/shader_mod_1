#version 150

uniform vec4 matCoeff;
uniform vec3 color;

in vec4 posAmbient;
in vec3 vnormal;

out vec4 color0;

vec3 lightSsao(vec4 posAmbient, vec3 normal, vec4 matCoeff, vec3 color);
vec3 applyFog(vec3 color);
vec3 toneMap(vec3 color);

void main() {
	color0.rgb = toneMap(applyFog(lightSsao(posAmbient, vnormal, matCoeff, color)));
}
