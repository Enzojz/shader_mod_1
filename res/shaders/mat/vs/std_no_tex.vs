#version 150

uniform mat4 projView;
uniform mat4 viewInverse;

in vec4 attrPosAmbient;
in vec3 attrNormal;

out vec4 posAmbient;
out vec3 vnormal;

void calcFogFactor(vec3 pos);

void main() {
	vec4 pos = vec4(vec3(attrPosAmbient), 1.0);

	gl_Position = projView * pos;

	calcFogFactor(vec3(attrPosAmbient));

	vnormal = attrNormal;
	posAmbient = vec4(pos.xyz, attrPosAmbient.w);	
}
