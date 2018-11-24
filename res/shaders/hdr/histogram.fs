#version 150

uniform sampler2D texFramebuf;
uniform vec2 range;

in vec4 texCoord;

//out vec4 color;

float calcLum(vec3 color);
vec3 unToneMap(vec3 y);

void main() {
    vec3 col = texture(texFramebuf, texCoord.xy).rgb;

	//float lum = calcLum(col);
	float lum = calcLum(unToneMap(col));

	if (lum < range.x || lum >= range.y) discard;

}
