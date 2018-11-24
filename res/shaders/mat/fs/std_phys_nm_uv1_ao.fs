#version 150

uniform mat4 viewInverse;

uniform sampler2D albedoTex;
uniform sampler2D metalGlossAoTex;
uniform sampler2D normalTex;
uniform sampler2D aoTex;

uniform float normalScale;

uniform bool flipNormal;

in vec4 posAmbient;
in vec3 normal;
in vec3 tangent;
in vec3 binormal;
in vec3 texCoordAlpha;
in vec2 texCoord1;

out vec4 color;

vec3 decodeNormalScale(vec2 color, float scale);
vec3 lightPhysSsaoNoShadow(vec3 pos, float ambient, vec3 normal, vec3 albedo, float metalness, float glossiness);
vec3 lightPhysSsao(vec3 pos, float ambient, vec3 normal, vec3 albedo, float metalness, float glossiness);
vec3 applyFog(vec3 color);
vec3 toneMap(vec3 color);

bool isNonTrival(){
	vec2 bX = vec2(0.125, 0);
	vec2 bY = vec2(0, 0.125);
	
	vec4 v1 = vec4(
		texture(aoTex, bX + bY).b,
		texture(aoTex, 3 * bX + bY).b,
		texture(aoTex, 3 * bX + 3 * bY).b,
		texture(aoTex, bX + 3 * bY).b
	);

	vec4 v2 = vec4(
		texture(aoTex, 5 * bX + bY).b,
		texture(aoTex, 7 * bX + bY).b,
		texture(aoTex, 7 * bX + 3 * bY).b,
		texture(aoTex, 5 * bX + 3 * bY).b
	);
	
	vec4 v3 = vec4(
		texture(aoTex, 5 * bX + 5 * bY).b,
		texture(aoTex, 7 * bX + 5 * bY).b,
		texture(aoTex, 7 * bX + 7 * bY).b,
		texture(aoTex, 5 * bX + 7 * bY).b
	);
	
	vec4 v4 = vec4(
		texture(aoTex, bX + 5 * bY).b,
		texture(aoTex, 3 * bX + 5 * bY).b,
		texture(aoTex, 3 * bX + 7 * bY).b,
		texture(aoTex, bX + 7 * bY).b
	);

	bvec4 r = bvec4(
		all(equal(v1, vec4(0.0, 1.0, 0.0, 1.0))),
		all(equal(v2, vec4(1.0, 1.0, 1.0, 1.0))),
		all(equal(v3, vec4(0.0, 1.0, 0.0, 1.0))),
		all(equal(v4, vec4(1.0, 1.0, 1.0, 1.0)))
	);

	return all(equal(v1, vec4(0.0, 1.0, 0.0, 1.0)));
}

void main() {
	vec4 nrmlTexValue = texture(normalTex, texCoordAlpha.xy);
	vec3 texNormal = decodeNormalScale(nrmlTexValue.rg, normalScale);
	mat3 tangentMat = mat3(tangent, binormal, normal);
	vec3 transfNormal = normalize(tangentMat * texNormal);
	if (!gl_FrontFacing && flipNormal) transfNormal = -transfNormal;

	vec3 albedo = texture(albedoTex, texCoordAlpha.xy).rgb;

	//albedo = applyOp(posAmbient.xyz, normal, texCoordAlpha.xy, opTex1, opSettings1, opScale1, opOpacity1, albedo);
	//albedo = applyOp(posAmbient.xyz, normal, texCoordAlpha.xy, opTex2, opSettings2, opScale2, opOpacity2, albedo);

	vec3 metalGlossAo = texture(metalGlossAoTex, texCoordAlpha.xy).rgb;


	float ambient = min(posAmbient.w, metalGlossAo.b);
	if (true)
		color.rgb = toneMap(applyFog(lightPhysSsao(posAmbient.xyz, ambient, transfNormal, albedo, metalGlossAo.r, metalGlossAo.g)));
	else
	{
		vec4 aoCol = texture(aoTex, texCoord1);
		metalGlossAo.b *= clamp(aoCol.r - .5, .0, .5) * 2.0;
		color.rgb = toneMap(applyFog(lightPhysSsao(posAmbient.xyz, ambient, transfNormal, albedo, metalGlossAo.r, metalGlossAo.g)));
	}
	color.a = texCoordAlpha.z;
}
