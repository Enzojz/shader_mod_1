#version 150

uniform sampler2DRect tex;

in vec2 texCoord0;
in vec2 texCoord1;

out vec4 color;

void main() {
	float s = pow(texCoord1.y, 8.0);
	
	float a = pow(.5 - .5 * cos(3.141592 * texCoord1.y), 2.0);
	float b = texCoord1.y;
	
	float s2 = mix(a, b, s);
		
	float a2 = texture(tex, texCoord0.xy).r;				// get original height value
	float b2 = texCoord1.x;
	
	color.r = mix(a2, b2, s2);
	gl_FragDepth = color.r * .00030518;						// 1/3276.75
}
