#version 150

uniform mat4 projView;

in vec4 attrPosition;

in mat4 instAttrModel;		// instAttrModel[3][3] = ambient
in vec4 instAttrColor;

out vec4 color;

void calcFogFactor(vec3 pos);

void main() {
	vec4 posAmbient = instAttrModel * attrPosition;

	gl_Position = projView * vec4(posAmbient.xyz, 1.0);

	calcFogFactor(posAmbient.xyz);
	
	color = instAttrColor;
}
