#version 150

uniform mat4 projView;

uniform vec2 size;
uniform vec2 ofs;

in vec4 attrPosition;

out vec2 texCoord0;
out vec2 texCoord1;

void main() {
	gl_Position = projView * vec4(attrPosition.x, attrPosition.y, .0, 1.0);
	
	texCoord0 = (attrPosition.xy + ofs) * size;		// calc. window coordinates
	texCoord1 = attrPosition.zw;					// copy height value and blend factor to texcoord 1
}
