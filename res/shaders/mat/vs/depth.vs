#version 150

uniform mat4 projView;

in vec4 attrPosition;
in mat4 instAttrModel;		// instAttrModel[3][3] = ambient

out float depth;

void main() {
	vec4 posAmbient = instAttrModel * attrPosition;

	gl_Position = projView * vec4(posAmbient.xyz, 1.0);

	depth = gl_Position.z;
}
