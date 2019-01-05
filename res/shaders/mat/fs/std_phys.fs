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
vec3 lightPhysSsaoLightmap(vec3 pos, float ambient, vec3 normal, vec3 albedo, float metalness, float glossiness, float brightness);
vec3 applyFog(vec3 color);
vec3 toneMap(vec3 color);

bool isNonTrival(){
	vec4 pattern = vec4(
		texture(albedoTex, vec2(0.25, 0.25)).a,
		texture(albedoTex, vec2(0.75, 0.25)).a,
		texture(albedoTex, vec2(0.25, 0.75)).a,
		texture(albedoTex, vec2(0.75, 0.75)).a
	) * 4;
	return all(equal(round(pattern), vec4(0.0, 4.0, 4.0, 1.0)));
}


void main() {
	vec3 nrml = normal;
	if (!gl_FrontFacing && flipNormal) nrml = -nrml;

	vec3 albedo = texture(albedoTex, texCoordAlpha.xy).rgb;

	vec4 metalGloss = texture(metalGlossAoTex, texCoordAlpha.xy);

	if (isNonTrival())
	{
		vec3 rawColor = lightPhysSsaoLightmap(posAmbient.xyz, posAmbient.w, nrml, albedo, metalGloss.r, metalGloss.g, 1.0);
		color.rgb = toneMap(applyFog(metalGloss.b / metalGloss.a * rawColor));
	}
	else 
	{
		float ambient = min(posAmbient.w, metalGloss.b);
		color.rgb = toneMap(applyFog(lightPhysSsao(posAmbient.xyz, ambient, nrml, albedo, metalGloss.r, metalGloss.g)));
	}
	
	color.a = texCoordAlpha.z;
}
