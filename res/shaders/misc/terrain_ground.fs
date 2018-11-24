#version 150

uniform sampler2D detailTex;
uniform sampler2D detailNrmlTex;
//uniform sampler2D noiseTex;

uniform sampler2D heightmapTex;
uniform vec2 tileSize;
uniform vec2 resolution;
uniform float texel;

in vec2 texCoord0;
in vec2 texCoord1;

out vec4 color0;
out vec4 color1;

vec3 decodeNormal(vec2 color);
vec3 nrmlAmb2rgb(vec4 nrmlAmb);
float getHeightNormalSlope(sampler2D heightmapTex, vec2 tileSize, vec2 resolution, float texel, vec2 tc,
		out mat3 tangentMat, out float slope);

void main() {
	if (any(lessThan(texCoord0, vec2(.0, .0))) || any(greaterThan(texCoord0, vec2(1.0, 1.0)))) discard;

	mat3 tangentMat;
	float slope;
	float height = getHeightNormalSlope(heightmapTex, tileSize, resolution, texel, texCoord0, tangentMat, slope);

	vec4 col = texture(detailTex, texCoord1);
	vec3 nrml = decodeNormal(texture(detailNrmlTex, texCoord1).rg);

	// adapt the color to the grass which is darker
	// TODO remove this! adapt textures or grass
	//col *= 0.8;	

#if 0
	float n = texture(noiseTex, 1.7 * texCoord1).r;
	col.a = min(pow(1.5 * n * col.a + col.a, 2.0), 1.0);
#endif

	vec2 u = normalize(dFdx(texCoord1));
	//vec2 v = normalize(dFdy(texCoord1));
	vec2 v = vec2(-u.y, u.x);
	nrml.xy = transpose(mat2(u, v)) * nrml.xy;

	color0 = col;
	color1 = vec4(nrmlAmb2rgb(vec4(tangentMat * nrml, 1.0)), col.a);
}
