#version 150

struct AlphaScale {
	float alphaScale;
};
uniform AlphaScale alphaScales[16];

uniform float alphaThreshold;
uniform sampler2D colorAlphaTex;

in float depth;
in vec3 texCoordAlpha;
flat in int matIndex;

float calcFragDepth(float depth);

void main() {
	if (texCoordAlpha.z < .99) discard;		// fade alpha
	
	if (alphaScales[matIndex].alphaScale * texture(colorAlphaTex, texCoordAlpha.xy).a <= alphaThreshold) discard;	// opacity alpha	

	gl_FragDepth = calcFragDepth(depth);
}
