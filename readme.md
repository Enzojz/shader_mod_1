This mod helps you archive some effects that not implemented by the original shader

# New effects:
1. Shadow free
2. Lightmap
3. Enhanced UV projection
   
All effects are implemented with following 4 materials
PHYSICAL_OP
PHYSICAL_NRML_MAP_OP
PHYSICAL_NRML_MAP_UV1_AO
PHYSICAL_NRML_MAP_UV1_AO_OP

The game treats PHYSICAL_NRML_MAP_UV1_AO and PHYSICAL_NRML_MAP_UV1_AO_OP in exactly the same way so you can choose any, there's no difference.

You need to have UV1 channel in the model if you choose PHYSICAL_NRML_MAP_UV1_AO or PHYSICAL_NRML_MAP_UV1_AO_OP

# General use condition
   You need to use "operation_1" parameter to make the effects work, like following

	map_op_1 = 
	{
		fileName = "addictive information.dds",
		magFilter = "LINEAR",
		minFilter = "LINEAR_MIPMAP_LINEAR",
	},
	operation_1 = 
	{
		op = "MULTIPLICATION",
		mode = "NORMAL",
		scale = { 1.0, 1.0 },
		opacity = -128.0
	}
   When the effect is enabled, map_op_1 will not be used in render but rather used for addictive information

# Shadow free 
   specify "ocpacity" as any value among -128, -256, -512 and -1024, you will get shadow free effect.
   If you don't want to reproject your UVs, please specify -128.

# Lightmap
This effect is only implemented on PHYSICAL_NRML_MAP_UV1_AO or PHYSICAL_NRML_MAP_UV1_AO_OP
The lightmap is on the G channel of the UV1 map of the material
The shadow free effect is mandatory when lightmap is used

# New UV projections
Three kind of UV projections are implemented with this shader mod, they will effect the total of UV0, that means works all on albedo, metal_gloss_ao and normals textures.

Linear algebra knwoladges is mandatory to understand the following contents. 

1. If opacity ∈ [-128, -255], the UV0 is not changed
2. If opacity ∈ [-256, -511], the world coordinate XYZ may be applied to the projects.
You need to use the map_op_1 to specify how the remap is done.
In the map_op_1, the left half is used to specify the U coordinate, the right half is used to specify the V coordinate
The three channels RGB corresponds to XYZ
When the part is on white, that means the projection is on, when the part is on black, that means not used.
Examples: 
 - Left = Red, Right = Green -> XY -> UV
 - Left = Green, Right = Blue -> YZ -> UV
 
2. If opacity ∈ [-512, -1023], the projection is on the tangent surface
You need to use the map_op_1 to specify the direction of negative U
The left part RGB is the XYZ coordinat in the world space, the right part is the polarity, black means negative and white means positive.
This retrived value will be normalized then cross multiplied with the normal vector, to get the U vector, then V direction will be obtained by cross multiply between the normal vector and U vector.
For example:
 - (0, 0, 255) on the left, and black on the right means (0, 0, -1), if the normal is directed to Y direction (0, 1, 0), then you will get a U towards up (0, 0, 1) and V towards position X (1, 0, 0).
 
4. If opacity ≤ -1024, the mapping will be done as tangent as U, and bi-tangent (bi-normal) as V. This effect is not supported by PHYSICAL_OP.

For 2, 3, 4, change of the scale paramter will let the projection being fit.
