#version 150

uniform vec4 matCoeff;

uniform sampler2D colorAlphaTex;
uniform sampler2D overlayTex;

uniform sampler2D opTex;
uniform ivec2 opSettings;
uniform vec2 opScale;
uniform float opOpacity;

uniform float alphaScale;
uniform float alphaThreshold;

in vec4 posAmbient;
centroid in vec3 normal;
in vec3 texCoordAlpha;

out vec4 color;

vec3 lightSsao(vec4 posAmbient, vec3 normal, vec4 matCoeff, vec3 color);
vec3 applyFog(vec3 color);
vec3 toneMap(vec3 color);
vec3 applyOp(vec3 pos, vec3 nrml, vec2 texCoord, sampler2D tex, ivec2 settings, vec2 scale, float opacity, vec3 color);

void main() {
	vec4 col = texture(colorAlphaTex, texCoordAlpha.xy);
	
	float alpha = alphaScale * col.a * texCoordAlpha.z;
	if (alpha <= alphaThreshold) discard;

	col.rgb = applyOp(posAmbient.xyz, normal, texCoordAlpha.xy, opTex, opSettings, opScale, opOpacity, col.rgb);

	vec3 nrml = normal; 
	//if (!gl_FrontFacing) nrml = -nrml;

	color = vec4(toneMap(applyFog(lightSsao(posAmbient, nrml, matCoeff, col.rgb))), alpha);
}
