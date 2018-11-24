#version 150

uniform mat4 projView;
uniform mat4 viewInverse;

in vec4 attrPosition;
in vec2 attrTexCoord;

out float height;
out vec2 texCoord;

void main() {
	vec4 pos = attrPosition;
	pos.xyz += vec3(viewInverse[3]);

    gl_Position = projView * pos;
    gl_Position.z = gl_Position.w;		// => depth = 1

	height = pos.z;
	texCoord = attrTexCoord;
}
