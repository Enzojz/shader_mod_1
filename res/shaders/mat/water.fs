#version 150

uniform mat4 viewInverse;

uniform float speed;

/*uniform vec3 lightDir;
uniform vec3 lightAmb;
uniform vec3 lightDiff;
uniform vec3 lightSpec;

uniform vec4 matCoeff;*/

uniform vec2 fragCoordScale;
uniform vec2 fragCoordOfs;
uniform sampler2D reflTex;
uniform sampler2D noiseTex;

in vec4 posAmbient;
//centroid in vec3 normal;
in vec2 texCoord;

in float fogFactor;

out vec4 color;

//float compShadow(vec3 vpos);
vec3 applyFog(vec3 color);
vec3 toneMap(vec3 color);
vec3 unToneMap(vec3 y);

vec2 rotate(vec2 pos, float angle) {
	float c = cos(angle);
	float s = sin(angle);
	return vec2(c*pos.x - s*pos.y, s*pos.x + c*pos.y);
}

float compDelta(vec2 pos) {
	const float PI_THIRD = 1.04719755;
	
	float result = .0;
	
	float s = 1.0;	
	for (int i = 0; i < 6; ++i) {	
		vec2 tc = pos / (20.0 * s);
		tc += rotate(vec2(speed, .0), PI_THIRD * i);
		result += s * texture(noiseTex, tc).r;
		
		s *= 1.5;
	}	
	
	return result * .2;
}

vec3 compDelta(vec3 pos) {
	float delta = compDelta(pos.xy);
	return vec3(pos.x, pos.y, pos.z + delta); 
}

void main() {
	//const vec3 waterCol = vec3(.11, .23, .22);
	const vec3 waterCol = pow(vec3(.091, .188, .170), vec3(2.2));

	vec3 vpos = posAmbient.xyz;

	float depth = texCoord.y / (texCoord.y + 2.5);

	vec3 viewVec = viewInverse[3].xyz - vpos;
	float len1 = inversesqrt(dot(viewVec, viewVec));
	vec3 viewDir = viewVec * len1;

	vec3 p = compDelta(vpos);
	
	vec3 px = compDelta(vpos + vec3(.1, .0, .0));
	vec3 py = compDelta(vpos + vec3(.0, .1, .0));
	
	vec3 dx = px - p;
	vec3 dy = py - p;
	
	vec3 normal = normalize(cross(dx, dy));
	normal = mix(vec3(.0, .0, 1.0), normal, .2 + .8 * depth);

#if 0
	float cosAlpha = dot(viewDir, normal);

	vec2 ofs = 0.07*normal.xy;

	vec3 halfAngle = normalize(viewDir + lightDir);

	vec3 waterAmb = waterCol * matCoeff.x * lightAmb;
	vec3 waterDiff = waterCol * matCoeff.y * lightDiff;
	vec3 waterSpec = .75 * len1 * matCoeff.z * pow(max(dot(normal, halfAngle), .0), matCoeff.w) * lightSpec;

	vec3 reflCol = texture(reflTex, gl_FragCoord.xy*fragCoordScale + fragCoordOfs + ofs).rgb;
	reflCol = mix(waterCol, reflCol, min(500.0 * len1, 1.0));

	float maxTransp = depth;
	//maxTransp = .05 * texCoord.y * maxTransp;
	//maxTransp = .5 * sqrt(texCoord.y) * maxTransp;		// TODO store sqrt in texCoord
	maxTransp = .15 * pow(texCoord.y, .8) * maxTransp;		// TODO pow

	vec4 col0 = mix(vec4(reflCol, 1.0), vec4(waterAmb, maxTransp), cosAlpha);
	vec4 col1 = mix(vec4(reflCol + waterSpec, 1.0), vec4(waterAmb + waterDiff + waterSpec, maxTransp), cosAlpha);

	float shadow = compShadow(vpos);
	
	vec4 scol = mix(col0, col1, shadow);
	
	// TODO reflCol is already tone-mapped!
	
	color = vec4(toneMap(applyFog(scol.rgb)), scol.w);
#else
	vec2 ofs = 0.13 * normal.xy;
	vec3 reflCol = texture(reflTex, gl_FragCoord.xy*fragCoordScale + fragCoordOfs + ofs).rgb;
	reflCol = min(reflCol, vec3(.999));
	reflCol = unToneMap(reflCol);

	vec3 reflDir = normalize(reflect(-viewVec, normal));
	float nDotR = max(dot(normal, reflDir), .0);
	float specRefl = .04 + .96 * pow(1.0 - nDotR, 5.0);

	float maxTransp = depth;

	vec3 col = specRefl * reflCol + maxTransp * waterCol;
	float alpha = 1.0 - (1.0 - maxTransp) * (1.0 - specRefl);

	//color = vec4(toneMap(applyFog(col)), alpha);
	color = vec4(toneMap(applyFog(col)), alpha) * (1.0 - fogFactor);
#endif
}
