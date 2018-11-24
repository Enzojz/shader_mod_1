#version 150

uniform mat4 viewInverse;

uniform sampler2D albedoTex;
uniform sampler2D metalGlossAoTex;
uniform sampler2D normalTex;
uniform sampler2D cblendTex;

uniform float cblendScale;
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
in vec3 cblendColor;

out vec4 color;

vec3 decodeNormalScale(vec2 color, float scale);
vec3 lightPhysSsao(vec3 pos, float ambient, vec3 normal, vec3 albedo, float metalness, float glossiness);
vec3 applyFog(vec3 color);
vec3 toneMap(vec3 color);
vec3 colorBlend(vec3 color, float mask, float scale, vec3 newColor);
vec3 applyOp(vec3 pos, vec3 nrml, vec2 texCoord, sampler2D tex, ivec2 settings, vec2 scale, float opacity, vec3 color);

void main() {
	vec4 nrmlTexValue = texture(normalTex, texCoordAlpha.xy);
	vec3 texNormal = decodeNormalScale(nrmlTexValue.rg, normalScale);
	mat3 tangentMat = mat3(tangent, binormal, normal);
	vec3 transfNormal = normalize(tangentMat * texNormal);
	if (!gl_FrontFacing && flipNormal) transfNormal = -transfNormal;

	vec3 albedoValue = texture(albedoTex, texCoordAlpha.xy).rgb;

	float cblendValue = texture(cblendTex, texCoordAlpha.xy).r;

	vec3 albedo = colorBlend(albedoValue, cblendValue, cblendScale, cblendColor);

	albedo = applyOp(posAmbient.xyz, normal, texCoordAlpha.xy, opTex1, opSettings1, opScale1, opOpacity1, albedo);
	albedo = applyOp(posAmbient.xyz, normal, texCoordAlpha.xy, opTex2, opSettings2, opScale2, opOpacity2, albedo);

	vec3 metalGlossAo = texture(metalGlossAoTex, texCoordAlpha.xy).rgb;

	float ambient = min(posAmbient.w, metalGlossAo.b);

	color.rgb = toneMap(applyFog(lightPhysSsao(posAmbient.xyz, ambient, transfNormal, albedo, metalGlossAo.r, metalGlossAo.g)));
	color.a = texCoordAlpha.z;
}
