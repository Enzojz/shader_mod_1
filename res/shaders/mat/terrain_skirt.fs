#version 150

uniform sampler2D colorTex;
uniform sampler2D detailTex;
uniform vec4 matCoeff;

in vec4 posAmbient;
in vec3 normal;
in vec2 texCoord;
in vec2 texCoord2;

out vec4 color;

vec3 light(vec4 posAmbient, vec3 normal, vec4 matCoeff, vec3 color);
vec3 applyFog(vec3 color);
vec3 toneMap(vec3 color);

void main() {
	vec3 col = texture(colorTex, texCoord).rgb;
	float detail = texture(detailTex, texCoord2).r;

	col *= .8 * detail + .6;

	color.rgb = toneMap(applyFog(light(posAmbient, normal, matCoeff, col)));
}
