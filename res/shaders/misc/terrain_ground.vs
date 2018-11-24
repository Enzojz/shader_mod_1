#version 150

uniform mat4 projView;

uniform vec2 terrainScale;
uniform vec2 terrainOffset;
uniform vec2 colorScale;

in vec4 attrPosition;
in vec2 attrTexCoord;
in vec2 attrTexCoord2;

out vec2 texCoord0;
out vec2 texCoord1;
out vec2 texCoord2;
out vec2 texCoord3;

void main() {
	//gl_Position = projView * (attrPosition + vec4(terrainOffset, vec2(.0)));
	gl_Position = projView * attrPosition;

	texCoord0 = (attrPosition.xy - terrainOffset) * terrainScale;
	texCoord1 = attrTexCoord;
	texCoord2 = attrPosition.xy * colorScale;
	texCoord3 = attrTexCoord2;
}
