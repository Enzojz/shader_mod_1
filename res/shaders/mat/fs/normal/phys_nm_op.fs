#version 150

uniform mat4 viewInverse;

uniform sampler2D albedoTex;
uniform sampler2D metalGlossAoTex;
uniform sampler2D normalTex;

uniform vec3 albedoScale;

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
in vec3 normal_;
in vec3 tangent_;
in vec3 binormal_;
centroid in vec3 texCoordAlpha;

vec3 decodeNormalScale(vec2 color, float scale);
vec3 getFaceNormal(vec3 normal_, bool flip);
vec3 getNormalMapped(vec3 texNormal, vec3 normal_, vec3 binormal_, vec3 tangent_, bool flip);
vec3 applyOp(vec3 pos, vec3 nrml, vec2 texCoord, sampler2D tex, ivec2 settings, vec2 scale, float opacity, vec3 color);
void fragmentOutput(vec3 pos, float ambient, vec3 normal, vec3 albedo, float metalness, float glossiness, float alpha);
void fragmentOutput(vec3 pos, float ambient, vec3 normal, vec3 albedo, float metalness, float glossiness, float alpha, float shadow);
void fragmentOutputLm(vec3 pos, float ambient, vec3 normal, vec3 albedo, float metalness, float glossiness, float alpha, float brightness);

// These four values means use the mod shader, since they are negative and lower than -1, almost impossible to have impact on non-concerned materials
bool isNonTrival(){ return any(equal(vec4(opOpacity1), vec4(-128.0, -256.0, -512.0, -1024.0))); }

// Check if the UV coordinate should be overwritten
vec2 uvPos()
{	
	if (opOpacity1 <= -1024.0) // Tangent-Bitangent
	{
		vec3 xDir = normalize(tangent_);
		vec3 yDir = normalize(binormal_);
		return vec2(dot(posAmbient.xyz, xDir), dot(posAmbient.xyz, yDir)) * opScale1;
	}
	else if (opOpacity1 <= -512.0) // Normal-Reference-Axis
	{
		vec3 nrml = getFaceNormal(normal_, flipNormal);
		vec3 axis = normalize(texture(opTex1, vec2(0.25, 0.5)).xyz * (step(0.5, texture(opTex1, vec2(0.75, 0.5)).xyz) - 0.5));
		vec3 xDir = cross(nrml, axis);
		vec3 yDir = cross(nrml, xDir);
		return vec2(dot(posAmbient.xyz, xDir), dot(posAmbient.xyz, yDir)) * opScale1;
	}
	else if (opOpacity1 <= -256.0) // World xyz
	{
		vec3 checkValueU = texture(opTex1, vec2(0.25, 0.5)).xyz;
		vec3 checkValueV = texture(opTex1, vec2(0.75, 0.5)).xyz;

		return vec2(
			dot(step(0.75, checkValueU), posAmbient.xyz) + dot(1.0 - step(0.25, checkValueU), texCoordAlpha.xyz),
			dot(step(0.75, checkValueV), posAmbient.xyz) + dot(1.0 - step(0.25, checkValueV), texCoordAlpha.xyz)
		) * opScale1;
	} else if (opOpacity1 <= -128.0) // UV
		return texCoordAlpha.xy * opScale1;
	else // Non trival
		return texCoordAlpha.xy;
}

void main() {
	vec2 uv = uvPos();
	
	vec3 faceNormal = getFaceNormal(normal_, flipNormal);
	
	vec3 texNormal = decodeNormalScale(texture(normalTex, uv).rg, normalScale);
	vec3 normal = getNormalMapped(texNormal, normal_, binormal_, tangent_, flipNormal);

	vec3 albedo = texture(albedoTex, uv).rgb;

	albedo = applyOp(posAmbient.xyz, faceNormal, uv, opTex2, opSettings2, opScale2, opOpacity2, albedo);

	vec3 metalGlossAo = texture(metalGlossAoTex, texCoordAlpha.xy).rgb;

	float ambient = min(posAmbient.w, metalGlossAo.b);

	if (isNonTrival())
	{
		albedo = applyOp(posAmbient.xyz, faceNormal, uv, opTex2, opSettings2, opScale2, opOpacity2, albedo);
		fragmentOutputLm(posAmbient.xyz, ambient, normal, albedo * albedoScale, metalGlossAo.r, metalGlossAo.g, texCoordAlpha.z, 1.0);
	} 
	else
	{
		if (opOpacity1 > -128)
			albedo = applyOp(posAmbient.xyz, faceNormal, uv, opTex1, opSettings1, opScale1, opOpacity1, albedo);
		albedo = applyOp(posAmbient.xyz, faceNormal, uv, opTex2, opSettings2, opScale2, opOpacity2, albedo);

		fragmentOutput(posAmbient.xyz, ambient, normal, albedo * albedoScale, metalGlossAo.r, metalGlossAo.g, texCoordAlpha.z);
	}
	
}
