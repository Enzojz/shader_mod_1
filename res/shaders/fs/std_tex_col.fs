#version 150

uniform sampler2D tex;

in vec4 texCoord;
in vec4 color;

out vec4 col;

void main() {
	col = color * texture(tex, texCoord.xy);
}
