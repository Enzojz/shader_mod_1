#version 150

uniform mat4 projView;

in vec4 attrPosAmbient;
in vec3 attrNormal;
in vec2 attrTexCoord;

out vec4 posAmbient;
centroid out vec3 normal;
out vec2 texCoord;

void calcFogFactor(vec3 pos);

void main() {
	gl_Position = projView * vec4(attrPosAmbient.xyz, 1.0);
	posAmbient = attrPosAmbient;
	normal = attrNormal;
	texCoord = attrTexCoord;

	calcFogFactor(attrPosAmbient.xyz);
}
