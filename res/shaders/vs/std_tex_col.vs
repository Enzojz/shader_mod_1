#version 150

uniform mat4 projView;
uniform mat4 textureMatrix;

in vec4 attrPosition;
in vec4 attrTexCoord;
in vec4 attrColor;

out vec4 texCoord;
out vec4 color;

void main() {
    gl_Position = projView * attrPosition;
	texCoord = textureMatrix * attrTexCoord;
	color = attrColor;
}
