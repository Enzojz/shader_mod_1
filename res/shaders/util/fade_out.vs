#version 150

uniform vec2 fadeOutDist;			// end, 1.0 / (end - start)

float compFadeOutAlpha(float distance) {
	
	// the 2.0 scale allows for using this function for transitions between two objects
	// so that always one of the two objects is 100% opaque during interpolation
	// note that alpha to coverage can not really blend two objects with 50% each
	
	return min(2.0 * (fadeOutDist.x - distance) * fadeOutDist.y, 1.0);
}

float compFadeOutAlpha(vec3 vpos, vec3 campos) {
	return compFadeOutAlpha(distance(vpos, campos));
}
