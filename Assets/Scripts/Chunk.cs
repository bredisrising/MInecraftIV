using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    Mesh mesh;

    MeshRenderer meshRenderer;
    MeshFilter meshFilter;

	//Vector3[] vertices;
	//int[] triangles;
	//Vector2[] uvs;

	List<Vector3> vertices = new List<Vector3>();
	List<int> triangles = new List<int>();
	List<Vector2> uvs = new List<Vector2>();

	bool[,,] voxelMap = new bool[VoxelData.chunkDim, VoxelData.chunkDim, VoxelData.chunkDim];

	private void Start()
	{
		meshRenderer = GetComponent<MeshRenderer>();
		meshFilter = GetComponent<MeshFilter>();

		buildChunk();

	}



	void buildChunk()
	{
		makeVoxelMap();

		//vertices = new Vector3[36 * VoxelData.chunkCubeDim * VoxelData.chunkCubeDim * VoxelData.chunkCubeDim];
		//triangles = new int[36 * VoxelData.chunkCubeDim * VoxelData.chunkCubeDim * VoxelData.chunkCubeDim];
		//uvs = new Vector2[36 * VoxelData.chunkCubeDim * VoxelData.chunkCubeDim * VoxelData.chunkCubeDim];

		for (int y = 0; y < VoxelData.chunkDim; y++)
		{
			for (int x = 0; x < VoxelData.chunkDim; x++)
			{
				for (int z = 0; z < VoxelData.chunkDim; z++)
				{
					applyVoxelData(new Vector3(x, y, z));
				}
			}
		}

		createMesh();
	}




	void makeVoxelMap()
	{
		for (int y = 0; y < VoxelData.chunkDim; y++)
		{
			for (int x = 0; x < VoxelData.chunkDim; x++)
			{
				for (int z = 0; z < VoxelData.chunkDim; z++)
				{
					voxelMap[x, y, z] = true;
				}
			}
		}
	}




	bool checkVoxelExists(Vector3 pos)
	{
		int x = (int)pos.x;
		int y = (int)pos.y;
		int z = (int)pos.z;

		if (x < 0 || x > VoxelData.chunkDim - 1 || y < 0 || y > VoxelData.chunkDim - 1 || z < 0 || z > VoxelData.chunkDim - 1)
			return false;

		return voxelMap[x, y, z];
	}



	void applyVoxelData (Vector3 pos)
	{
		// 36 vertices because uv mapping a cube
		// makes it so each face needs its own 4 vertices

		//triangles use the indices of the vertices
		//but the vertices are already in order
		//so using i is fine

		for (int face = 0; face < 6; face++)
		{

			if(!checkVoxelExists(pos + VoxelData.faceChecks[face])) // adds face checks to see if the face is facing a voxel or not
			{
				for (int vert = 0; vert < 6; vert++)
				{
					vertices.Add(VoxelData.voxelVertices[VoxelData.voxelTriangles[face * 6 + vert]] + pos);
					triangles.Add(vertices.Count - 1); //using i is fine
					uvs.Add(VoxelData.voxelUVs[vert]);
				}
			} 
			
		}
	}




	void createMesh()
	{

		mesh = new Mesh();
		

		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.uv = uvs.ToArray();

		mesh.RecalculateNormals();

		meshFilter.mesh = mesh;
	}

}
