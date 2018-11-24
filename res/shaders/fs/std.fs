#version 150

uniform sampler2D tex;

in vec4 texCoord;

out vec4 color;

void main() {
	color = texture(tex, texCoord.xy);
}
