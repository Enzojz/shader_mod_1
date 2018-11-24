#version 150

float calcFragDepth(float depth) {
	const float SLOPE_SCALE = 1.0;
	const float BIAS = 2.0 * .000015259;		// 1 / (2^16 - 1)

	float m = max(abs(dFdx(depth)), abs(dFdy(depth)));

	return (.5 * depth + .5) + SLOPE_SCALE * m + BIAS;
}
