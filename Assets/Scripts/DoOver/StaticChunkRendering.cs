using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Mathematics;


public class StaticChunkRendering : MonoBehaviour
{
	public static GameObject GenerateChunk(int[] blockMap, int3 position, Material mat, int index)
	{
		//make code below from prefab
		GameObject chunkObject = new GameObject();
		chunkObject.AddComponent<MeshFilter>();
		chunkObject.AddComponent<MeshRenderer>();
		chunkObject.GetComponent<MeshRenderer>().material = mat;
		chunkObject.transform.position = new Vector3(position.x, position.y, position.z);


		List<Vector3> vertices = new List<Vector3>();
		List<int> triangles = new List<int>();
		List<Vector2> uvs = new List<Vector2>();
		List<Color> colors = new List<Color>();

		for(int x = 0; x < VoxelData.chunkDim; x++)
		{
			for(int y = 0; y < VoxelData.chunkDim; y++)
			{
				for(int z = 0; z < VoxelData.chunkDim; z++)
				{
					applyVoxelData(blockMap, new Vector3(x, y, z), vertices, triangles, uvs, colors, index);
				}
			}
		}

		generateMesh(chunkObject.GetComponent<MeshFilter>(), vertices, triangles, uvs);

		return chunkObject;
		
	}

	private static bool checkBlockExists(Vector3 pos, int[] blockMap, int index)
	{
		if (pos.x > 15 || pos.x < 0 || pos.y > 15 || pos.y < 0 || pos.z < 0 || pos.z > 15)
			return false;

		if (blockMap[index * 4096 + (int)pos.x * VoxelData.chunkDim2 + (int)pos.y * VoxelData.chunkDim + (int)pos.z] == 0)
			return false;

		return true;
	}

	static void generateMesh(MeshFilter filter, List<Vector3> vertices, List<int> triangles, List<Vector2> uvs)
	{
		Mesh mesh = new Mesh();

		mesh.SetVertices(vertices);
		mesh.SetTriangles(triangles, 0);
		mesh.SetUVs(0, uvs);
		mesh.RecalculateNormals();

		filter.mesh = mesh;

	}

	static void addTexture(int block, int face, List<Vector2> uvs)
	{
		float y = (block - 1) * VoxelData.normalizedAtlasBlockSizeY;
		float x = face * VoxelData.normalizedAtlasBlockSizeX;

		uvs.Add(new Vector2(x, y));
		uvs.Add(new Vector2(x, y + VoxelData.normalizedAtlasBlockSizeY));
		uvs.Add(new Vector2(x + VoxelData.normalizedAtlasBlockSizeX, y + VoxelData.normalizedAtlasBlockSizeY));
		uvs.Add(new Vector2(x + VoxelData.normalizedAtlasBlockSizeX, y));
	}

	private static void applyVoxelData(int[] blockMap, Vector3 pos, List<Vector3> vertices, List<int> triangles, List<Vector2> uvs, List<Color> colors, int index)
	{
		// 36 vertices because uv mapping a cube
		// makes it so each face needs its own 4 vertices


		//triangles use the indices of the vertices
		//but the vertices are already in order
		//so using i is fine

		

		int block = blockMap[index*4096 + (int)pos.x * VoxelData.chunkDim2 + (int)pos.y * VoxelData.chunkDim + (int)pos.z];

		if (block == 0)
			return;

		for (int face = 0; face < 6; face++)
		{ 
			if (!checkBlockExists(pos + VoxelData.faceChecks[face], blockMap, index)) // adds face checks to see if the face is facing a voxel or not
			{
				vertices.Add(VoxelData.voxelVertices[VoxelData.voxelTriangles[face * 4]] + pos);
				vertices.Add(VoxelData.voxelVertices[VoxelData.voxelTriangles[face * 4 + 1]] + pos);
				vertices.Add(VoxelData.voxelVertices[VoxelData.voxelTriangles[face * 4 + 2]] + pos);
				vertices.Add(VoxelData.voxelVertices[VoxelData.voxelTriangles[face * 4 + 3]] + pos);

				triangles.Add(vertices.Count - 4);
				triangles.Add(vertices.Count - 3);
				triangles.Add(vertices.Count - 2);

				triangles.Add(vertices.Count - 2);
				triangles.Add(vertices.Count - 1);
				triangles.Add(vertices.Count - 4);

				addTexture(block, face, uvs);

				//Color color = world.blockList.types[block].blockColor;
				//colors.Add(color);
				//colors.Add(color);
				//colors.Add(color);
				//colors.Add(color);

			}

		}

	}

}

