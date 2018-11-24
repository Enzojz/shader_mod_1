#version 150

uniform sampler2D tex0;
uniform sampler2D tex1;
uniform sampler2D tex2;
uniform sampler2D tex3;

in vec2 texCoord;

out vec4 color;

//float toneMap(float relLum);

void main() {
	vec4 lums = vec4(texture(tex0, texCoord).a, texture(tex1, texCoord).a,
			texture(tex2, texCoord).a, texture(tex3, texCoord).a);

	//lums = pow(lums, vec4(.75));

	float lum = dot(vec4(1.0, 2.0, 4.0, 8.0) / 15.0, lums);

	float factor = 4.0;

	color = factor * vec4(lum);

	//color = vec4(lums.z);
}
