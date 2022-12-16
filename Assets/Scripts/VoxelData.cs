using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelData
{
    public static readonly int chunkDim = 30;

    public static readonly int verticesInCube = 36;

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

    public static readonly int[] voxelTriangles = new int[36]
    {
        0, 2, 4, 4, 1, 0, //front face
        6, 7, 5, 5, 3, 6, //bak face
        2, 5, 7, 7, 4, 2, //top feces
        1, 6, 3, 3, 0, 1, //bottom feces
        1, 4, 7, 7, 6, 1, //right feces
        3, 5, 2, 2, 0, 3, //left face

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

    public static readonly Vector2[] voxelUVs = new Vector2[6]
	{
        new Vector2(0f, 0f),
        new Vector2(0f, 1f),
        new Vector2(1f, 1f),
        new Vector2(1f, 1f),
        new Vector2(1f, 0f),
        new Vector2(0f, 0f),
    };

    

}
