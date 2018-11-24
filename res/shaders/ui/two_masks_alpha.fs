#version 150

uniform sampler2D mask;
uniform sampler2D borderMask;
uniform sampler2D alphaMask;

uniform vec3 backgroundCol;
uniform vec3 foregroundCol;
uniform vec3 borderCol;

in vec4 texCoord;

out vec4 color;

void main() {
	float mask = texture(mask, texCoord.xy).r;
	float border = texture(borderMask, texCoord.xy).r;
	float alpha = texture(alphaMask, texCoord.xy).r;

	color.rgb = mix(backgroundCol, borderCol, border);
	color.rgb = mix(color.rgb, foregroundCol, mask); 	
	color.a = alpha;	
}
