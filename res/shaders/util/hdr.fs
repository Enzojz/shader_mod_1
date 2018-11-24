#version 150

uniform float exposure;

float calcLum(vec3 color) {
	const vec3 weights = vec3(.2126, .7152, .0722);
	//const vec3 weights = vec3(.27, .67, .06);
	//const vec3 weights = vec3(.3, .59, .11);
	return dot(weights, color);
}

vec3 toneMap(vec3 color) {
	color *= exposure;

	//vec3 x = max(color - vec3(.004), vec3(.0));
	vec3 x = color;
	return (x * (6.2 * x + vec3(.5))) / (x * (6.2 * x + vec3(1.7)) + vec3(.06));
}

vec4 toneMap(vec4 color) {
	return vec4(toneMap(color.rgb), color.a);
}

vec3 unToneMap(vec3 y) {
	float c = exposure;
	//return (0.0000645161 * (-55.9017 * sqrt(701.0*c*c*y*y - 106.0*c*c*y + vec3(125.0*c*c)) - 2063.0*c*y + vec3(563.0*c))) / (c*c*y - vec3(c*c));
	return (0.0016129 * (-2.23607 * sqrt(701.0*c*c*y*y - 106.0*c*c*y + vec3(125.0*c*c)) - 85.0*c*y + vec3(25.0*c))) / (c*c*y - vec3(c*c));
}
