#version 150

uniform sampler2D colorTex;
uniform sampler2D nrmlTex;
uniform vec4 matCoeff;

in vec3 vpos;
in vec2 texCoord;

out vec4 color;

vec3 lightSsao(vec4 posAmbient, vec3 normal, vec4 matCoeff, vec3 color);
vec3 applyFog(vec3 color);
vec3 toneMap(vec3 color);

vec4 rgb2nrmlAmb(vec3 rgb);

void main() {
	vec3 col = texture(colorTex, texCoord).rgb;
	
	if (col.x > 0.9 && col.y < 0.1 && col.z > 0.9) {
		discard; 
	}
	
	vec4 nrmlAmb = rgb2nrmlAmb(texture(nrmlTex, texCoord).rgb);

	color.rgb = toneMap(applyFog(lightSsao(vec4(vpos, nrmlAmb.w), nrmlAmb.xyz, matCoeff, col)));
}
