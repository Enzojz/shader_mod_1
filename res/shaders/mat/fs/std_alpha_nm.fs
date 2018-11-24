#version 150

uniform vec4 matCoeff;

uniform sampler2D colorAlphaTex;
uniform sampler2D normalTex;

struct AlphaScale {
	float alphaScale;
};
uniform AlphaScale alphaScales[16];

uniform float alphaThreshold;

in vec4 posAmbient;
in vec3 normal;
in vec3 tangent;
in vec3 binormal;
in vec3 texCoordAlpha;
flat in int matIndex;

out vec4 color;

vec3 light(vec4 posAmbient, vec3 normal, vec4 matCoeff, vec3 color);
vec3 applyFog(vec3 color);
vec3 toneMap(vec3 color);

void main() {
	vec4 texCol = texture(colorAlphaTex, texCoordAlpha.xy);

	float alpha = alphaScales[matIndex].alphaScale * texCol.a * texCoordAlpha.z;
	if (alpha <= alphaThreshold) discard;
	
	//vec3 texNormal = vec3(.0, .0, 1.0);

	vec4 nrmlTexValue = texture(normalTex, texCoordAlpha.xy);	
	vec3 texNormal = normalize(2.0 * (nrmlTexValue.rgb - vec3(.5)));

	mat3 tangentMat = mat3(tangent, binormal, normal);

	vec3 transfNormal = normalize(tangentMat * texNormal);
	//if (!gl_FrontFacing) transfNormal = -transfNormal;

	color = vec4(toneMap(applyFog(light(posAmbient, transfNormal, vec4(matCoeff.x, matCoeff.y, matCoeff.z * nrmlTexValue.w, matCoeff.w), texCol.rgb))), alpha);
}
