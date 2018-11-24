#version 150

uniform sampler2D tex;

in vec4 color;

out vec4 col;

void main() {
	col = color * texture(tex, gl_PointCoord);
}
