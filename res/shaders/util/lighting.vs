#version 150

uniform mat4 viewInverse;

//uniform samplerCube lightTex0;
uniform samplerCube lightTex1;
uniform float lightScale;

vec3 decodeRgbe(vec4 color) {
	if (color.a == .0) return vec3(.0);

	//float exponent = 256.0 * color.a - 128.0;
	float exponent = 64.0 * color.a - 32.0;
	return color.rgb * exp2(exponent);
}

vec3 lightNew(vec3 pos, float ambient, vec3 normal, vec3 diffColor, vec3 specColor, float glossiness) {
	// comp reflection vector
	vec3 camVertex = pos - viewInverse[3].xyz;
	vec3 reflDir = normalize(reflect(camVertex, normal));

	// appoximation of the fresnel equation
	//float nDotR = max(dot(normal, reflDir), .0);
	float nDotR = clamp(dot(normal, reflDir), .0, 1.0);
	specColor += max(vec3(glossiness) - specColor, vec3(.0)) * vec3(pow(1.0 - nDotR, 5.0));

	diffColor *= vec3(1.0) - specColor;

	float maxLod = 6.0;
	float specLod = maxLod * (1.0 - glossiness);

	//vec3 diffLight0 = decodeRgbe(textureLod(lightTex0, normal, maxLod));
	//vec3 specLight0 = decodeRgbe(textureLod(lightTex0, reflDir, specLod));

	vec3 diffLight1 = decodeRgbe(textureLod(lightTex1, normal, maxLod));
	vec3 specLight1 = decodeRgbe(textureLod(lightTex1, reflDir, specLod));

	//float shadow = compShadow(pos);		// 1 = no shadow!

	//vec3 diffLight = mix(diffLight0, diffLight1, shadow);
	//vec3 diffLight = ambient * diffLight0 + mix(.35, 1.0, ambient) * shadow * (diffLight1 - diffLight0);
	//vec3 specLight = mix(specLight0, specLight1, shadow);	// TODO ambient (depending on specExp)

	//return lightScale * (diffColor * diffLight + specColor * specLight);

	return lightScale * (diffColor * diffLight1 + specColor * specLight1);
}

vec3 lightNew(vec3 pos, float ambient, vec3 normal, vec3 albedo, float metalness, float glossiness) {
	vec3 diffColor = mix(albedo, vec3(.0), metalness);
	vec3 specColor = mix(vec3(mix(.01, .05, glossiness)), albedo, metalness);

	return lightNew(pos, ambient, normal, diffColor, specColor, glossiness);
}
