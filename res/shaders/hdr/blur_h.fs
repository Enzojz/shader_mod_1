#version 150

uniform sampler2D tex;
uniform float ofs;

in vec2 texCoord;

out vec4 color;

void main() {
    //float weights[7] = float[7](0.006, 0.061, 0.242, 0.383, 0.242, 0.061, 0.006);
    float weights9[9] = float[9](1.0/256.0, 8.0/256.0, 28.0/256.0, 56.0/256.0, 70.0/256.0, 56.0/256.0, 28.0/256.0, 8.0/256.0, 1.0/256.0);

#if 1
    vec4 res = vec4(0.0);
	for (int i = 0; i < 9; ++i) {
		vec2 ofs2 = vec2(ofs*(float(i)-4.0), .0);
		vec4 col = texture(tex, texCoord + ofs2);
		res += col*weights9[i];
	}
	color = res;
#else
	color = texture(tex, texCoord);
#endif
}
