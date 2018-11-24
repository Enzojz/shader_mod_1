#version 150

uniform mat4 viewInverse;

uniform sampler2D albedoTex;
uniform sampler2D metalGlossAoTex;
uniform sampler2D normalTex;
uniform sampler2D aoTex;

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
in vec2 texCoord1;

out vec4 color;

vec3 decodeNormalScale(vec2 color, float scale);
vec3 lightPhysSsao(vec3 pos, float ambient, vec3 normal, vec3 albedo, float metalness, float glossiness);
vec3 lightPhysSsaoNoShadow(vec3 pos, float ambient, vec3 normal, vec3 albedo, float metalness, float glossiness);
vec3 applyFog(vec3 color);
vec3 toneMap(vec3 color);
vec3 applyOp(vec3 pos, vec3 nrml, vec2 texCoord, sampler2D tex, ivec2 settings, vec2 scale, float opacity, vec3 color);

bool isNonTrival(){
	vec4 pattern = vec4(
		texture(aoTex, vec2(0.25, 0.25)).b,
		texture(aoTex, vec2(0.75, 0.25)).b,
		texture(aoTex, vec2(0.25, 0.75)).b,
		texture(aoTex, vec2(0.75, 0.75)).b
	) * 4;
	return all(equal(round(pattern), vec4(0.0, 4.0, 4.0, 1.0)));
}

vec2 uvPos()
{
	vec4 checkValue = texture(aoTex, vec2(0.0625, 0.0625)) * 4;
	if (checkValue.b == 0.0)  
		return texCoordAlpha.xy;
	else //(checkValue == 1.0)
		return posAmbient.xy;
	// else if (checkValue == 2.0)
	// {
	// 	vec2 nxy = normalize(normal.xy);
	// 	vec2 mxy = nxy.yx * (mod(posAmbient.xy));
	// 	return vec2(mxy.x, posAmbient.z);
	// }
	// else
	// {
	// 	vec2 nxy = normalize(normal.xy);
	// 	vec2 mxy = nxy.yx * (mod(posAmbient.xy));
	// 	return vec2(mxy.y, posAmbient.z);
	// }
}

void main() {
	if (isNonTrival())
	{
		vec2 uv = uvPos();

		vec4 nrmlTexValue = texture(normalTex, uv);
		vec3 texNormal = decodeNormalScale(nrmlTexValue.rg, normalScale);
		mat3 tangentMat = mat3(tangent, binormal, normal);
		vec3 transfNormal = normalize(tangentMat * texNormal);
		if (!gl_FrontFacing && flipNormal) transfNormal = -transfNormal;

		vec3 albedo = texture(albedoTex, uv).rgb;
		vec3 metalGlossAo = texture(metalGlossAoTex, uv).rgb;

		float ambient = min(posAmbient.w, metalGlossAo.b);	
		color.rgb = toneMap(applyFog(lightPhysSsaoNoShadow(posAmbient.xyz, ambient, transfNormal, albedo, metalGlossAo.r, metalGlossAo.g)));
	}
	else 
	{
		vec4 nrmlTexValue = texture(normalTex, texCoordAlpha.xy);
		vec3 texNormal = decodeNormalScale(nrmlTexValue.rg, normalScale);
		mat3 tangentMat = mat3(tangent, binormal, normal);
		vec3 transfNormal = normalize(tangentMat * texNormal);
		if (!gl_FrontFacing && flipNormal) transfNormal = -transfNormal;

		vec3 albedo = texture(albedoTex, texCoordAlpha.xy).rgb;

		// albedo = applyOp(posAmbient.xyz, normal, texCoordAlpha.xy, opTex1, opSettings1, opScale1, opOpacity1, albedo);
		// albedo = applyOp(posAmbient.xyz, normal, texCoordAlpha.xy, opTex2, opSettings2, opScale2, opOpacity2, albedo);

		vec3 metalGlossAo = texture(metalGlossAoTex, texCoordAlpha.xy).rgb;

		vec4 aoCol = texture(aoTex, texCoord1);
		metalGlossAo.b *= clamp(aoCol.r - .5, .0, .5) * 2.0;	
		float ambient = min(posAmbient.w, metalGlossAo.b);	
		color.rgb = toneMap(applyFog(lightPhysSsao(posAmbient.xyz, ambient, transfNormal, albedo, metalGlossAo.r, metalGlossAo.g)));
	}
	color.a = texCoordAlpha.z;
}
