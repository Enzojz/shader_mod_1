#version 150

uniform mat4 viewInverse;

uniform float fogDensity;
uniform vec2 fogDist;			// start, end - start

out float fogFactor;

/*void calcFogFactor(float dist) {
	fogFactor = clamp(exp(-fogDensity * dist), .0, 1.0);
}*/

// TODO change calcFogFactor in fog.vs too!
float calcFogFactor2(vec3 pos) {
	vec3 camPos = vec3(viewInverse[3]);
	//calcFogFactor(length(pos - camPos));

	vec3 weights = vec3(1.0, 1.0, 1.0);		// 1.0, 1.0, .25
	float dist = length(weights * (pos - camPos));
	//return 1.0 - clamp(exp(-fogDensity * (dist - fogDist.x) / fogDist.y), .0, 1.0);
	float f = clamp((dist - fogDist.x) / fogDist.y, .0, 1.0);
	return fogDensity * pow(f, 6.0);
}

void calcFogFactor(vec3 pos) {
	fogFactor = calcFogFactor2(pos);
}
