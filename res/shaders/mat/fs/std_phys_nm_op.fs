#version 150

uniform mat4 viewInverse;

uniform sampler2D albedoTex;
uniform sampler2D metalGlossAoTex;
uniform sampler2D normalTex;

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

out vec4 color;

vec3 decodeNormalScale(vec2 color, float scale);
vec3 lightPhysSsao(vec3 pos, float ambient, vec3 normal, vec3 albedo, float metalness, float glossiness);
vec3 lightPhysSsaoLightmap(vec3 pos, float ambient, vec3 normal, vec3 albedo, float metalness, float glossiness, float brightness);
vec3 applyFog(vec3 color);
vec3 toneMap(vec3 color);
vec3 applyOp(vec3 pos, vec3 nrml, vec2 texCoord, sampler2D tex, ivec2 settings, vec2 scale, float opacity, vec3 color);

bool isNonTrival(){ return any(equal(vec4(opOpacity1), vec4(-128.0, -256.0, -512.0, -1024.0))); }

vec2 uvPos()
{
	if (opOpacity1 == -256.0) 
	{
		vec3 checkValueU = texture(opTex1, vec2(0.25, 0.5)).xyz;
		vec3 checkValueV = texture(opTex1, vec2(0.75, 0.5)).xyz;

		return vec2(
			dot(step(0.75, checkValueU), posAmbient.xyz) + dot(1.0 - step(0.25, checkValueU), texCoordAlpha.xyz),
			dot(step(0.75, checkValueV), posAmbient.xyz) + dot(1.0 - step(0.25, checkValueV), texCoordAlpha.xyz)
		) * opScale1;
	} else if (opOpacity1 == -512.0)
	{
		vec3 axis = normalize(texture(opTex1, vec2(0.5, 0.5)).xyz);
		vec3 xDir = normalize(cross(normal, axis));
		vec3 yDir = cross(normal, xDir);
		return vec2(dot(posAmbient.xyz, xDir), dot(posAmbient.xyz, yDir)) * opScale1;
	} else if (opOpacity1 == -1024.0)
	{
		vec3 xDir = normalize(tangent);
		vec3 yDir = normalize(binormal);
		return vec2(dot(posAmbient.xyz, xDir), dot(posAmbient.xyz, yDir)) * opScale1;
	}
	else 
		return texCoordAlpha.xy;
}

void main() {
	vec2 uv = uvPos();

	vec4 nrmlTexValue = texture(normalTex, uv);
	vec3 texNormal = decodeNormalScale(nrmlTexValue.rg, normalScale);
	mat3 tangentMat = mat3(tangent, binormal, normal);
	vec3 transfNormal = normalize(tangentMat * texNormal);
	if (!gl_FrontFacing && flipNormal) transfNormal = -transfNormal;
	
	vec3 albedo = texture(albedoTex, uv).rgb;

	vec3 metalGlossAo = texture(metalGlossAoTex, uv).rgb;

	float ambient = min(posAmbient.w, metalGlossAo.b);

	if (isNonTrival())
	{
		albedo = applyOp(posAmbient.xyz, normal, uv, opTex2, opSettings2, opScale2, opOpacity2, albedo);

		vec3 rawColor = lightPhysSsaoLightmap(posAmbient.xyz, ambient, transfNormal, albedo, metalGlossAo.r, metalGlossAo.g, 1.0);
		color.rgb = toneMap(applyFog(rawColor));
	} 
	else
	{
		albedo = applyOp(posAmbient.xyz, normal, uv, opTex1, opSettings1, opScale1, opOpacity1, albedo);
		albedo = applyOp(posAmbient.xyz, normal, uv, opTex2, opSettings2, opScale2, opOpacity2, albedo);

		color.rgb = toneMap(applyFog(lightPhysSsao(posAmbient.xyz, ambient, transfNormal, albedo, metalGlossAo.r, metalGlossAo.g)));
	}
	color.a = texCoordAlpha.z;
}
