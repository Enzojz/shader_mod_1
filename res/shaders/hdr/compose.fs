#version 150

uniform sampler2D texFramebuf;
uniform sampler2D texBlurred;

in vec4 texCoord;

out vec4 color;

void main() {
    vec4 col1 = texture(texFramebuf, texCoord.xy);
    vec4 col2 = texture(texBlurred, texCoord.xy);

	//color = col1 + col2;
	color = 255.0/(210.0 - 0.0) * (col1 - 0.0/255.0) + col2;

	//color = col2;
}
