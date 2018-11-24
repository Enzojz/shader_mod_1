#version 150

uniform sampler2D depthTex;

in vec4 texCoord;

out vec4 color;

void main() {
	vec4 texCol = texture(depthTex, texCoord.xy);

	color = vec4(texCol.r);
	//color = texCol.r == 1.0 ? vec4(1.0) : vec4(.0);
}
