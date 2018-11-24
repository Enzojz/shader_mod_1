#version 150

uniform mat4 viewInverse;

uniform sampler2D albedoTex;
uniform sampler2D metalGlossAoTex;
uniform sampler2D normalTex;
uniform sampler2D cblendDirtRustTex;

uniform float cblendScale;
uniform float normalScale;

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
vec3 lightPhysSsao(vec3 pos, float ambient, vec3 normal, vec3 albedo, float metalness, float glossiness);
vec3 applyFog(vec3 color);
vec3 toneMap(vec3 color);
vec3 colorBlend(vec3 color, float mask, float scale, vec3 newColor);
void applyDirtRust(vec2 texCoord, float dirtMask, float rustMask, float age, inout vec3 albedo,
		inout float gloss, inout vec3 normal);

void main() {
	vec4 nrmlTexValue = texture(normalTex, texCoordAlpha.xy);
	vec3 texNormal = decodeNormalScale(nrmlTexValue.rg, normalScale);

	vec3 albedoValue = texture(albedoTex, texCoordAlpha.xy).rgb;

	vec3 cblendDirtRust = texture(cblendDirtRustTex, texCoordAlpha.xy).rgb;

	vec3 albedo = colorBlend(albedoValue, cblendDirtRust.r, cblendScale, cblendColor);

	vec3 metalGlossAo = texture(metalGlossAoTex, texCoordAlpha.xy).rgb;
	
	float metal = metalGlossAo.r;
	float gloss = metalGlossAo.g;	
	float ambient = min(posAmbient.w, metalGlossAo.b);
	
	applyDirtRust(texCoordAlpha.xy, cblendDirtRust.g, cblendDirtRust.b, age, albedo, gloss, texNormal);

	mat3 tangentMat = mat3(tangent, binormal, normal);
	vec3 transfNormal = normalize(tangentMat * texNormal);
	if (!gl_FrontFacing && flipNormal) transfNormal = -transfNormal;
	
	color.rgb = toneMap(applyFog(lightPhysSsao(posAmbient.xyz, ambient, transfNormal, albedo, metal, gloss)));
	color.a = texCoordAlpha.z;
}
