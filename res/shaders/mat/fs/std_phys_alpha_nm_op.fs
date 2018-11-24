#version 150

uniform vec4 matCoeff;

uniform sampler2D albedoAlphaTex;
uniform sampler2D metalGlossAoTex;
uniform sampler2D normalTex;

uniform float alphaScale;
uniform float alphaThreshold;

uniform float normalScale;

uniform sampler2D opTex1;
uniform ivec2 opSettings1;
uniform vec2 opScale1;
uniform float opOpacity1;

uniform sampler2D opTex2;
uniform ivec2 opSettings2;
uniform vec2 opScale2;
uniform float opOpacity2;

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
vec3 applyOp(vec3 pos, vec3 nrml, vec2 texCoord, sampler2D tex, ivec2 settings, vec2 scale, float opacity, vec3 color);

void main() {
	vec4 col = texture(albedoAlphaTex, texCoordAlpha.xy);

	float alpha = alphaScale * texCoordAlpha.z * col.a;
	if (alpha <= alphaThreshold) discard;

	vec3 albedo = col.rgb;
	albedo = applyOp(posAmbient.xyz, normal, texCoordAlpha.xy, opTex1, opSettings1, opScale1, opOpacity1, albedo);
	albedo = applyOp(posAmbient.xyz, normal, texCoordAlpha.xy, opTex2, opSettings2, opScale2, opOpacity2, albedo);

	vec4 nrmlTexValue = texture(normalTex, texCoordAlpha.xy);
	vec3 texNormal = decodeNormalScale(nrmlTexValue.rg, normalScale);
	mat3 tangentMat = mat3(tangent, binormal, normal);
	vec3 nrml = normalize(tangentMat * texNormal);
	if (!gl_FrontFacing && flipNormal) nrml = -nrml;

	vec3 metalGlossAo = texture(metalGlossAoTex, texCoordAlpha.xy).rgb;

	float ambient = min(posAmbient.w, metalGlossAo.b);

	color = toneMap(applyFog(lightPhysSsao(posAmbient.xyz, ambient, nrml, albedo, metalGlossAo.r, metalGlossAo.g, alpha)));
}
