#version 150

uniform mat4 viewInverse;

uniform sampler2D albedoGlossTex;

uniform bool flipNormal;

in vec4 posAmbient;
centroid in vec3 normal;
in vec3 texCoordAlpha;

out vec4 color;

vec3 lightPhysSsao(vec3 pos, float ambient, vec3 normal, vec3 albedo, float metalness, float glossiness);
vec3 applyFog(vec3 color);
vec3 toneMap(vec3 color);

void main() {
	vec3 nrml = normal;
	if (!gl_FrontFacing && flipNormal) nrml = -nrml;

	vec4 albedoGloss = texture(albedoGlossTex, texCoordAlpha.xy);

	vec3 albedo = albedoGloss.rgb;
	float metal = .0;
	float gloss = albedoGloss.a;
	float ambient = posAmbient.w;

	color.rgb = toneMap(applyFog(lightPhysSsao(posAmbient.xyz, ambient, nrml, albedo, metal, gloss)));
	color.a = texCoordAlpha.z;
}
