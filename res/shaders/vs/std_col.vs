#version 150

uniform mat4 projView;

in vec4 attrPosition;
in vec4 attrColor;

out vec4 color;

void main() {
    gl_Position = projView * attrPosition;
	color = attrColor;
}
