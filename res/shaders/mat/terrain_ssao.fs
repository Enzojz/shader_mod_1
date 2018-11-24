#version 150

uniform mat4 view;

uniform sampler2D colorTex;
uniform sampler2D nrmlTex;

in vec3 texCoord;

out vec4 color;

vec4 rgb2nrmlAmb(vec3 rgb);

void main() {
	vec3 col = texture(colorTex, texCoord.xy).rgb;
	
	if (col.x > 0.9 && col.y < 0.1 && col.z > 0.9) {
		discard; 
	}
	
	vec4 nrmlAmb = rgb2nrmlAmb(texture(nrmlTex, texCoord.xy).rgb);

	color = vec4(vec3(.5) + .5 * vec3(view * vec4(nrmlAmb.xyz, .0)), sqrt(texCoord.z));		
}
