#version 150

uniform mat4 proj;
uniform mat4 view;

uniform mat4 viewInverse;

uniform float alphaScale;

in vec4 attrPosAmbient;
in vec3 attrOffset;
in vec2 attrTexCoord;
in vec3 attrNormal;
in vec3 attrTangent;

in mat4 instAttrModel;

out vec4 posAmbient;
out vec3 normal;
out vec3 tangent;
out vec3 binormal;
out vec3 texCoordAlpha;
flat out int matIndex;

float compFadeOutAlpha(vec3 vpos, vec3 campos);
void calcFogFactor(vec3 pos);

void main() {
	vec4 centerWorld = (instAttrModel * vec4(attrPosAmbient.xyz, 1.0));
	vec4 posEye = view * centerWorld + vec4(attrOffset, .0);

	posAmbient = vec4((viewInverse * posEye).xyz, instAttrModel[3][3] * attrPosAmbient.w);
		
	mat3 model3 = mat3(instAttrModel);
	
	normal = model3 * attrNormal;
	tangent = model3 * attrTangent;
	binormal = cross(normal, tangent);
	
	float alpha = compFadeOutAlpha(centerWorld.xyz, vec3(viewInverse[3]));
	
	texCoordAlpha = vec3(attrTexCoord.x, attrTexCoord.y, alpha);

	matIndex = 0;

	calcFogFactor(centerWorld.xyz);
	
	gl_Position = proj * posEye;
}
