#version 150

uniform mat4 projView;
uniform mat4 viewInverse;

uniform vec4 matCoeff;
uniform float pixelPerAngle;

in vec4 attrPosAlpha;
in vec2 attrColorSize;

out vec3 l0;
out float alpha;

//vec2 compAdsNoSpecular(vec3 normal, vec2 matCoeff, float ambient);
//vec3 light0NoSpecular(vec2 ads, vec3 color);
vec3 lightNew(vec3 pos, float ambient, vec3 normal, vec3 albedo, float metalness, float glossiness);
void calcFogFactor(vec3 pos);

void main() {
	vec3 vpos = attrPosAlpha.xyz;
	alpha = attrPosAlpha.w;
	
	vec3 color = vec3(attrColorSize.x);
	float size = attrColorSize.y;
	
    gl_Position = projView * vec4(vpos, 1.0);
	
	vec3 vertexToCam = viewInverse[3].xyz - vpos;
    
    gl_PointSize = size * pixelPerAngle / dot(vertexToCam, viewInverse[2].xyz);

	vec3 nrml = normalize(vertexToCam);

	// TODO shadowing does not work (because depth is not written). so we just use the lighted version.

	//vec2 ad = compAdsNoSpecular(nrml, matCoeff.xy, 1.0);
	//l0 = light0NoSpecular(ad, color);	

	l0 = lightNew(vpos, 1.0, nrml, color, .0, .0);
	l0 = mix(color, l0, .7);		// HACK brighten up

	calcFogFactor(vpos);
}
