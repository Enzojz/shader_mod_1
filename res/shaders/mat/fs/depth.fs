#version 150

in float depth;

float calcFragDepth(float depth);

void main() {
	// empty

	gl_FragDepth = calcFragDepth(depth);
}
