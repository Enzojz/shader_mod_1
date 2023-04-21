#version 150

uniform mat4 view;
uniform mat4 viewInverse;

uniform vec3 lightDir;
uniform mat4 lightTransf;
uniform mat4 lightInvTransf;

uniform samplerCube lightTex0;
uniform samplerCube lightTex1;
uniform float lightScale;
uniform float ambientScale;

// shadow mapping
uniform mat4 lightMat;
uniform sampler2DShadow depthTex;

out vec4 color0;
out vec4 color1;

vec4 applyFog(vec4 color);

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

vec4 getLight(samplerCube lightTex, vec3 dir, float lod) {
	return textureLod(lightTex, vec3(lightInvTransf * vec4(dir, 1.0)), lod);
}

vec4 lightDiffSpec(vec3 pos, float ambient, vec3 normal, vec3 diffColor, vec3 specColor, float glossiness,
		float alpha, float shadow) {
	
	diffColor = clamp(diffColor, vec3(.0), vec3(1.0));
	specColor = clamp(specColor, vec3(.0), vec3(1.0));

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

	vec3 diffLight0 = getLight(lightTex0, normal, maxLod).rgb;
	
	vec3 specLight0 = getLight(lightTex0, reflDir, specLod).rgb;

	vec3 diffLight1 = getLight(lightTex1, normal, maxLod).rgb;
	vec3 specLight1 = getLight(lightTex1, reflDir, specLod).rgb;

	if (ambientScale > 1.0) {
		vec3 diffF = vec3(max(dot(normal, lightDir), .0));
		vec3 diffB = + vec3(max(dot(normal, vec3(0, 0, 1)), .0));
	
		float diffNorm = 1.0 / lightScale;
	
		diffLight0 = diffNorm * .25 * (1.0 + diffB);
		diffLight1 = diffNorm * 1.5 * ((diffLight0 + diffF));
		
		specLight0 = vec3(.1 / lightScale);
		specLight1 = vec3((1.0 / lightScale) * max(dot(reflDir, lightDir), .0));
	}

	//vec3 diffLight = mix(diffLight0, diffLight1, shadow);
	vec3 diffLight = ambientScale * ambient * diffLight0 + 1.0 / ambientScale * mix(.35, 1.0, ambient) * shadow * (diffLight1 - diffLight0);
	vec3 specLight = mix(ambientScale * specLight0, 1.0 / ambientScale * specLight1, shadow);	// TODO ambient (depending on specExp)

	//return lightScale * (diffColor * diffLight + specColor * specLight);
	// assuming specColor.r = g = b
	float alpha2 = 1.0 - (1.0 - specColor.r) * (1.0 - alpha);

	return vec4(lightScale * (diffColor * diffLight + specColor * specLight), alpha2);
}

vec4 lightDiffSpec(vec3 pos, float ambient, vec3 normal, vec3 diffColor, vec3 specColor, float glossiness,
		float alpha) {
		
	float shadow = compShadow(pos);
	return lightDiffSpec(pos, ambient, normal, diffColor, specColor, glossiness, alpha, shadow);
}

vec4 lightPhysLinear(vec3 pos, float ambient, vec3 normal, vec3 albedo, float metalness, float glossiness,
		float alpha, float shadow) {

	vec3 diffColor = mix(albedo, vec3(.0), metalness);
	//vec3 specColor = mix(vec3(.04), albedo, metalness);
	vec3 specColor = mix(vec3(mix(.01, .05, glossiness)), albedo, metalness);

	return lightDiffSpec(pos, ambient, normal, diffColor, specColor, glossiness, alpha, shadow);
}

vec4 lightPhysLinear(vec3 pos, float ambient, vec3 normal, vec3 albedo, float metalness, float glossiness,
		float alpha) {

	float shadow = compShadow(pos);
	return lightPhysLinear(pos, ambient, normal, albedo, metalness, glossiness, alpha, shadow);
}

vec4 lightPhys(vec3 pos, float ambient, vec3 normal, vec3 albedo, float metalness, float glossiness,
		float alpha, float shadow) {

	//metalness = 1.0 - pow(1.0 - metalness, 2.2);
	metalness = metalness * (2.0 - metalness);		// = 1.0 - (1.0 - metalness)^2

	float f1 = .875;
	float f2 = .6522;		// log(2048) / log(1000000)

	glossiness = glossiness <= f1 ? f2 * glossiness : f1 * f2 + (1.0 - f1 * f2) * (glossiness - f1) / (1.0 - f1);

	albedo = pow(albedo, vec3(2.2));

	return lightPhysLinear(pos, ambient, normal, albedo, metalness, glossiness, alpha, shadow);
}

vec4 lightPhys(vec3 pos, float ambient, vec3 normal, vec3 albedo, float metalness, float glossiness,
		float alpha) {

	float shadow = compShadow(pos);
	return lightPhys(pos, ambient, normal, albedo, metalness, glossiness, alpha, shadow);
}


vec3 normalToRgb(vec3 normal) {
	return vec3(view * vec4(normal, .0));
}

vec3 rgbToNormal(vec3 rgb) {
	return rgb;
}

void fragmentOutput(vec3 pos, float ambient, vec3 normal, vec3 albedo, float metalness, float glossiness,
		float alpha, float shadow) {

	color0 = applyFog(lightPhys(pos, ambient, normal, albedo, metalness, glossiness, alpha, shadow));
	color1 = vec4(normalToRgb(normal), 1.0);
}

void fragmentOutput(vec3 pos, float ambient, vec3 normal, vec3 albedo, float metalness, float glossiness,
		 float alpha) {

	float shadow = compShadow(pos);
	fragmentOutput(pos, ambient, normal, albedo, metalness, glossiness, alpha, shadow);
}

void fragmentOutputCustom(vec4 color, vec4 normal) {
	color0 = color;
	color1 = vec4(normalToRgb(normal.xyz), normal.w);
}

void fragmentOutputCustomColor(vec4 color) {
	color0 = color;
	color1 = vec4(.0);
}

void fragmentOutputCustomNormal(vec4 normal) {
	color0 = vec4(.0);
	color1 = vec4(normalToRgb(normal.xyz), normal.w);
}


vec4 getAlbedoAndGlossForLegacyMatCoeff(vec4 matCoeff, vec3 color) {
	float scale = mix(matCoeff.x, matCoeff.y, 7.0/12);		// amb 0.5, diff 0.7

	vec3 albedo = pow(color, vec3(2.2)) * scale;

	// glossiness = log(specExp) / log(1000000.0)
	// log(specExp) = glossiness * log(1000000.0)
	// specExp = pow(1000000.0, glossiness)

	//float glossiness = log(matCoeff.w) / log(1000000.0);
	float glossiness = log(mix(1.0, matCoeff.w, matCoeff.z)) / log(1000000.0);

	return vec4(pow(albedo, vec3(1.0 / 2.2)), glossiness);
}


//==============================================================


vec4 lightDiffSpecLightMap(vec3 pos, float ambient, vec3 normal, vec3 diffColor, vec3 specColor, float glossiness,
		float alpha, float brightness) {
	
	diffColor = clamp(diffColor, vec3(.0), vec3(1.0));
	specColor = clamp(specColor, vec3(.0), vec3(1.0));

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

	vec3 diffLight0 = getLight(lightTex0, normal, maxLod).rgb;
	
	vec3 specLight0 = getLight(lightTex0, reflDir, specLod).rgb;

	vec3 diffLight1 = getLight(lightTex1, normal, maxLod).rgb;
	vec3 specLight1 = getLight(lightTex1, reflDir, specLod).rgb;

	if (ambientScale > 1.0) {
		vec3 diffF = vec3(max(dot(normal, lightDir), .0));
		vec3 diffB = + vec3(max(dot(normal, vec3(0, 0, 1)), .0));
	
		float diffNorm = 1.0 / lightScale;
	
		diffLight0 = diffNorm * .25 * (1.0 + diffB);
		diffLight1 = diffNorm * 1.5 * ((diffLight0 + diffF));
		
		specLight0 = vec3(.1 / lightScale);
		specLight1 = vec3((1.0 / lightScale) * max(dot(reflDir, lightDir), .0));
	}

	//vec3 diffLight = mix(diffLight0, diffLight1, shadow);
	vec3 diffLight = vec3(ambientScale * ambient * brightness);// + 1.0 / ambientScale * mix(.35, 1.0, ambient) * shadow * (diffLight1 - diffLight0);
	vec3 specLight = mix(ambientScale * specLight0, 1.0 / ambientScale * specLight1, vec3(1.0));	// TODO ambient (depending on specExp)

	//return lightScale * (diffColor * diffLight + specColor * specLight);
	// assuming specColor.r = g = b
	float alpha2 = 1.0 - (1.0 - specColor.r) * (1.0 - alpha);

	return vec4(lightScale * (diffColor * diffLight + specColor * specLight), alpha2);
}

vec4 lightPhysLinearLightMap(vec3 pos, float ambient, vec3 normal, vec3 albedo, float metalness, float glossiness,
		float alpha, float brightness) {

	vec3 diffColor = mix(albedo, vec3(.0), metalness);
	//vec3 specColor = mix(vec3(.04), albedo, metalness);
	vec3 specColor = mix(vec3(mix(.01, .05, glossiness)), albedo, metalness);

	return lightDiffSpecLightMap(pos, ambient, normal, diffColor, specColor, glossiness, alpha, brightness);
}


vec4 lightPhysLightMap(vec3 pos, float ambient, vec3 normal, vec3 albedo, float metalness, float glossiness,
		float alpha, float brightness) {

	//metalness = 1.0 - pow(1.0 - metalness, 2.2);
	metalness = metalness * (2.0 - metalness);		// = 1.0 - (1.0 - metalness)^2

	float f1 = .875;
	float f2 = .6522;		// log(2048) / log(1000000)

	glossiness = glossiness <= f1 ? f2 * glossiness : f1 * f2 + (1.0 - f1 * f2) * (glossiness - f1) / (1.0 - f1);

	albedo = pow(albedo, vec3(2.2));

	return lightPhysLinearLightMap(pos, ambient, normal, albedo, metalness, glossiness, alpha, brightness);
}

void fragmentOutputLightMap(vec3 pos, float ambient, vec3 normal, vec3 albedo, float metalness, float glossiness,
		float alpha, float brightness) {

	color0 = applyFog(lightPhysLightMap(pos, ambient, normal, albedo, metalness, glossiness, alpha, brightness));
	color1 = vec4(normalToRgb(normal), 1.0);
}
