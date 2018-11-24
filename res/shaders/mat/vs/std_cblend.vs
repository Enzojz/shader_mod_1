#version 150

uniform mat4 projView;

in vec4 attrPosition;
in vec2 attrTexCoord;

in mat4 instAttrModel;		// instAttrModel[3][3] = ambient

out vec2 texCoord;

void main() {
	vec4 posAmbient = instAttrModel * attrPosition;

	gl_Position = projView * vec4(posAmbient.xyz, 1.0);
	texCoord = attrTexCoord;
}
