#version 150

uniform sampler2D permTex;
uniform sampler1D simplexTex;

#define ONE 0.00390625
#define ONEHALF 0.001953125
// The numbers above are 1/256 and 0.5/256, change accordingly
// if you change the code to use another texture size.

float snoise0(vec2 P) {
	// Skew and unskew factors are a bit hairy for 2D, so define them as constants
	#define F2 0.366025403784			// This is (sqrt(3.0)-1.0)/2.0
	#define G2 0.211324865405			// This is (3.0-sqrt(3.0))/6.0

	// Skew the (x,y) space to determine which cell of 2 simplices we're in
	float s = (P.x + P.y) * F2;		// Hairy factor for 2D skewing
	vec2 Pi = floor(P + s);
	float t = (Pi.x + Pi.y) * G2;	// Hairy factor for unskewing
	vec2 P0 = Pi - t;				// Unskew the cell origin back to (x,y) space
	Pi = Pi * ONE + ONEHALF;		// Integer part, scaled and offset for texture lookup

	vec2 Pf0 = P - P0;				// The x,y distances from the cell origin

	// For the 2D case, the simplex shape is an equilateral triangle.
	// Find out whether we are above or below the x=y diagonal to
	// determine which of the two triangles we're in.
	float dd = float(Pf0.x > Pf0.y);
	vec2 o1 = vec2(dd, 1.0-dd);

	// Noise contribution from simplex origin
	vec2 grad0 = texture(permTex, Pi).rg * 2.0 - 1.0;
	float t0 = max(0.5 - dot(Pf0, Pf0), 0.0);
	t0 *= t0;
	float n0 = t0 * t0 * dot(grad0, Pf0);

	// Noise contribution from middle corner
	vec2 Pf1 = Pf0 - o1 + G2;
	vec2 grad1 = texture(permTex, Pi + o1*ONE).rg * 2.0 - 1.0;
	float t1 = max(0.5 - dot(Pf1, Pf1), 0.0);
	t1 *= t1;
	float n1 = t1 * t1 * dot(grad1, Pf1);

	// Noise contribution from last corner
	vec2 Pf2 = Pf0 - vec2(1.0-2.0*G2);
	vec2 grad2 = texture(permTex, Pi + vec2(ONE, ONE)).rg * 2.0 - 1.0;
	float t2 = max(0.5 - dot(Pf2, Pf2), 0.0);
	t2 *= t2;
	float n2 = t2 * t2 * dot(grad2, Pf2);

	// Sum up and scale the result to cover the range [-1,1]
	return n0 + n1 + n2;
}

float snoise(vec2 P) {
	//return 70.0*snoise0(P);
	return 0.5 + 35.0*snoise0(P);
}

float ridged(vec2 P) {
	return 1.0 - 70.0*abs(snoise0(P));
}

float ridged2(vec2 P) {
	float n = 70.0*abs(snoise0(P));
	return mix(1.0-n, 1.0-n*n, pow(1.0-n, 8.0));
}

float snoise(vec3 P) {
	// The skewing and unskewing factors are much simpler for the 3D case
	#define F3 0.333333333333
	#define G3 0.166666666667

	// Skew the (x,y,z) space to determine which cell of 6 simplices we're in
	float s = (P.x + P.y + P.z) * F3; // Factor for 3D skewing
	vec3 Pi = floor(P + s);
	float t = (Pi.x + Pi.y + Pi.z) * G3;
	vec3 P0 = Pi - t; // Unskew the cell origin back to (x,y,z) space
	Pi = Pi * ONE + ONEHALF; // Integer part, scaled and offset for texture lookup

	vec3 Pf0 = P - P0;  // The x,y distances from the cell origin

	// For the 3D case, the simplex shape is a slightly irregular tetrahedron.
	// To find out which of the six possible tetrahedra we're in, we need to
	// determine the magnitude ordering of x, y and z components of Pf0.
	// The method below is explained briefly in the C code. It uses a small
	// 1D texture as a lookup table. The table is designed to work for both
	// 3D and 4D noise, so only 8 (only 6, actually) of the 64 indices are
	// used here.
	float c1 = (Pf0.x > Pf0.y) ? 0.5078125 : 0.0078125; // 1/2 + 1/128
	float c2 = (Pf0.x > Pf0.z) ? 0.25 : 0.0;
	float c3 = (Pf0.y > Pf0.z) ? 0.125 : 0.0;
	float sindex = c1 + c2 + c3;
	vec3 offsets = texture(simplexTex, sindex).rgb;
	vec3 o1 = step(0.375, offsets);
	vec3 o2 = step(0.125, offsets);

	// Noise contribution from simplex origin
	float perm0 = texture(permTex, Pi.xy).a;
	vec3  grad0 = texture(permTex, vec2(perm0, Pi.z)).rgb * 4.0 - 1.0;
	float t0 = max(0.6 - dot(Pf0, Pf0), 0.0);
	t0 *= t0;
	float n0 = t0 * t0 * dot(grad0, Pf0);

	// Noise contribution from second corner
	vec3 Pf1 = Pf0 - o1 + G3;
	float perm1 = texture(permTex, Pi.xy + o1.xy*ONE).a;
	vec3  grad1 = texture(permTex, vec2(perm1, Pi.z + o1.z*ONE)).rgb * 4.0 - 1.0;
	float t1 = max(0.6 - dot(Pf1, Pf1), 0.0);
	t1 *= t1;
	float n1 = t1 * t1 * dot(grad1, Pf1);

	// Noise contribution from third corner
	vec3 Pf2 = Pf0 - o2 + 2.0 * G3;
	float perm2 = texture(permTex, Pi.xy + o2.xy*ONE).a;
	vec3  grad2 = texture(permTex, vec2(perm2, Pi.z + o2.z*ONE)).rgb * 4.0 - 1.0;
	float t2 = max(0.6 - dot(Pf2, Pf2), 0.0);
	t2 *= t2;
	float n2 = t2 * t2 * dot(grad2, Pf2);

	// Noise contribution from last corner
	vec3 Pf3 = Pf0 - vec3(1.0-3.0*G3);
	float perm3 = texture(permTex, Pi.xy + vec2(ONE, ONE)).a;
	vec3  grad3 = texture(permTex, vec2(perm3, Pi.z + ONE)).rgb * 4.0 - 1.0;
	float t3 = max(0.6 - dot(Pf3, Pf3), 0.0);
	t3 *= t3;
	float n3 = t3 * t3 * dot(grad3, Pf3);

	// Sum up and scale the result to cover the range [-1,1]
	return 32.0 * (n0 + n1 + n2 + n3);
}

