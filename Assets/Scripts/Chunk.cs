using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    Mesh mesh;

    [SerializeField] int xSize = 20;
    [SerializeField] int zSize = 20;

    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        createShape();
    }

    void createShape()
    {
        mesh.Clear();
        mesh.vertices = VoxelData.voxelVertices;
        mesh.triangles = VoxelData.voxelTriangles;

        

        mesh.RecalculateNormals();
    }

}
