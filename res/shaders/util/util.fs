#version 150

/*vec3 decodeNormal(vec3 color) {
	return normalize(2.0 * (color - vec3(128.0/255.0)));
}*/

vec3 decodeNormal(vec2 color) {
	color = 255.0 / 127.0 * (color - vec2(128.0 / 255.0));

	//return vec3(color, sqrt(1.0 - dot(color, color)));
	return vec3(color, sqrt(1.0 - min(dot(color, color), 1.0)));
}

vec3 decodeNormalScale(vec2 color, float scale) {
	vec3 nrml = decodeNormal(color);
	return normalize(vec3(scale * nrml.xy, nrml.z));
}

float desaturate(vec3 color) {
	//return dot(vec3(.3, .59, .11), color);
	return .5 * (min(color.r, min(color.g, color.b)) + max(color.r, max(color.g, color.b)));
}

vec3 overlay(vec3 col0, vec3 col1) {
	//float gray = desaturate(col0);
	float gray = dot(vec3(0.3, 0.59, 0.11), col0);
	
	if (gray < .5) return 2.0 * col1 * col0;
	return 1.0 - 2.0 * (1.0 - col1) * (1.0 - col0);
}

vec3 overlay(vec4 col0, vec3 col1) {
	return mix(col0.rgb, overlay(col0.rgb, col1), col0.a);
}

float overlay(float col0, float col1) {
	if (col0 < .5) return 2.0 * col1 * col0;
	return 1.0 - 2.0 * (1.0 - col1) * (1.0 - col0);
}

vec3 colorBlend(vec3 color, float mask, float scale, vec3 newColor) {
	if (newColor.r < .0) {
		return color;
	}
	
	//vec3 gray = vec3(scale * desaturate(color));
	vec3 gray = mix(vec3(scale * desaturate(color)), color, mask);
	return mix(clamp(overlay(newColor, gray), vec3(.0), vec3(1.0)), color, mask);
}

vec3 applyOp(vec3 pos, vec3 nrml, vec2 texCoord, sampler2D tex, ivec2 settings, vec2 scale, float opacity, vec3 color) {
	if (settings.x == 0) {					// Op::NO_OP
		return color;
	}

	vec2 tc;

	if (settings.y == 0) {					// Mode::TEXCOORD
		tc = texCoord;
	} else if (settings.y == 1) {			// Mode::WORLD_XY
		tc = pos.xy;
	} else /*if (settings.y == 2)*/ {		// Mode::NORMAL
		vec3 xDir = normalize(cross(vec3(.1411, .0, .99), nrml));
		vec3 yDir = cross(nrml, xDir);
		tc = vec2(dot(pos, xDir), dot(pos, yDir));
	}

	vec3 opCol = texture(tex, scale * tc).rgb;

	if (settings.x == 1) {					// Op::MULTIPLICATION
		return color * mix(vec3(1.0), opCol, opacity);
	}
	
	if (settings.x == 2) {					// Op::OVERLAY
		return overlay(color, mix(vec3(.5), opCol, opacity));
	}
	
	// settings.x == 3						// Op::LINEAR_BURN
	return color - (vec3(1.0) - mix(vec3(1.0), opCol, opacity));
}
