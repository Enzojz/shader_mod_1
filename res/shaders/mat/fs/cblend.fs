#version 150

uniform sampler2D cblendTex;

//uniform float cblendScale;

in vec2 texCoord;

out vec4 color;

void main() {
	color.a = 1.0 - texture(cblendTex, texCoord).r;
}
