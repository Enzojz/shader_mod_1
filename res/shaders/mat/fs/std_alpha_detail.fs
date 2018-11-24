#version 150

uniform vec4 matCoeff;

uniform sampler2D colorAlphaTex;
uniform sampler2D detailTex;

uniform float alphaScale;
uniform float alphaThreshold;

in vec4 posAmbient;
centroid in vec3 normal;
in vec3 texCoordAlpha;
in vec2 texCoordDetail;

out vec4 color;

vec3 lightSsao(vec4 posAmbient, vec3 normal, vec4 matCoeff, vec3 color);
vec3 applyFog(vec3 color);
vec3 toneMap(vec3 color);

void main() {
	vec4 col = texture(colorAlphaTex, texCoordAlpha.xy);
	float alpha = alphaScale * col.a * texCoordAlpha.z;
	if (alpha <= alphaThreshold) discard;
	
	vec4 colDetail = texture(detailTex, texCoordDetail);
	col.rgb = mix(col.rgb, colDetail.rgb, colDetail.a);
	
	vec3 nrml = normal; 
	//if (!gl_FrontFacing) nrml = -nrml;
	
	color = vec4(toneMap(applyFog(lightSsao(posAmbient, nrml, matCoeff, col.rgb))), alpha);
}
