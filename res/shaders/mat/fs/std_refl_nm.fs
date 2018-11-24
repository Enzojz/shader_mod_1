#version 150

uniform mat4 viewInverse;

uniform vec4 matCoeff;

uniform sampler2D colorReflectTex;
uniform samplerCube envTex;
uniform sampler2D normalTex;

in vec4 posAmbient;
in vec3 normal;
in vec3 tangent;
in vec3 binormal;
in vec3 texCoordAlpha;

out vec4 color;

vec3 lightSsao(vec4 posAmbient, vec3 normal, vec4 matCoeff, vec3 color);
vec3 applyFog(vec3 color);
vec3 toneMap(vec3 color);

void main() {
	vec4 nrmlTexValue = texture(normalTex, texCoordAlpha.xy);
	vec3 texNormal = normalize(2.0 * (nrmlTexValue.rgb - vec3(128.0/255.0)));
	mat3 tangentMat = mat3(tangent, binormal, normal);
	vec3 transfNormal = normalize(tangentMat * texNormal);
	
	vec4 col = texture(colorReflectTex, texCoordAlpha.xy);

#if 0
	// comp reflection vector & read cube map
	vec3 camVertex = posAmbient.xyz - viewInverse[3].xyz;
	vec3 reflDir = normalize(reflect(camVertex, transfNormal));
	vec4 reflCol = texture(envTex, reflDir);

	// (the simplest!) appoximation of the fresnel equation
	float reflFactor = (1.0 - dot(transfNormal, reflDir)) * (1.0 - col.w);

	vec3 l = lightSsao(posAmbient, transfNormal, vec4(matCoeff.x, matCoeff.y, matCoeff.z * nrmlTexValue.w, matCoeff.w), col.rgb);
	color.rgb = toneMap(applyFog(mix(l, reflCol.rgb, reflFactor)));
	color.a = texCoordAlpha.z;
#else
	vec4 mc2 = matCoeff;
	mc2.z = matCoeff.z * nrmlTexValue.w;
	mc2.w = matCoeff.w * pow(1000000.0 / matCoeff.w, 1.0 - col.w);

	color.rgb = toneMap(applyFog(lightSsao(posAmbient, transfNormal, mc2, col.rgb)));
	color.a = texCoordAlpha.z;
#endif
}