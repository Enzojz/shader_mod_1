#version 150

uniform sampler2D detailTex;
uniform sampler2D detailNrmlTex;
uniform sampler2D colorTex;
uniform sampler2D alphaTex;

uniform sampler2D heightmapTex;
uniform vec2 tileSize;
uniform vec2 resolution;
uniform float texel;

in vec2 texCoord0;
in vec2 texCoord1;
in vec2 texCoord2;
in vec2 texCoord3;

out vec4 color0;
out vec4 color1;

vec3 decodeNormal(vec2 color);
vec3 overlay(vec3 col0, vec3 col1);
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
	vec3 colorCol = texture(colorTex, texCoord2).rgb;

	col.rgb = overlay(col.rgb, colorCol);
	
	vec4 alpha = texture(alphaTex, texCoord3); // TODO use second texture coordinate
	col.a = alpha.a;

	vec2 u = normalize(dFdx(texCoord1));
	//vec2 v = normalize(dFdy(texCoord1));
	vec2 v = vec2(-u.y, u.x);
	nrml.xy = transpose(mat2(u, v)) * nrml.xy;
	
	color0 = col;
	color1 = vec4(nrmlAmb2rgb(vec4(tangentMat * nrml, 1.0)), col.a);
}
