#version 150

uniform mat4 projView;
uniform mat4 viewInverse;

in vec4 attrPosition;
in vec4 attrInfluence;

//in mat4 instAttrModel;		// instAttrModel[3][3] = ambient
in vec4 instAttrColor;

out vec4 color;

void calcFogFactor(vec3 pos);
mat4 calcSkinMatrix(vec4 infl);

void main() {
	mat4 skinMat = calcSkinMatrix(attrInfluence);
	vec4 skinnedPos = skinMat * attrPosition;

	vec4 posAmbient = /*instAttrModel **/ skinnedPos;
	gl_Position = projView * vec4(posAmbient.xyz, 1.0);

	calcFogFactor(posAmbient.xyz);

	color = instAttrColor;
}
