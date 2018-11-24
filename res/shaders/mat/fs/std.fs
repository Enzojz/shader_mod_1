#version 150

uniform sampler2D colorTex;
uniform vec4 matCoeff;

in vec4 posAmbient;
centroid in vec3 normal;
in vec2 texCoord;

out vec4 color;

vec3 lightSsao(vec4 posAmbient, vec3 normal, vec4 matCoeff, vec3 color);
vec3 applyFog(vec3 color);
vec3 toneMap(vec3 color);

void main() {
	vec3 nrml = gl_FrontFacing ? normal : -normal;

	vec4 col = texture(colorTex, texCoord);

	color = vec4(toneMap(applyFog(lightSsao(posAmbient, nrml, matCoeff, col.rgb))), col.a);
}
