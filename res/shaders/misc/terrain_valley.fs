#version 150

in vec4 texCoord;

out float color;

void main() {
	float tmp = 1.0 - pow(.5 * cos(3.141592 * texCoord.y) + .5, texCoord.z);
	
	color = tmp * texCoord.x;					// height value
	gl_FragDepth = 1.0 - .00030518 * color;		// 1/3276.75
}
