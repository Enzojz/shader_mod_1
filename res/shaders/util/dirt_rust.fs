#version 150

uniform sampler2D dirtTex;
uniform sampler2D dirtNormalTex;
uniform sampler2D rustTex;
uniform sampler2D rustNormalTex;

uniform vec3 dirtColor;
uniform vec3 rustColor;
uniform float dirtScale; 
uniform float rustScale;
uniform float dirtOpacity;
uniform float rustOpacity; 

vec3 decodeNormal(vec2 color);

void applyDirtRust(vec2 texCoord, float dirtMask, float rustMask, float age, inout vec3 albedo,
		inout float gloss, inout vec3 normal) {
	vec4 dirtTexValue = texture(dirtTex, dirtScale * texCoord);
	vec4 rustTexValue = texture(rustTex, rustScale * texCoord);

	float dirtFactor = clamp((dirtMask - (1.0 - age)) / (age * .92 + .01), .0, 1.0);
	float rustFactor = clamp((rustMask - (1.0 - age)) / (age * .92 + .01), .0, 1.0);

	// 1. rust, 2. dirt
	albedo = mix(albedo, mix(rustColor + 2.0 * rustTexValue.rgb - 1.0, rustColor, 1.0 - rustOpacity), rustFactor);
	albedo = mix(albedo, mix(dirtColor + 2.0 * dirtTexValue.rgb - 1.0, dirtColor, 1.0 - dirtOpacity), dirtFactor);

	gloss = mix(gloss, dirtTexValue.a, dirtFactor);
	gloss = mix(gloss, rustTexValue.a, rustFactor);

	vec3 dirtNormal = decodeNormal(texture(dirtNormalTex, dirtScale * texCoord).rg);
	vec3 rustNormal = decodeNormal(texture(rustNormalTex, rustScale * texCoord).rg);

	normal = mix(normal, dirtNormal, dirtFactor);
	normal = mix(normal, rustNormal, rustFactor);
}
