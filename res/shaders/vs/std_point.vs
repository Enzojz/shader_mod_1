#version 150

uniform mat4 projView;

in vec4 attrPosition;
in vec4 attrColor;			// alpha = point size

out vec4 color;

void main() {
    gl_Position = projView * attrPosition;
	gl_PointSize = attrColor.a;
	color = vec4(attrColor.rgb, 1.0);
}
