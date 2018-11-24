#version 150

uniform sampler2D tex;

in vec2 texCoord;

out vec4 color;

void main() {
	float lum = texture(tex, texCoord).a;

	color.a = lum;
}
