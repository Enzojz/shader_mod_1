#version 150

uniform sampler2D tex;

uniform float lightScale;

uniform vec3 fogColor;

in float height;
in vec2 texCoord;

out vec4 color;

vec3 decodeRgbe(vec4 color);
vec3 toneMap(vec3 color);

void main() {
	//vec3 col = texture(tex, texCoord).rgb;
	vec3 col = decodeRgbe(texture(tex, texCoord));

	//float s = clamp(height / 2000.0, .0, 1.0);
	float s = smoothstep(.0, 1000.0, height);

	color.rgb = toneMap(mix(fogColor, lightScale * col, s));
}
