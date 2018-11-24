#version 150

uniform mat4 viewInverse;

uniform vec4 matCoeff;

uniform sampler2D colorReflectTex;
uniform samplerCube envTex;

uniform sampler2D opTex;
uniform ivec2 opSettings;
uniform vec2 opScale;
uniform float opOpacity;

in vec4 posAmbient;
centroid in vec3 normal;
in vec3 texCoordAlpha;

out vec4 color;

vec3 lightSsao(vec4 posAmbient, vec3 normal, vec4 matCoeff, vec3 color);
vec3 applyFog(vec3 color);
vec3 toneMap(vec3 color);
vec3 applyOp(vec3 pos, vec3 nrml, vec2 texCoord, sampler2D tex, ivec2 settings, vec2 scale, float opacity, vec3 color);

void main() {
	vec3 nrml = gl_FrontFacing ? normal : -normal;

	vec4 col = texture(colorReflectTex, texCoordAlpha.xy);

	col.rgb = applyOp(posAmbient.xyz, normal, texCoordAlpha.xy, opTex, opSettings, opScale, opOpacity, col.rgb);

#if 0
	// comp reflection vector & read cube map
	vec3 camVertex = posAmbient.xyz - viewInverse[3].xyz;
	vec3 reflDir = normalize(reflect(camVertex, nrml));
	vec4 reflCol = texture(envTex, reflDir);

	// (the simplest!) appoximation of the fresnel equation
	float reflFactor = (1.0 - dot(nrml, reflDir)) * (1.0 - col.w);

	vec3 l = lightSsao(posAmbient, nrml, matCoeff, col.rgb);
	color.rgb = toneMap(applyFog(mix(l, reflCol.rgb, reflFactor)));
	color.a = texCoordAlpha.z;
#else
	vec4 mc2 = matCoeff;
	mc2.w = matCoeff.w * pow(1000000.0 / matCoeff.w, 1.0 - col.w);

	color.rgb = toneMap(applyFog(lightSsao(posAmbient, nrml, mc2, col.rgb)));
	color.a = texCoordAlpha.z;
#endif
}
