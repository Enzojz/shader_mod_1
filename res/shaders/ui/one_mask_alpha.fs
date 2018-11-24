#version 150

uniform sampler2D mask;

in vec4 texCoord;
in vec4 color;

out vec4 col;

void main() {
	float alpha = color.a * texture(mask, texCoord.xy).r;

	col.rgb = color.rgb;
	col.a = alpha;
}
