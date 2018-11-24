#version 150

uniform mat4 projView;
uniform mat4 viewInverse;

in vec4 attrPosition;
in vec3 attrNormal;
in vec2 attrTexCoord;
in vec4 attrInfluence;
//in mat4 instAttrModel;		// instAttrModel[3][3] = ambient

out vec4 posAmbient;
centroid out vec3 normal;
out vec2 texCoord;

void calcFogFactor(vec3 pos);
mat4 calcSkinMatrix(vec4 infl);

void main() {
	mat4 skinMat = calcSkinMatrix(attrInfluence);
	vec4 skinnedPos = skinMat * attrPosition;
	vec4 skinnedNrml = skinMat * vec4(attrNormal, .0);

	posAmbient = /*instAttrModel **/ skinnedPos;
	gl_Position = projView * vec4(posAmbient.xyz, 1.0);
	normal = normalize(vec3(/*instAttrModel **/ skinnedNrml));
	texCoord = attrTexCoord;

	calcFogFactor(posAmbient.xyz);
}