#version 150

uniform mat4 projView;

in vec4 attrPosition;
in vec3 attrNormal;
in float attrTexCoord;

out vec4 posAmbient;
out vec3 normal;
out vec2 texCoord;
out vec2 texCoord2;

void calcFogFactor(vec3 pos);

void main() {
	gl_Position = projView * attrPosition;
	posAmbient = vec4(attrPosition.xyz, 1.0);		// TODO ambient?
	normal = attrNormal;
	texCoord = vec2(attrTexCoord / 8192.0, attrPosition.z > .0 ? 1.0 : .0);
	texCoord2 = vec2(attrTexCoord / 500.0, attrPosition.z / 500.0);			// TODO uniform

	calcFogFactor(posAmbient.xyz);
}
