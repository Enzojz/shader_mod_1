#version 150

uniform sampler2D colorAlphaTex;

struct AlphaScale {
	float alphaScale;
};
uniform AlphaScale alphaScales[16];

uniform float alphaThreshold;

in vec3 texCoordAlpha;
in vec4 color;
flat in int matIndex;

out vec4 color0;

void main() {
	float alpha = texture(colorAlphaTex, texCoordAlpha.xy).a * alphaScales[matIndex].alphaScale * texCoordAlpha.z;

	if (alpha <= alphaThreshold) discard;

	color0 = vec4(color.rgb, color.a * alpha);
}
