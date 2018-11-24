#version 150

uniform mat4 projView;

uniform vec3 camPos;

in vec4 attrPosition;
in vec2 attrTexCoord;
in mat4 instAttrModel;		// instAttrModel[3][3] = ambient

out vec3 texCoordAlpha;
out float depth;
flat out int matIndex;

float compFadeOutAlpha(vec3 vpos, vec3 campos);

void main() {
	vec4 posAmbient = instAttrModel * attrPosition;

	gl_Position = projView * vec4(posAmbient.xyz, 1.0);

	texCoordAlpha = vec3(attrTexCoord, compFadeOutAlpha(posAmbient.xyz, camPos));
	depth = gl_Position.z;
	matIndex = 0;
}
