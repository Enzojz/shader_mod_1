#version 150

uniform mat4 projView;

in vec4 attrPosAmbient;

out vec4 posAmbient;

void calcFogFactor(vec3 pos);

void main() {
	gl_Position = projView * vec4(attrPosAmbient.xyz, 1.0);

	posAmbient = attrPosAmbient;

	calcFogFactor(attrPosAmbient.xyz);
}
