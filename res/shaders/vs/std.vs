#version 150

uniform mat4 projView;

in vec4 attrPosition;
in vec4 attrTexCoord;

out vec4 texCoord;

void main() {
    gl_Position = projView * attrPosition;
	texCoord = attrTexCoord;
}
