using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class Chunk
{
	ChunkCoord coord;

    Mesh mesh;

	GameObject chunkObject;
    MeshRenderer meshRenderer;
    MeshFilter meshFilter;

	List<Vector3> vertices = new List<Vector3>();
	List<int> triangles = new List<int>();
	List<Vector2> uvs = new List<Vector2>();
	List<Color> colors = new List<Color>();
	
	public byte[] blockMap = new byte[VoxelData.chunkDim * VoxelData.chunkDim * VoxelData.chunkDim];

	World world;

	private bool _isActive;
	public bool isBlockMapCreated = false;
	public Chunk(ChunkCoord _coord, World _world, bool generateOnLoad)
	{
		coord = _coord;
		world = _world;
		isActive = true;

		if (generateOnLoad)
		{
			init(true);
		}
	}

	public void init(bool doordonot) 
	{
		chunkObject = new GameObject();
		meshFilter = chunkObject.AddComponent<MeshFilter>();
		meshRenderer = chunkObject.AddComponent<MeshRenderer>();

		meshRenderer.material = world.worldMat;
		chunkObject.transform.SetParent(world.transform);
		chunkObject.transform.position = new Vector3(coord.x * VoxelData.chunkDim, coord.y * VoxelData.chunkDim, coord.z * VoxelData.chunkDim);
		chunkObject.name = "Chunk " + coord.x + " " + coord.y + " " + coord.z;

		if (doordonot)
		{
			makeVoxelMap();
			buildChunk();
		}
			
	}



	public void buildChunk()
	{
		//makeVoxelMap();

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
		for (int x = 0; x < VoxelData.chunkDim; x++)
		{
			for (int y = 0; y < VoxelData.chunkDim; y++)
			{
				for (int z = 0; z < VoxelData.chunkDim; z++)
				{
					blockMap[x * 256 + y * 16 + z] = world.getBlock(new Vector3(x, y, z) + position);
				}
			}
		}

		/*
		int numVertices;

		int length = VoxelData.chunkDim * VoxelData.chunkDim * VoxelData.chunkDim;
		ComputeBuffer blockMapCB = new ComputeBuffer(length, sizeof(int));
		blockMapCB.SetData(blockMap);

		length = 1;
		ComputeBuffer numVerticesCB = new ComputeBuffer(length, sizeof(int));
		numVerticesCB.SetData(new int[] { 0 });


		world.shader.SetBuffer(0, "blocks", blockMapCB);
		world.shader.SetBuffer(1, "blocks", blockMapCB);

		world.shader.SetBuffer(1, "numVertices", numVerticesCB);

		world.shader.Dispatch(0, 4, 4, 4);
		world.shader.Dispatch(1, 4, 4, 4);



		blockMapCB.GetData(blockMap);
		blockMapCB.Dispose();
		numVerticesCB.Dispose();
		*/


		isBlockMapCreated = true;

	}

	public bool isActive
	{
		get { return _isActive; }
		set 
		{ 
			_isActive = value;
			if(chunkObject != null)
			{
				chunkObject.SetActive(value);
			}	
		}
	}
	public Vector3 position
	{
		get { return coord.toBlockWorldPositionVector3(); }
	}

	bool isVoxelInChunk(Vector3 pos)
	{
		int x = (int)pos.x;
		int y = (int)pos.y;
		int z = (int)pos.z;

		if (x < 0 || x > VoxelData.chunkDim - 1 || y < 0 || y > VoxelData.chunkDim - 1 || z < 0 || z > VoxelData.chunkDim - 1)
			return false;

		return true;
		
	}

	bool checkBlockExists(Vector3 pos)
	{
		int x = (int)pos.x;
		int y = (int)pos.y;
		int z = (int)pos.z;

		if (!isVoxelInChunk(pos))
		{
			return world.checkForBlock(pos + position);
		}

		return world.blockList.types[blockMap[x * VoxelData.chunkDim2 + y * VoxelData.chunkDim + z]].isSolid;
	}

	public byte getBlockFromWorldPos(Vector3 pos)
	{
		int x = ((int)pos.x) / VoxelData.chunkDim;
		int y = ((int)pos.y) / VoxelData.chunkDim;
		int z = ((int)pos.z) / VoxelData.chunkDim;

		x -= (int)chunkObject.transform.position.x;
		y -= (int)chunkObject.transform.position.y;
		z -= (int)chunkObject.transform.position.z;

		return blockMap[x * VoxelData.chunkDim2 + y * VoxelData.chunkDim + z];
	}

	void applyVoxelData (Vector3 pos)
	{
		// 36 vertices because uv mapping a cube
		// makes it so each face needs its own 4 vertices

		//triangles use the indices of the vertices
		//but the vertices are already in order
		//so using i is fine

		int block = blockMap[(int)pos.x * VoxelData.chunkDim2 + (int)pos.y * VoxelData.chunkDim + (int)pos.z];

		if (world.blockList.types[block].blockName == "Air")
			return;

		for (int face = 0; face < 6; face++)
		{



			//if (!checkBlockExists(pos + VoxelData.faceChecks[face])) // adds face checks to see if the face is facing a voxel or not
			//{
			//	vertices.Add(VoxelData.voxelVertices[VoxelData.voxelTriangles[face * 4]] + pos);
			//	vertices.Add(VoxelData.voxelVertices[VoxelData.voxelTriangles[face * 4 + 1]] + pos);
			//	vertices.Add(VoxelData.voxelVertices[VoxelData.voxelTriangles[face * 4 + 2]] + pos);
			//	vertices.Add(VoxelData.voxelVertices[VoxelData.voxelTriangles[face * 4 + 3]] + pos);

			//	triangles.Add(vertices.Count - 4);
			//	triangles.Add(vertices.Count - 3);
			//	triangles.Add(vertices.Count - 2);

			//	triangles.Add(vertices.Count - 2);
			//	triangles.Add(vertices.Count - 1);
			//	triangles.Add(vertices.Count - 4);

			//	addTexture(block, face);

			//	Color color = world.blockList.types[block].blockColor;
			//	colors.Add(color);
			//	colors.Add(color);
			//	colors.Add(color);
			//	colors.Add(color);

			//}

		}
	}


	void addTexture(int id, int faceIndex)
	{
		float y = (id - 1) * VoxelData.normalizedAtlasBlockSizeY;
		float x = faceIndex * VoxelData.normalizedAtlasBlockSizeX;

		uvs.Add(new Vector2(x, y));
		uvs.Add(new Vector2(x, y + VoxelData.normalizedAtlasBlockSizeY));
		uvs.Add(new Vector2(x + VoxelData.normalizedAtlasBlockSizeX, y + VoxelData.normalizedAtlasBlockSizeY));
		uvs.Add(new Vector2(x + VoxelData.normalizedAtlasBlockSizeX, y));
	}

	void createMesh()
	{
		mesh = new Mesh();


		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.uv = uvs.ToArray();
		mesh.colors = colors.ToArray();

		mesh.RecalculateNormals();

		meshFilter.mesh = mesh;

		//if (coord.x == 0)
		//{
		//	var savePath = "Assets/" + "AChunkMesh.asset";
		//	AssetDatabase.CreateAsset(mesh, savePath);
		//}

	}

}



public class ChunkCoord
{
	public int x;
	public int y;
	public int z;

	public ChunkCoord(int _x, int _y, int _z)
	{
		x = _x;
		y = _y;
		z = _z;
	}

	public ChunkCoord(Vector3 pos)
	{
		x = (int)pos.x;
		y = (int)pos.y;
		z = (int)pos.z;
	}

	public ChunkCoord()
	{
		x = 0;
		y = 0;
		z = 0;
	}


	public Vector3 toVector3()
	{
		return new Vector3(x, y, z);
	}

	public Vector3 toBlockWorldPositionVector3()
	{
		return new Vector3(x, y, z) * VoxelData.chunkDim;
	}

	public bool isEqual(ChunkCoord other)
	{
		if (other == null)
		{
			return false;
		} 
		else if (other.x == x && other.y == y && other.z == z)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

}
