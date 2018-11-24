#version 150

uniform vec4 matCoeff;

uniform sampler2D albedoOpacityTex;
uniform sampler2D metalGlossAoTex;
uniform sampler2D normalTex;
uniform sampler2D cblendDirtRustTex;

uniform float alphaScale;
uniform float normalScale;
//uniform float alphaThreshold;

uniform float cblendScale;

uniform bool flipNormal;

in vec4 posAmbient;
in vec3 normal;
in vec3 tangent;
in vec3 binormal;
in vec3 texCoordAlpha;
in vec3 cblendColor;
in float age;

out vec4 color;

vec3 decodeNormalScale(vec2 color, float scale);
vec4 lightPhysSsao(vec3 pos, float ambient, vec3 normal, vec3 albedo, float metalness, float glossiness, float alpha);
vec4 applyFog(vec4 color);
vec4 toneMap(vec4 color);
vec3 colorBlend(vec3 color, float mask, float scale, vec3 newColor);
void applyDirtRust(vec2 texCoord, float dirtMask, float rustMask, float age, inout vec3 albedo,
		inout float gloss, inout vec3 normal);

void main() {
	vec4 col = texture(albedoOpacityTex, texCoordAlpha.xy);

	float alpha = alphaScale * texCoordAlpha.z * col.a;
	//if (alpha <= alphaThreshold) discard;

	vec3 cblendDirtRust = texture(cblendDirtRustTex, texCoordAlpha.xy).rgb;

	vec3 albedo = colorBlend(col.rgb, cblendDirtRust.r, cblendScale, cblendColor);
	
	vec4 nrmlTexValue = texture(normalTex, texCoordAlpha.xy);
	vec3 texNormal = decodeNormalScale(nrmlTexValue.rg, normalScale);

	vec3 metalGlossAo = texture(metalGlossAoTex, texCoordAlpha.xy).rgb;

	float metal = metalGlossAo.r;
	float gloss = metalGlossAo.g;	
	float ambient = min(posAmbient.w, metalGlossAo.b);

	applyDirtRust(texCoordAlpha.xy, cblendDirtRust.g, cblendDirtRust.b, age, albedo, gloss, texNormal);

	mat3 tangentMat = mat3(tangent, binormal, normal);
	vec3 nrml = normalize(tangentMat * texNormal);
	if (!gl_FrontFacing && flipNormal) nrml = -nrml;

	color = toneMap(applyFog(lightPhysSsao(posAmbient.xyz, ambient, nrml, albedo, metal, gloss, alpha)));
}
