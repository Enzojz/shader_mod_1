#version 150

uniform mat4 viewInverse;

uniform samplerCube lightTex0;
uniform samplerCube lightTex1;
uniform float lightScale;

// shadow mapping
uniform mat4 lightMat;
uniform sampler2DShadow depthTex;

uniform sampler2D ssaoTex;
//uniform sampler2D ssaoTex2;
uniform vec4 viewport;

vec3 decodeRgbe(vec4 color) {
	if (color.a == .0) return vec3(.0);

	//float exponent = 256.0 * color.a - 128.0;
	float exponent = 64.0 * color.a - 32.0;
	return color.rgb * exp2(exponent);
}

float getSsaoTerm() {
	// TODO HACK negative width means SSAO disabled
	if (viewport.x < .0) return 1.0;

	return texture(ssaoTex, gl_FragCoord.xy * viewport.xy + viewport.zw).r;
}

float compShadow(vec3 vpos) {
	vec4 proj = lightMat * vec4(vpos, 1.0);
	proj.xy /= proj.w;

	if (any(lessThan(proj.xy, vec2(-1.0, -1.0))) || any(greaterThan(proj.xy, vec2(1.0, 1.0)))) {
		return 1.0;
	}

	proj.xyz = .5 * proj.xyz + vec3(.5);

	/*float depth = texture(depthTex, proj.xy).r;

	return proj.z <= depth ? 1.0 : .0;*/

	return texture(depthTex, proj.xyz);
}

vec4 lightNew(vec3 pos, float ambient, vec3 normal, vec3 diffColor, vec3 specColor, float glossiness, float alpha, float shadow) {
	// comp reflection vector
	vec3 camVertex = pos - viewInverse[3].xyz;
	vec3 reflDir = normalize(reflect(camVertex, normal));

	// appoximation of the fresnel equation
	//float nDotR = max(dot(normal, reflDir), .0);
	//float nDotR = clamp(dot(normal, reflDir), .0, 1.0);
	float nDotR = min(abs(dot(normal, reflDir)), 1.0);
	specColor += max(vec3(glossiness) - specColor, vec3(.0)) * vec3(pow(1.0 - nDotR, 5.0));

	diffColor *= vec3(1.0) - specColor;

	float maxLod = 6.0;
	float specLod = maxLod * (1.0 - glossiness);

	vec3 refNormal = vec3(normal.xy, normal.z < 0 ? -normal.z : normal.z);

	vec3 diffLight0 = decodeRgbe(textureLod(lightTex0, refNormal, maxLod));
	vec3 specLight0 = decodeRgbe(textureLod(lightTex0, reflDir, specLod));

	vec3 diffLight1 = decodeRgbe(textureLod(lightTex1, refNormal, maxLod));
	vec3 specLight1 = decodeRgbe(textureLod(lightTex1, reflDir, specLod));

	//vec3 diffLight = mix(diffLight0, diffLight1, shadow);
	vec3 diffLight = ambient * diffLight0 + mix(.35, 1.0, ambient) * shadow * (diffLight1 - diffLight0);
	vec3 specLight = mix(specLight0, specLight1, shadow);	// TODO ambient (depending on specExp)

	//return lightScale * (diffColor * diffLight + specColor * specLight);
	// assuming specColor.r = g = b
	float alpha2 = 1.0 - (1.0 - specColor.r) * (1.0 - alpha);
	return vec4(lightScale * (diffColor * diffLight + specColor * specLight), alpha2);
}

vec4 lightNew(vec3 pos, float ambient, vec3 normal, vec3 albedo, float metalness, float glossiness, float alpha) {
	//metalness = 1.0;
	//glossiness = 1.0;

	vec3 diffColor = mix(albedo, vec3(.0), metalness);
	//vec3 specColor = mix(vec3(.04), albedo, metalness);
	vec3 specColor = mix(vec3(mix(.01, .05, glossiness)), albedo, metalness);
	
	float shadow = compShadow(pos);	

	return lightNew(pos, ambient, normal, diffColor, specColor, glossiness, alpha, shadow);
}

vec3 lightNew(vec3 pos, float ambient, vec3 normal, vec3 albedo, float metalness, float glossiness) {
	return lightNew(pos, ambient, normal, albedo, metalness, glossiness, 1.0).rgb;
}

vec3 lightNew(vec3 pos, float ambient, vec3 normal, vec4 matCoeff, vec3 color) {
	float scale = mix(matCoeff.x, matCoeff.y, 7.0/12);		// amb 0.5, diff 0.7

	//vec3 albedo = pow(color, vec3(2.2));
	vec3 albedo = pow(color, vec3(2.2)) * scale;
	float metalness = .0;

	// glossiness = log(specExp) / log(1000000.0)
	// log(specExp) = glossiness * log(1000000.0)
	// specExp = pow(1000000.0, glossiness)

	//float glossiness = log(matCoeff.w) / log(1000000.0);
	float glossiness = log(mix(1.0, matCoeff.w, matCoeff.z)) / log(1000000.0);

	return lightNew(pos, ambient, normal, albedo, metalness, glossiness);
}

vec4 lightPhysSsao(vec3 pos, float ambient, vec3 normal, vec3 albedo, float metalness, float glossiness, float alpha) {
	//metalness = 1.0 - pow(1.0 - metalness, 2.2);
	metalness = metalness * (2.0 - metalness);		// = 1.0 - (1.0 - metalness)^2

	float f1 = .875;
	float f2 = .6522;		// log(2048) / log(1000000)

	glossiness = glossiness <= f1 ? f2 * glossiness : f1 * f2 + (1.0 - f1 * f2) * (glossiness - f1) / (1.0 - f1);

	albedo = pow(albedo, vec3(2.2));

	return lightNew(pos, min(getSsaoTerm(), ambient), normal, albedo, metalness, glossiness, alpha);
}

vec3 lightPhysSsao(vec3 pos, float ambient, vec3 normal, vec3 albedo, float metalness, float glossiness) {
	return lightPhysSsao(pos, ambient, normal, albedo, metalness, glossiness, 1.0).rgb;
}

vec3 light(vec4 posAmbient, vec3 normal, vec4 matCoeff, vec3 color) {
	return lightNew(posAmbient.xyz, posAmbient.w, normal, matCoeff, color);
}

vec3 lightSsao(vec4 posAmbient, vec3 normal, vec4 matCoeff, vec3 color) {
	return lightNew(posAmbient.xyz, min(getSsaoTerm(), posAmbient.w), normal, matCoeff, color);
}

vec4 lightNewNoShadow(vec3 pos, float ambient, vec3 normal, vec3 albedo, float metalness, float glossiness, float alpha) {
	//metalness = 1.0;
	//glossiness = 1.0;

	vec3 diffColor = mix(albedo, vec3(.0), metalness);
	//vec3 specColor = mix(vec3(.04), albedo, metalness);
	vec3 specColor = mix(vec3(mix(.01, .05, glossiness)), albedo, metalness);

	return lightNew(pos, ambient, normal, diffColor, specColor, glossiness, alpha, 1.0);
}

vec4 lightPhysSsaoNoShadow(vec3 pos, float ambient, vec3 normal, vec3 albedo, float metalness, float glossiness, float alpha) {
	//metalness = 1.0 - pow(1.0 - metalness, 2.2);
	metalness = metalness * (2.0 - metalness);		// = 1.0 - (1.0 - metalness)^2

	float f1 = .875;
	float f2 = .6522;		// log(2048) / log(1000000)

	glossiness = glossiness <= f1 ? f2 * glossiness : f1 * f2 + (1.0 - f1 * f2) * (glossiness - f1) / (1.0 - f1);

	albedo = pow(albedo, vec3(2.2));

	return lightNewNoShadow(pos, getSsaoTerm() * ambient, normal, albedo, metalness, glossiness, alpha);
}

vec3 lightPhysSsaoNoShadow(vec3 pos, float ambient, vec3 normal, vec3 albedo, float metalness, float glossiness) {
	return lightPhysSsaoNoShadow(pos, ambient, normal, albedo, metalness, glossiness, 1.0).rgb;
}
