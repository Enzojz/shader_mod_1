#version 150

uniform mat4 projView;
uniform mat4 viewInverse;

in vec4 attrPosAmbient;
in vec2 attrTexCoord;

in mat4 instAttrModel;		// instAttrModel[3][3] = ambient

out vec3 texCoordAlpha;

float compFadeOutAlpha(vec3 vpos, vec3 campos);
void calcFogFactor(vec3 pos);

void main() {
	vec4 posAmbient = instAttrModel * attrPosAmbient;
	
	float alpha = compFadeOutAlpha(posAmbient.xyz, vec3(viewInverse[3]));

	gl_Position = projView * vec4(posAmbient.xyz, 1.0);

	texCoordAlpha = vec3(attrTexCoord, alpha);

	calcFogFactor(posAmbient.xyz);
}
