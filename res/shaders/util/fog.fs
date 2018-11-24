#version 150

uniform vec3 fogColor;

in float fogFactor;

vec3 applyFog(vec3 color) {
	return mix(color, fogColor, fogFactor);
}

vec4 applyFog(vec4 color) {
	return vec4(applyFog(color.rgb), color.a);
}
