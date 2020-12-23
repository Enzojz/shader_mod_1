#version 150
/// BEGIN AUTOGENERATED HEADER
#define CONC(a,b) a##b

#define IF(c, t, e) CONC(IF_, c)(t, e)

#define IF_UNDEF(t, e) e
#define IF_0(t, e) t
#define IF_1(t, e) t
#define IF_2(t, e) t
#define IF_3(t, e) t
#define IF_4(t, e) t
#define IF_5(t, e) t
#define IF_6(t, e) t
#define IF_7(t, e) t
#define IF_8(t, e) t
#define IF_9(t, e) t
#define IF_10(t, e) t
#define IF_11(t, e) t
#define IF_12(t, e) t
#define IF_13(t, e) t
#define IF_14(t, e) t
#define IF_15(t, e) t
#define IF_16(t, e) t
#ifdef GL_SPIRV
	#define LAYOUT_SAMPLER(s, b) layout(set = s, binding = b)
	#define LAYOUT_UBO(s, b) layout(std140, set = s, binding = b)
	#define LAYOUT_PUSH_CONSTANT() layout(push_constant, std140)

	#define VERTEX_INPUT(loc, type, name, defaultValue) IF(loc, layout (location = loc) in type name, const type name = defaultValue)
		
	#define INPUT_MOD(loc, mod, type, name, defaultValue) IF(loc, layout (location = loc) mod in type name, const type name = defaultValue)
		
	#define INPUT(loc, type, name, defaultValue) IF(loc, layout (location = loc) in type name, const type name = defaultValue)

	#define OUTPUT(loc, type, name) IF(loc, layout (location = loc) out type name, type name)
	
	#define OUTPUT_MOD(loc, mod, type, name) IF(loc, layout (location = loc) mod out type name, type name)
	
	#define WINDING cw

	#define LOCATION(loc) layout(location = loc) 
#else
	#extension GL_ARB_separate_shader_objects : require
	#extension GL_ARB_explicit_attrib_location : require
	#extension GL_ARB_shading_language_420pack : require

	#define LAYOUT_SAMPLER(s, b) layout(binding = b)
	#define LAYOUT_UBO(s, b) layout(std140, binding = b)
	#define LAYOUT_PUSH_CONSTANT() layout(std140, binding = 2)

	#define VERTEX_INPUT(loc, type, name, defaultValue) IF(loc, layout (location = loc) in type name, const type name = defaultValue)
		
	#define INPUT_MOD(loc, mod, type, name, defaultValue) mod in type name
		
	#define INPUT(loc, type, name, defaultValue) in type name

	#define OUTPUT(loc, type, name) out type name
	
	#define OUTPUT_MOD(loc, mod, type, name) mod out type name
	
	#define WINDING ccw

	#define LOCATION(loc)
#endif
/// END AUTOGENERATED HEADER

LAYOUT_SAMPLER(2, 23) uniform sampler2D albedoTex;
LAYOUT_SAMPLER(2, 24) uniform sampler2D metalGlossAoTex;
LAYOUT_SAMPLER(2, 26) uniform sampler2D opTex1;
LAYOUT_SAMPLER(2, 27) uniform sampler2D opTex2;

LAYOUT_UBO(1, 3) uniform Material {
	vec4 albedoScales[16];

	ivec2 opSettings1;
	vec2 opScale1;
	float opOpacity1;

	ivec2 opSettings2;
	vec2 opScale2;
	float opOpacity2;

	bool flipNormal;
} u_mat;

INPUT(POS_LOC, vec4, posAmbient, vec4(1.0));
INPUT(NRML_LOC, vec3, normal_, vec3(1.0));
INPUT(TNGT_LOC, vec3, tangent_, vec3(1.0));
INPUT(BINORM_LOC, vec3, binormal_, vec3(1.0));
INPUT_MOD(TC_LOC, centroid, vec3, texCoordAlpha, vec3(1.0));

vec3 getFaceNormal(vec3 normal_, bool flip);
vec3 applyOp(vec3 pos, vec3 nrml, vec2 texCoord, sampler2D tex, ivec2 settings, vec2 scale, float opacity, vec3 color);
void fragmentOutput(vec3 pos, float ambient, vec3 normal, vec3 albedo, float metalness, float glossiness, float alpha);
void fragmentOutput(vec3 pos, float ambient, vec3 normal, vec3 albedo, float metalness, float glossiness, float alpha, float shadow);
void fragmentOutputLightMap(vec3 pos, float ambient, vec3 normal, vec3 albedo, float metalness, float glossiness, float alpha, float brightness);

// These four values means use the mod shader, since they are negative and lower than -1, almost impossible to have impact on non-concerned materials
bool isNonTrival(){ return any(equal(vec4(u_mat.opOpacity1), vec4(-128.0, -256.0, -512.0, -1024.0))); }

// Check if the UV coordinate should be overwritten
vec2 uvPos()
{	
	if (u_mat.opOpacity1 <= -1280.0) // Tangent-Bitangent
	{
		vec3 xDir = normalize(tangent_);
		vec3 yDir = normalize(binormal_);

		vec4 checkValueU = texture(opTex1, vec2(0.25, 0.5)).xyzw;
		vec4 checkValueV = texture(opTex1, vec2(0.75, 0.5)).xyzw;

		vec4 pos = vec4(
			dot(posAmbient.xyz, xDir), 
			dot(posAmbient.xyz, yDir),
			texCoordAlpha.x,
			texCoordAlpha.y
		);

		return vec2(
			dot(step(0.5, checkValueU), pos),
			dot(step(0.5, checkValueV), pos)
		) * u_mat.opScale1;
	}
	else if (u_mat.opOpacity1 <= -1024.0) // Tangent-Bitangent
	{
		vec3 xDir = normalize(tangent_);
		vec3 yDir = normalize(binormal_);
		return vec2(dot(posAmbient.xyz, xDir), dot(posAmbient.xyz, yDir)) * u_mat.opScale1;
	}
	else if (u_mat.opOpacity1 <= -512.0) // Normal-Reference-Axis
	{
		vec3 nrml = getFaceNormal(normal_, u_mat.flipNormal);
		vec3 axis = normalize(texture(opTex1, vec2(0.25, 0.5)).xyz * (step(0.5, texture(opTex1, vec2(0.75, 0.5)).xyz) - 0.5));
		vec3 xDir = cross(nrml, axis);
		vec3 yDir = cross(nrml, xDir);
		return vec2(dot(posAmbient.xyz, xDir), dot(posAmbient.xyz, yDir)) * u_mat.opScale1;
	}
	else if (u_mat.opOpacity1 <= -256.0) // World xyz
	{
		vec3 checkValueU = texture(opTex1, vec2(0.25, 0.5)).xyz;
		vec3 checkValueV = texture(opTex1, vec2(0.75, 0.5)).xyz;

		return vec2(
			dot(step(0.75, checkValueU), posAmbient.xyz) + dot(1.0 - step(0.25, checkValueU), texCoordAlpha.xyz),
			dot(step(0.75, checkValueV), posAmbient.xyz) + dot(1.0 - step(0.25, checkValueV), texCoordAlpha.xyz)
		) * u_mat.opScale1;
	} else if (u_mat.opOpacity1 <= -128.0) // UV
		return texCoordAlpha.xy * u_mat.opScale1;
	else // Non trival
		return texCoordAlpha.xy;
}

void main() {
	vec2 uv = uvPos();
	
	vec3 normal = getFaceNormal(normal_, u_mat.flipNormal);

	vec3 albedo = texture(albedoTex, uv).rgb;

	vec3 metalGlossAo = texture(metalGlossAoTex, uv).rgb;
	
	float ambient = min(posAmbient.w, metalGlossAo.b);

	if (isNonTrival())
	{
		albedo = applyOp(posAmbient.xyz, normal, uv, opTex2, u_mat.opSettings2, u_mat.opScale2, u_mat.opOpacity2, albedo);
		fragmentOutputLightMap(posAmbient.xyz, ambient, normal, albedo * u_mat.albedoScales[0].xyz, metalGlossAo.r, metalGlossAo.g, texCoordAlpha.z, 1.0);
	}
	else
	{
		if (u_mat.opOpacity1 > -128)
			albedo = applyOp(posAmbient.xyz, normal, uv, opTex1, u_mat.opSettings1, u_mat.opScale1, u_mat.opOpacity1, albedo);
		albedo = applyOp(posAmbient.xyz, normal, uv, opTex2, u_mat.opSettings2, u_mat.opScale2, u_mat.opOpacity2, albedo);
		fragmentOutput(posAmbient.xyz, ambient, normal, albedo * u_mat.albedoScales[0].xyz, metalGlossAo.r, metalGlossAo.g, texCoordAlpha.z);
	}

}