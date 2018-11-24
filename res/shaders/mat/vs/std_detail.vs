#version 150

uniform mat4 projView;
uniform mat4 viewInverse;

in vec4 attrPosAmbient;
in vec3 attrNormal;
in vec2 attrTexCoord;
in vec2 attrTexCoordDetail;

in mat4 instAttrModel;		// instAttrModel[3][3] = ambient

out vec4 posAmbient;
centroid out vec3 normal;
out vec3 texCoordAlpha;
out vec2 texCoordDetail;

float compFadeOutAlpha(vec3 vpos, vec3 campos);
void calcFogFactor(vec3 pos);

void main() {
	posAmbient = instAttrModel * attrPosAmbient;
	
	float alpha = compFadeOutAlpha(posAmbient.xyz, vec3(viewInverse[3]));

	gl_Position = projView * vec4(posAmbient.xyz, 1.0);

	normal = normalize(vec3(instAttrModel * vec4(attrNormal, .0)));
	texCoordAlpha = vec3(attrTexCoord, alpha);

	texCoordDetail = attrTexCoordDetail;

	calcFogFactor(posAmbient.xyz);
}