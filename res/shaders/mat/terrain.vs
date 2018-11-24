#version 150

uniform mat4 projView;
uniform mat4 viewInverse;

uniform vec2 texSize;
uniform vec2 texOfs;

in vec4 attrPosAmbient;

out vec3 vpos;
out vec2 texCoord;

//void calcFogFactor(float dist);
void calcFogFactor(vec3 pos);

void main() {
	vpos = attrPosAmbient.xyz;

	gl_Position = projView * attrPosAmbient;

	texCoord = (attrPosAmbient.xy + texOfs) * texSize;

	//gl_FogFragCoord = length(vec3(viewInverse - viewInverse[3]));

	// less fog in high areas.. 00015259 = .5 / 3276.75
	//float dist = length(vec3(posAmbient - viewInverse[3])) * (1.0 - .00015259 * posAmbient.z);
	//calcFogFactor(dist);
	calcFogFactor(vpos);
}
