#version 150

uniform mat4 projView;
uniform mat4 viewInverse;

in vec4 attrPosition;
in vec3 attrNormal;
in vec2 attrTexCoord;
in vec4 attrInfluence;
in vec4 attrTangent;
//in mat4 instAttrModel;		// instAttrModel[3][3] = ambient
in vec3 instAttrCblendColors[4];

out vec4 posAmbient;
out vec3 normal;
out vec3 tangent;
out vec3 binormal;
out vec3 texCoordAlpha;
out vec3 cblendColors[4];

void calcFogFactor(vec3 pos);
mat4 calcSkinMatrix(vec4 infl);

void main() {
	mat4 skinMat = calcSkinMatrix(attrInfluence);
	vec4 skinnedPos = skinMat * attrPosition;
	vec4 skinnedNrml = skinMat * vec4(attrNormal, .0);
	vec4 skinnedTangent = skinMat * vec4(vec3(attrTangent), .0);

	posAmbient = /*instAttrModel **/ skinnedPos;
	gl_Position = projView * vec4(posAmbient.xyz, 1.0);
	normal = normalize(vec3(/*instAttrModel **/ skinnedNrml));
	tangent = normalize(vec3(/*instAttrModel **/ skinnedTangent));
	binormal = cross(normal, tangent) * attrTangent.w;
	texCoordAlpha = vec3(attrTexCoord, 1.0);

	calcFogFactor(posAmbient.xyz);

	cblendColors = instAttrCblendColors;
}
