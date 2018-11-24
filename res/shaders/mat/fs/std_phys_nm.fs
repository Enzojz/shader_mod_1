#version 150

uniform mat4 viewInverse;

uniform sampler2D albedoTex;
uniform sampler2D metalGlossAoTex;
uniform sampler2D normalTex;

uniform float normalScale;

uniform bool flipNormal;

in vec4 posAmbient;
in vec3 normal;
in vec3 tangent;
in vec3 binormal;
in vec3 texCoordAlpha;

out vec4 color;

vec3 decodeNormalScale(vec2 color, float scale);
vec3 lightPhysSsao(vec3 pos, float ambient, vec3 normal, vec3 albedo, float metalness, float glossiness);
vec3 applyFog(vec3 color);
vec3 toneMap(vec3 color);

void main() {
	vec4 nrmlTexValue = texture(normalTex, texCoordAlpha.xy);
	vec3 texNormal = decodeNormalScale(nrmlTexValue.rg, normalScale);
	mat3 tangentMat = mat3(tangent, binormal, normal);
	vec3 transfNormal = normalize(tangentMat * texNormal);
	if (!gl_FrontFacing && flipNormal) transfNormal = -transfNormal;
	
	vec3 albedo = texture(albedoTex, texCoordAlpha.xy).rgb;

	vec3 metalGlossAo = texture(metalGlossAoTex, texCoordAlpha.xy).rgb;

	float ambient = min(posAmbient.w, metalGlossAo.b);
	
	color.rgb = toneMap(applyFog(lightPhysSsao(posAmbient.xyz, ambient, transfNormal, albedo, metalGlossAo.r, metalGlossAo.g)));
	color.a = texCoordAlpha.z;
}
