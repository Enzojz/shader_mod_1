#version 150

uniform mat4 proj;
uniform mat4 view;

uniform vec3 camPos;

in vec4 attrPosAmbient;
in vec3 attrOffset;
in vec2 attrTexCoord;

in mat4 instAttrModel;

out float depth;
out vec3 texCoordAlpha;
flat out int matIndex;

float compFadeOutAlpha(vec3 vpos, vec3 campos);

void main() {
	vec4 centerWorld = (instAttrModel * vec4(attrPosAmbient.xyz, 1.0));
	vec4 posEye = view * centerWorld + vec4(attrOffset, .0);
	
	posEye.z -= length(attrOffset);
	
	gl_Position = proj * posEye;

	depth = gl_Position.z;
	
	texCoordAlpha = vec3(attrTexCoord, compFadeOutAlpha(centerWorld.xyz, camPos));

	matIndex = 0;
}
