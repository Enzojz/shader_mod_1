#version 150

vec3 permute(vec3 x) {
	return mod(((x*34.0)+1.0)*x, 289.0);
}

float snoise(vec2 v) {
 	const vec4 C = vec4(
 			0.211324865405187, // (3.0-sqrt(3.0))/6.0
 			0.366025403784439, // 0.5*(sqrt(3.0)-1.0)
 			-0.577350269189626, // -1.0 + 2.0 * C.x
 			0.024390243902439); // 1.0 / 41.0
 			
 	// First corner
 	vec2 i = floor(v + dot(v, C.yy) );
 	vec2 x0 = v - i + dot(i, C.xx);

	// Other corners
	vec2 i1;
	//i1.x = step( x0.y, x0.x ); // x0.x > x0.y ? 1.0 : 0.0
	//i1.y = 1.0 - i1.x;
	i1 = (x0.x > x0.y) ? vec2(1.0, 0.0) : vec2(0.0, 1.0);
	// x0 = x0 - 0.0 + 0.0 * C.xx ;
	// x1 = x0 - i1 + 1.0 * C.xx ;
	// x2 = x0 - 1.0 + 2.0 * C.xx ;
	vec4 x12 = x0.xyxy + C.xxzz;
	x12.xy -= i1;

	// Permutations
	i = mod(i, 289.0); // Avoid truncation effects in permutation
	vec3 p = permute( permute( i.y + vec3(0.0, i1.y, 1.0 )) + i.x + vec3(0.0, i1.x, 1.0 ));

	vec3 m = max(0.5 - vec3(dot(x0,x0), dot(x12.xy,x12.xy), dot(x12.zw,x12.zw)), 0.0);
	m = m*m;
	m = m*m;

	// Gradients: 41 points uniformly over a line, mapped onto a diamond.
	// The ring size 17*17 = 289 is close to a multiple of 41 (41*7 = 287)
	vec3 x = 2.0 * fract(p * C.www) - 1.0;
	vec3 h = abs(x) - 0.5;
	vec3 ox = floor(x + 0.5);
	vec3 a0 = x - ox;
	
	// Normalise gradients implicitly by scaling m
	// Inlined for speed: m *= taylorInvSqrt( a0*a0 + h*h );
	m *= 1.79284291400159 - 0.85373472095314 * ( a0*a0 + h*h );

	// Compute final noise value at P
	vec3 g;
	g.x = a0.x * x0.x + h.x * x0.y;
	g.yz = a0.yz * x12.xz + h.yz * x12.yw;
	return 130.0 * dot(m, g);
}

vec3 nrmlAmb2rgb(vec4 nrmlAmb) {
    vec3 result;
    result.r = .5 + .5 * nrmlAmb.x;
    result.g = nrmlAmb.w;
    result.b = .5 + .5 * nrmlAmb.y;
	return result;        
}

vec4 rgb2nrmlAmb(vec3 rgb) {
	vec4 result;
	result.x = (2.0 * rgb.r - 1.0) * .9375;		// .9375 = 1.0 - 1 / 16.0 factor fixes issues with 16-bit rgb textures
	result.y = (2.0 * rgb.b - 1.0) * .9375;
	result.z = sqrt(max(1.0 - result.x * result.x - result.y * result.y, .0));
	result.w = rgb.g;
	return result;
}

float getHeightNormalSlope(sampler2D heightmapTex, vec2 tileSize, vec2 resolution, float texel, vec2 tc,
		out mat3 tangentMat, out float slope) {
	const float hmax = 3276.75;

	//vec2 texel = vec2(1.0) / textureSize(heightmapTex, 0);

	vec2 posCnt = tileSize * tc;
	vec2 posBL = posCnt - .5 * resolution;
	vec2 posTR = posCnt + .5 * resolution;

	vec2 tcCnt = (1.0 - 3.0 * texel) * tc + vec2(1.5 * texel);
	vec2 tcBL = (1.0 - 3.0 * texel) * posBL / tileSize + vec2(1.5 * texel);
	vec2 tcTR = (1.0 - 3.0 * texel) * posTR / tileSize + vec2(1.5 * texel);

	float hl = hmax * texture(heightmapTex, vec2(tcBL.x, tcCnt.y)).r;
	float hr = hmax * texture(heightmapTex, vec2(tcTR.x, tcCnt.y)).r;
	float hb = hmax * texture(heightmapTex, vec2(tcCnt.x, tcBL.y)).r;
	float ht = hmax * texture(heightmapTex, vec2(tcCnt.x, tcTR.y)).r;

	float height = .25 * (hl + hr + hb + ht);

	vec3 normal = normalize(vec3((hl - hr) * resolution.y, (hb - ht) * resolution.x, resolution.x * resolution.y));
	//vec3 tangent = normalize(vec3(resolution.x, .0, hr - hl));
	vec3 tangent = normalize(vec3(normal.z, .0, -normal.x));
	vec3 bitangent = cross(normal, tangent);

	tangentMat = mat3(tangent, bitangent, normal);	

	slope = sqrt(1.0 - normal.z * normal.z) / normal.z;

	return height;
}
