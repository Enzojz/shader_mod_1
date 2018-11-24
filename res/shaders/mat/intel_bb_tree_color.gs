#version 150

layout (points) in;
layout (triangle_strip, max_vertices = 4) out;

uniform mat4 projView;
uniform mat4 viewInverse;

in VertexData {
	vec3 position;
	vec4 normalScale;
	flat int index;

	vec4 instColor;

	vec4 texCoords[7];
	vec4 dimFadeIn;
	vec4 coordsOH;
	float heightOH;
} inData[];

out vec3 texCoordAlpha;
out vec4 color;
flat out int matIndex;

float compFadeOutAlpha(vec3 vpos, vec3 campos, int index);
vec3[4] getVertices(vec2 dim, vec3 pos, vec3 tangent, vec3 binormal, float scale);
vec3[4] getVerticesOH(vec4 coordsOH, float heightOH, vec3 pos, vec3 tangent, vec3 binormal, vec3 normal, float scale);
vec2[4] getTexCoords(vec4 texCoords[7], int faceIdx);

void main() {
	vec3 vpos = inData[0].position;
	vec3 vnrml = inData[0].normalScale.xyz;
	float scale = inData[0].normalScale.w;
	vec4 vcolor = inData[0].instColor;
	int vindex = inData[0].index;

	float fadeOutAlpha = compFadeOutAlpha(vpos, viewInverse[3].xyz, vindex);
	if (fadeOutAlpha <= .0) return;

	vec4 texCoords[7] = inData[0].texCoords;
	vec2 fadeInAngle = inData[0].dimFadeIn.zw;
	vec2 dim = inData[0].dimFadeIn.xy;

	const float PI = 3.14159265;

	vec3 vertexToCam = viewInverse[3].xyz - vpos;

	float angle = atan(vertexToCam.z / length(vertexToCam.xy));
	
	color = vcolor;
	matIndex = vindex;
	
	vec3 binormal = vec3(.0, .0, 1.0);
	vec3 tangent = normalize(cross(binormal, vertexToCam));
	vec3 normal = cross(tangent, binormal);

	float instanceRot = atan(vnrml.y, vnrml.x);
	float hAngle = atan(vertexToCam.y, vertexToCam.x) - instanceRot + PI / 6.0;		// [-pi, pi] - [0, 2*pi] + pi/6
	int j = int(12.0 + 3.0 * hAngle / PI) % 6;

	vec3[4] vertices = getVertices(dim, vpos, tangent, binormal, scale);
	vec2[4] tcs = getTexCoords(texCoords, j);

	for (int i = 0; i < 4; ++i) {
		texCoordAlpha = vec3(tcs[i], fadeOutAlpha);	
		gl_Position = projView * vec4(vertices[i], 1.0);
		EmitVertex();
	}
}
