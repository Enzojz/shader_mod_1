#version 150

uniform mat4 proj;
uniform mat4 view;

uniform vec2 nearFar;

uniform vec2 texSize;
uniform vec2 texOfs;

in vec4 attrPosition;

out vec3 texCoord;

void main() {
	vec4 eyeCoords = view * attrPosition;
	float depth = (-eyeCoords.z - nearFar.x) * nearFar.y;

	vec2 tc = (attrPosition.xy + texOfs) * texSize;

	gl_Position = proj * eyeCoords;
	texCoord = vec3(tc, depth);
}
