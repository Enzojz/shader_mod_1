#version 150

uniform mat4 projView;
uniform mat4 viewInverse;

in vec4 attrPosAmbient;
in vec3 attrNormal;
in vec2 attrTexCoord;
in vec4 attrTangent;

in mat4 instAttrModel;		// instAttrModel[3][3] = ambient
in vec3 instAttrCblendColor;

out vec4 posAmbient;
out vec3 normal;
out vec3 tangent;
out vec3 binormal;
out vec3 texCoordAlpha;
out vec3 cblendColor;

float compFadeOutAlpha(vec3 vpos, vec3 campos);
void calcFogFactor(vec3 pos);

void main() {
	posAmbient = instAttrModel * attrPosAmbient;
	
	float alpha = compFadeOutAlpha(posAmbient.xyz, vec3(viewInverse[3]));

	gl_Position = projView * vec4(posAmbient.xyz, 1.0);

	normal = vec3(instAttrModel * vec4(attrNormal, .0));
	tangent = vec3(instAttrModel * vec4(vec3(attrTangent), .0));
	binormal = cross(normal, tangent) * attrTangent.w;
	
	texCoordAlpha = vec3(attrTexCoord, alpha);
	
	calcFogFactor(posAmbient.xyz);
	
	cblendColor = instAttrCblendColor;
}
