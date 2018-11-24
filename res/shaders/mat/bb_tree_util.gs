#version 150

vec3[4] getVertices(vec2 dim, vec3 pos, vec3 tangent, vec3 binormal, float scale) {
	return vec3[](
		pos + scale * (-.5 * dim.x * tangent),
		pos + scale * ( .5 * dim.x * tangent),
		pos + scale * (-.5 * dim.x * tangent + dim.y * binormal),
		pos + scale * ( .5 * dim.x * tangent + dim.y * binormal)
	);
}

vec3[4] getVerticesOH(vec4 coordsOH, float heightOH, vec3 pos, vec3 tangent, vec3 binormal, vec3 normal, float scale) {
	return vec3[](
		pos + scale * (coordsOH[0] * tangent + coordsOH[1] * binormal + heightOH * normal),
		pos + scale * (coordsOH[2] * tangent + coordsOH[1] * binormal + heightOH * normal),
		pos + scale * (coordsOH[0] * tangent + coordsOH[3] * binormal + heightOH * normal),
		pos + scale * (coordsOH[2] * tangent + coordsOH[3] * binormal + heightOH * normal)
	);
}

vec2[4] getTexCoords(vec4 texCoords[7], int faceIdx) {
	vec2 a = texCoords[faceIdx].xy;
	vec2 b = texCoords[faceIdx].zw;

	vec2 d = (a.x < b.x) == (a.y < b.y) ? vec2(b.x - a.x, .0) : vec2(.0, b.y - a.y);

	return vec2[](a, a + d, b - d, b);
}
