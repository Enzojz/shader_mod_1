#version 150

uniform float lightScale;
uniform sampler2D emissiveTex;

in vec3 texCoordAlpha;

out vec4 color;

vec3 decodeRgbe(vec4 color);
vec3 applyFog(vec3 color);
vec3 toneMap(vec3 color);

void main() {
	vec4 tex = texture(emissiveTex, texCoordAlpha.xy);

	vec3 col = lightScale == 1.0 ? decodeRgbe(tex) : lightScale * tex.rbg;
		
	color.rgb = toneMap(applyFog(col));
	color.a = texCoordAlpha.z;
}
