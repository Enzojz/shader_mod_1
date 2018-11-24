#version 150

uniform mat4 joints[40];

const float WEIGHTS_SUM_1 = 1.0 / .999;		// see ColladaConv/Writer2.cpp

mat4 calcSkinMatrix(vec4 infl) {
	mat4 ret = (WEIGHTS_SUM_1 * fract(infl[0])) * joints[int(infl[0])];

	for (int i = 1; i < 4; ++i) {
		float weight = fract(infl[i]);
		//if (weight == .0) break;

		ret += (WEIGHTS_SUM_1 * weight) * joints[int(infl[i])];
	}

	return ret;
}
