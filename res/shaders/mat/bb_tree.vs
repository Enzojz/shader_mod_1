#version 150

struct Billboard {
	vec4 texCoords[7];
	vec4 dimFadeIn;
	vec4 coordsOH;
	float heightOH;
};
uniform Billboard billboards[1];

in vec4 attrPositionIdx;
in vec3 attrNormal;

in mat4 instAttrModel;
in vec4 instAttrColor;

out VertexData {
	vec3 position;
	vec4 normalScale;
	flat int index;

	vec4 instColor;

	vec4 texCoords[7];
	vec4 dimFadeIn;
	vec4 coordsOH;
	float heightOH;
} outData;

void main() {
	vec3 nrml = vec3(instAttrModel * vec4(attrNormal, .0));

	outData.position = vec3(instAttrModel * vec4(attrPositionIdx.xyz, 1.0));
	outData.normalScale = vec4(normalize(nrml), length(nrml));
	outData.index = 0;

	outData.instColor = instAttrColor;

	//outData.texCoords = billboards[0].texCoords;
	outData.texCoords[0] = billboards[0].texCoords[0];
	outData.texCoords[1] = billboards[0].texCoords[1];
	outData.texCoords[2] = billboards[0].texCoords[2];
	outData.texCoords[3] = billboards[0].texCoords[3];
	outData.texCoords[4] = billboards[0].texCoords[4];
	outData.texCoords[5] = billboards[0].texCoords[5];
	outData.texCoords[6] = billboards[0].texCoords[6];
	outData.dimFadeIn = billboards[0].dimFadeIn;
	outData.coordsOH = billboards[0].coordsOH;
	outData.heightOH = billboards[0].heightOH;
}
