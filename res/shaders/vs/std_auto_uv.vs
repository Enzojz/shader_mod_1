#version 150

uniform mat4 projView;

in vec4 attrPosition;

out vec2 texCoord;

void main() {
    gl_Position = projView * attrPosition;
    texCoord = attrPosition.xy;
}
