#version 150

in vec4 texCoord;

out float color;

void main() {
	color = texCoord.x;
}
