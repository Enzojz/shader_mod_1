// -- MIT License

// -- Copyright (c) 2019 Enzojz

// -- Permission is hereby granted, free of charge, to any person obtaining a copy
// -- of this software and associated documentation files (the "Software"), to deal
// -- in the Software without restriction, including without limitation the rights
// -- to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// -- copies of the Software, and to permit persons to whom the Software is
// -- furnished to do so, subject to the following conditions:

// -- The above copyright notice and this permission notice shall be included in all
// -- copies or substantial portions of the Software.

// -- THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// -- IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// -- FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// -- AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// -- LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// -- OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// -- SOFTWARE.

#version 150

uniform mat4 viewInverse;

uniform sampler2D albedoTex;
uniform sampler2D metalGlossAoTex;

uniform sampler2D opTex1;
uniform ivec2 opSettings1;
uniform vec2 opScale1;
uniform float opOpacity1;

uniform sampler2D opTex2;
uniform ivec2 opSettings2;
uniform vec2 opScale2;
uniform float opOpacity2;

uniform bool flipNormal;

in vec4 posAmbient;
centroid in vec3 normal;
in vec3 texCoordAlpha;

out vec4 color;

vec3 lightPhysSsao(vec3 pos, float ambient, vec3 normal, vec3 albedo, float metalness, float glossiness);
vec3 lightPhysSsaoLightmap(vec3 pos, float ambient, vec3 normal, vec3 albedo, float metalness, float glossiness, float brightness);
vec3 applyFog(vec3 color);
vec3 toneMap(vec3 color);
vec3 applyOp(vec3 pos, vec3 nrml, vec2 texCoord, sampler2D tex, ivec2 settings, vec2 scale, float opacity, vec3 color);

// These four values means use the mod shader, since they are negative and lower than -1, almost impossible to have impact on non-concerned materials
bool isNonTrival(){ return any(equal(vec4(opOpacity1), vec4(-128.0, -256.0, -512.0, -1024.0))); }

// Check if the UV coordinate should be overwritten
vec2 uvPos()
{ 
	if (opOpacity1 <= -512.0) // Normal-Reference-Axis
	{
		vec3 nrml = normalize(normal);
		vec3 axis = normalize(texture(opTex1, vec2(0.25, 0.5)).xyz * (step(0.5, texture(opTex1, vec2(0.75, 0.5)).xyz) - 0.5));
		vec3 xDir = cross(nrml, axis);
		vec3 yDir = cross(nrml, xDir);
		return vec2(dot(posAmbient.xyz, xDir), dot(posAmbient.xyz, yDir)) * opScale1;
	}
	else if (opOpacity1 <= -256.0) // World xyz
	{
		vec3 checkValueU = texture(opTex1, vec2(0.25, 0.5)).xyz;
		vec3 checkValueV = texture(opTex1, vec2(0.75, 0.5)).xyz;

		return vec2(
			dot(step(0.75, checkValueU), posAmbient.xyz) + dot(1.0 - step(0.25, checkValueU), texCoordAlpha.xyz),
			dot(step(0.75, checkValueV), posAmbient.xyz) + dot(1.0 - step(0.25, checkValueV), texCoordAlpha.xyz)
		) * opScale1;
	} else if (opOpacity1 <= -128.0) // UV
		return texCoordAlpha.xy * opScale1;
	else // Non trival
		return texCoordAlpha.xy;
}

void main() {
	vec2 uv = uvPos();

	vec3 nrml = normal;
	if (!gl_FrontFacing && flipNormal) nrml = -nrml;

	vec3 albedo = texture(albedoTex, uv).rgb;

	vec3 metalGlossAo = texture(metalGlossAoTex, uv).rgb;

	float ambient = min(posAmbient.w, metalGlossAo.b);

	if (isNonTrival())
	{
		albedo = applyOp(posAmbient.xyz, normal, uv, opTex2, opSettings2, opScale2, opOpacity2, albedo);

		vec3 rawColor = lightPhysSsaoLightmap(posAmbient.xyz, ambient, nrml, albedo, metalGlossAo.r, metalGlossAo.g, 1.0);
		color.rgb = toneMap(applyFog(rawColor));
	}
	else 
	{
		if (opOpacity1 > -128)
			albedo = applyOp(posAmbient.xyz, normal, uv, opTex1, opSettings1, opScale1, opOpacity1, albedo);
		albedo = applyOp(posAmbient.xyz, normal, uv, opTex2, opSettings2, opScale2, opOpacity2, albedo);

		color.rgb = toneMap(applyFog(lightPhysSsao(posAmbient.xyz, ambient, nrml, albedo, metalGlossAo.r, metalGlossAo.g)));
	}
	
	color.a = texCoordAlpha.z;
}