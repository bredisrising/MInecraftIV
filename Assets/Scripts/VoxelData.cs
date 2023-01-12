using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
public class VoxelData
{
    public static readonly int chunkDim = 16;
	public static readonly int chunkDim2 = chunkDim * chunkDim;
	public static readonly int chunkDim3 = chunkDim * chunkDim * chunkDim;

	public static readonly int verticesInCube = 36;

    public static readonly int worldSizeInChonksXZ = 100;
    public static readonly int worldSizeInChonksY = 40; 

    

    public static readonly int worldSizeInBlocksY = worldSizeInChonksY * chunkDim;
    public static readonly int worldSizeInBlocksXZ = worldSizeInChonksXZ * chunkDim;


    
    public static readonly float normalizedAtlasBlockSizeY = 1f / 5; //right now we only have 5 blocks in the atlas but we need to make this dynamic
    public static readonly float normalizedAtlasBlockSizeX = 1f / 6; //6 for 6 faces


    public static readonly int viewDistanceInChonksXZ = 2;
    public static readonly int viewDistanceInChonksY = 2;
	public static readonly int cubicWorldSize = viewDistanceInChonksXZ * viewDistanceInChonksXZ * viewDistanceInChonksY;


	public static readonly Vector3[] voxelVertices = new Vector3[8]
    {
        new Vector3(0,0,0), 
        new Vector3(1,0,0),
        new Vector3(0,1,0),
        new Vector3(0,0,1),
        new Vector3(1,1,0),
        new Vector3(0,1,1),
        new Vector3(1,0,1),
        new Vector3(1,1,1),

    };

    public static readonly int[] voxelTrianglesWithDupes = new int[36]
    {
        0, 2, 4, 4, 1, 0, //front face
        6, 7, 5, 5, 3, 6, //bak face
        2, 5, 7, 7, 4, 2, //top feces
        1, 6, 3, 3, 0, 1, //bottom feces
        1, 4, 7, 7, 6, 1, //right feces
        3, 5, 2, 2, 0, 3, //left face

    };

    public static readonly int[] voxelTriangles = new int[24]
    {
        0, 2, 4, 1, //front face
        6, 7, 5, 3, //bak face
        2, 5, 7, 4, //top feces
        1, 6, 3, 0, //bottom feces
        1, 4, 7, 6, //right feces
        3, 5, 2, 0, //left face

    };

    public static readonly Vector3[] faceChecks = new Vector3[6]
    {
        //corresponds with order of voxel triangle faces above
        new Vector3(0,0,-1),
        new Vector3(0,0,1),
        new Vector3(0,1,0),
        new Vector3(0,-1,0),
        new Vector3(1,0,0),
        new Vector3(-1,0,0),
    };

    public static readonly Vector2[] voxelUVs = new Vector2[4]
	{
        //matches with the x order of voxel triangles

        new Vector2(0f, 0f),
        new Vector2(0f, 1f),
        new Vector2(1f, 1f),
        new Vector2(1f, 0f),
    };

    

}
