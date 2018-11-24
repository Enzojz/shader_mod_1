#version 150

uniform sampler2D normalMap;	// View space normal vectors and depth squared
uniform sampler2D rnm;			// Random vectors to randomize the sampling kernel

uniform vec2 normalMapRes1;

uniform int samples;
uniform vec3 randSphereVector[64];

uniform vec2 tcScale;

uniform vec2 nearPlaneHalfSize;
uniform vec2 nearFar; 

in vec2 texCoord;

out vec4 color;

vec2 matchPixelCenter(vec2 uv) {
	vec2 ij = floor(uv / normalMapRes1);
	return (vec2(.5) + ij) * normalMapRes1; 
}

vec3 depth2Position(vec2 uv, float depth) {
	vec2 xyOnNearPlane = (uv - .5) * 2.0 * nearPlaneHalfSize;
	vec3 xyzOnNearPlane = vec3(xyOnNearPlane.x, xyOnNearPlane.y, -nearFar.x);
	float scale = mix(nearFar.x, nearFar.y, depth) / nearFar.x;
	return xyzOnNearPlane * scale;
}

float sample (vec3 srcPosition, vec3 srcNormal, vec2 uv, float radius) {
	vec4 dstNormalAndDepth = texture(normalMap, uv);

	vec3 dstPosition = depth2Position(uv, dstNormalAndDepth.w * dstNormalAndDepth.w);	

	vec3 positionVec = dstPosition - srcPosition;
	float dist = length(positionVec);
	if (dist < .01) return .0;
	
	float intensity = max(dot(positionVec / dist, srcNormal) - .05, .0);
	
	float attenuation = max(1.0 - .25 * dist / radius, .0);
	
	return intensity * attenuation;
}

void main () {
	vec2 texCoordC = matchPixelCenter(texCoord);

	vec4 srcNormalAndDepth = texture(normalMap, texCoordC);

	vec3 srcPosition = depth2Position(texCoordC, srcNormalAndDepth.w * srcNormalAndDepth.w);
	vec3 srcNormal = 2.0 * srcNormalAndDepth.xyz - vec3(1.0);
	
	vec3 randVec = normalize(texture(rnm, texCoordC * tcScale).xyz * 2.0 - 1.0);
		
	float occlusion = 0.0;
	
	float minRadius = .75;
	float usualRadius = 1.25;	
	float radius = mix(minRadius, usualRadius, clamp(-srcPosition.z * .025, .0, 1.0)); 
	
	for (int i = 0; i < samples; ++i) {
		vec3 rsv = reflect(randSphereVector[i] * radius, randVec);
		
		vec3 samplePos = srcPosition + rsv;
		
		vec2 uv = .5 + .5 * (samplePos.xy / -samplePos.z * nearFar.x) / nearPlaneHalfSize;
		if (abs(uv.x - texCoord.x) < normalMapRes1.x && abs(uv.y - texCoord.y) < normalMapRes1.y) {
			continue;
		}
		
		uv = matchPixelCenter(uv);
		 
		occlusion += sample(srcPosition, srcNormal, uv, radius);
	}
	
	occlusion /= float(samples);		
	
	occlusion = clamp(3.0 * occlusion, 0.0, 1.0);
	
	color.r = 1.0 - occlusion;
}
