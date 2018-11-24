#version 150

uniform mat4 viewInverse;

uniform sampler2D albedoTex;
uniform sampler2D metalGlossAoTex;

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

	vec3 albedo = texture(albedoTex, texCoordAlpha.xy).rgb;

	vec3 metalGlossAo = texture(metalGlossAoTex, texCoordAlpha.xy).rgb;

	float ambient = min(posAmbient.w, metalGlossAo.b);

	color.rgb = toneMap(applyFog(lightPhysSsao(posAmbient.xyz, ambient, nrml, albedo, metalGlossAo.r, metalGlossAo.g)));
	color.a = texCoordAlpha.z;
}
