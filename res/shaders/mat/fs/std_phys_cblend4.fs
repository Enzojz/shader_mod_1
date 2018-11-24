#version 150

uniform mat4 viewInverse;

uniform sampler2D albedoTex;
uniform sampler2D metalGlossAoTex;
uniform sampler2D cblendTex;

uniform float cblendScales[4];

uniform bool flipNormal;

in vec4 posAmbient;
in vec3 normal;
in vec3 texCoordAlpha;
in vec3 cblendColors[4];

out vec4 color;

vec3 decodeNormalScale(vec2 color, float scale);
vec3 lightPhysSsao(vec3 pos, float ambient, vec3 normal, vec3 albedo, float metalness, float glossiness);
vec3 applyFog(vec3 color);
vec3 toneMap(vec3 color);
vec3 colorBlend(vec3 color, float mask, float scale, vec3 newColor);

void main() {
	vec3 nrml = normal;
	if (!gl_FrontFacing && flipNormal) nrml = -nrml;

	vec3 albedoValue = texture(albedoTex, texCoordAlpha.xy).rgb;

	vec4 cblendValue = texture(cblendTex, texCoordAlpha.xy);

	vec3 albedo = colorBlend(albedoValue, cblendValue.r, cblendScales[0], cblendColors[0]);
	albedo = colorBlend(albedo, cblendValue.g, cblendScales[1], cblendColors[1]);
	albedo = colorBlend(albedo, cblendValue.b, cblendScales[2], cblendColors[2]);
	albedo = colorBlend(albedo, cblendValue.a, cblendScales[3], cblendColors[3]);

	vec3 metalGlossAo = texture(metalGlossAoTex, texCoordAlpha.xy).rgb;

	float ambient = min(posAmbient.w, metalGlossAo.b);

	color.rgb = toneMap(applyFog(lightPhysSsao(posAmbient.xyz, ambient, nrml, albedo, metalGlossAo.r, metalGlossAo.g)));
	color.a = texCoordAlpha.z;
}
