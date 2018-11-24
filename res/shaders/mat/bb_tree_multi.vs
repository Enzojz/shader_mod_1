#version 150

struct Billboard {
	vec4 texCoords[7];
	vec4 dimFadeIn;
	vec4 coordsOH;
	float heightOH;
};
uniform Billboard billboards[16];

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
	int idx = int(attrPositionIdx.w);

	outData.position = vec3(instAttrModel * vec4(attrPositionIdx.xyz, 1.0));
	outData.normalScale = vec4(normalize(nrml), length(nrml));
	outData.index = idx;

	outData.instColor = instAttrColor;

	//outData.texCoords = billboards[idx].texCoords;
	outData.texCoords[0] = billboards[idx].texCoords[0];
	outData.texCoords[1] = billboards[idx].texCoords[1];
	outData.texCoords[2] = billboards[idx].texCoords[2];
	outData.texCoords[3] = billboards[idx].texCoords[3];
	outData.texCoords[4] = billboards[idx].texCoords[4];
	outData.texCoords[5] = billboards[idx].texCoords[5];
	outData.texCoords[6] = billboards[idx].texCoords[6];
	outData.dimFadeIn = billboards[idx].dimFadeIn;
	outData.coordsOH = billboards[idx].coordsOH;
	outData.heightOH = billboards[idx].heightOH;
}
