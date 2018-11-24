#version 150

uniform sampler2D tex;

in vec4 texCoord;

out vec4 color;

void main() {
	float val = texture(tex, texCoord.xy).r * texCoord.z;

	color = vec4(val, .0, .0, .0); 			// height value
	
	gl_FragDepth = 1.0 - val;
}
