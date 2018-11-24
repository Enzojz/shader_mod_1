#version 150

uniform mat4 proj;
uniform mat4 view;

uniform vec2 nearFar;

in vec4 attrPosition;
in vec3 attrNormal;
in vec4 attrInfluence;
//in mat4 instAttrModel;		// instAttrModel[3][3] = ambient

out vec4 normalDepth;

mat4 calcSkinMatrix(vec4 infl);

void main() {
	mat4 skinMat = calcSkinMatrix(attrInfluence);
	vec4 skinnedPos = skinMat * attrPosition;
	vec4 skinnedNrml = skinMat * vec4(attrNormal, .0);

	vec4 posAmbient = /*instAttrModel **/ skinnedPos;
	vec3 normal = normalize(vec3(/*instAttrModel **/ skinnedNrml));

	vec4 eyeCoords = view * vec4(posAmbient.xyz, 1.0);
	float depth = (-eyeCoords.z - nearFar.x) * nearFar.y;

	vec3 eyeNormal = vec3(view * vec4(normal, .0));

	gl_Position = proj * eyeCoords;
	normalDepth = vec4(eyeNormal, depth);
}
