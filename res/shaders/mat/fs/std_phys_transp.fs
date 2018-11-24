#version 150

uniform sampler2D albedoOpacityTex;
uniform sampler2D metalGlossAoTex;

uniform float alphaScale;
//uniform float alphaThreshold;

uniform bool flipNormal;

in vec4 posAmbient;
centroid in vec3 normal;
in vec3 texCoordAlpha;

out vec4 color;

vec4 lightPhysSsao(vec3 pos, float ambient, vec3 normal, vec3 albedo, float metalness, float glossiness, float alpha);
vec4 applyFog(vec4 color);
vec4 toneMap(vec4 color);

void main() {
	vec4 col = texture(albedoOpacityTex, texCoordAlpha.xy);

	float alpha = alphaScale * texCoordAlpha.z * col.a;
	//if (alpha <= alphaThreshold) discard;

	vec3 nrml = normal; 
	if (!gl_FrontFacing && flipNormal) nrml = -nrml;

	vec3 metalGlossAo = texture(metalGlossAoTex, texCoordAlpha.xy).rgb;

	float ambient = min(posAmbient.w, metalGlossAo.b);

	color = toneMap(applyFog(lightPhysSsao(posAmbient.xyz, ambient, nrml, col.rgb, metalGlossAo.r, metalGlossAo.g, alpha)));
}
