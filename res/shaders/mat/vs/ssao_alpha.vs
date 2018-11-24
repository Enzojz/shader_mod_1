#version 150

uniform mat4 proj;
uniform mat4 view;

uniform vec2 nearFar;

in vec4 attrPosition;
in vec3 attrNormal;
in vec2 attrTexCoord;
in mat4 instAttrModel;		// instAttrModel[3][3] = ambient

out vec4 normalDepth;
out vec2 texCoord;

void main() {
	vec4 posAmbient = instAttrModel * attrPosition;

	vec4 eyeCoords = view * vec4(posAmbient.xyz, 1.0);
	float depth = (-eyeCoords.z - nearFar.x) * nearFar.y;

	vec3 normal = normalize(vec3(instAttrModel * vec4(attrNormal, .0)));
	vec3 eyeNormal = vec3(view * vec4(normal, .0));

	gl_Position = proj * eyeCoords;
	normalDepth = vec4(eyeNormal, depth);
	texCoord = attrTexCoord;
}
