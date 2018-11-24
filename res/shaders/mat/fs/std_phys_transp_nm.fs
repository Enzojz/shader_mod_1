#version 150

uniform vec4 matCoeff;

uniform sampler2D albedoOpacityTex;
uniform sampler2D metalGlossAoTex;
uniform sampler2D normalTex;

uniform float alphaScale;
uniform float normalScale;
//uniform float alphaThreshold;

uniform bool flipNormal;

in vec4 posAmbient;
in vec3 normal;
in vec3 tangent;
in vec3 binormal;
in vec3 texCoordAlpha;

out vec4 color;

vec3 decodeNormalScale(vec2 color, float scale);
vec4 lightPhysSsao(vec3 pos, float ambient, vec3 normal, vec3 albedo, float metalness, float glossiness, float alpha);
vec4 applyFog(vec4 color);
vec4 toneMap(vec4 color);

void main() {
	vec4 col = texture(albedoOpacityTex, texCoordAlpha.xy);

	float alpha = alphaScale * texCoordAlpha.z * col.a;
	//if (alpha <= alphaThreshold) discard;
	
	vec4 nrmlTexValue = texture(normalTex, texCoordAlpha.xy);
	vec3 texNormal = decodeNormalScale(nrmlTexValue.rg, normalScale);
	mat3 tangentMat = mat3(tangent, binormal, normal);
	vec3 nrml = normalize(tangentMat * texNormal);
	if (!gl_FrontFacing && flipNormal) nrml = -nrml;

	vec3 metalGlossAo = texture(metalGlossAoTex, texCoordAlpha.xy).rgb;

	float ambient = min(posAmbient.w, metalGlossAo.b);

	color = toneMap(applyFog(lightPhysSsao(posAmbient.xyz, ambient, nrml, col.rgb, metalGlossAo.r, metalGlossAo.g, alpha)));
}
