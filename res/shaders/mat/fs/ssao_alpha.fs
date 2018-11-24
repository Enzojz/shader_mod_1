#version 150

uniform float alphaScale;
uniform float alphaThreshold;
uniform sampler2D colorAlphaTex;

in vec4 normalDepth;
in vec2 texCoord;

out vec4 color;

void main() {
	float alpha = alphaScale * texture(colorAlphaTex, texCoord).a;
	if (alpha <= alphaThreshold) discard;

	color = vec4(vec3(.5) + .5 * normalDepth.xyz * (gl_FrontFacing ? 1.0 : -1.0), sqrt(normalDepth.w));
}
