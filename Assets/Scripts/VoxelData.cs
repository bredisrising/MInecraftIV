using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelData
{
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
        0, 2, 1, 1, 2, 4, //front face
        6, 7, 5, 5, 3, 6, //bak face
        4, 2, 5, 5, 7, 4, //top feces
        1, 6, 3, 3, 0, 1, //bottom feces
        1, 4, 7, 7, 6, 1, //right feces
        3, 5, 2, 2, 0, 3, //left face

    };
}
