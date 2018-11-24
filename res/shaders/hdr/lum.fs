#version 150

uniform sampler2D tex;

float calcLum(vec3 color);
//float unToneMap(float finalLum);
//float unToneMapRel(float finalLum);
vec3 unToneMap(vec3 y);

in vec2 texCoord;

out vec4 color;

/*vec3 suppressLdr(vec3 col) {
	float lum = calcLum(col);

	//return lum < .75 ? vec4(.0) : col;
	//return 3.0 * (lum - .667) * col;
	//return 4.0 * (lum - .75) * col;
	//return 5.0 * (lum - .8) * col;
	return 10.0 * (lum - .9) * col;
	//return 20.0 * (lum - .95) * col;
}*/

void main() {
	vec3 col = texture(tex, texCoord).rgb;

	/*float finalLum = calcLum(col);

	//float lum = unToneMap(finalLum);
	float lum = unToneMapRel(finalLum);

	//color.a = 10.0 * (finalLum - .9) * lum;
	//color.a = 10.0 * (lum - .9);
	//color.a = lum >= .9 ? 9.0 * (lum - .89) : .0;
	//color.a = lum;

	//color.a = 2.0 * (lum - 1.0);
	color.a = lum;*/

	color.a = pow(max(calcLum(col) - .85, .0) / .15, 2.0);
}
