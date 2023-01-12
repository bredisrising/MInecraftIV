using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

public class NewWorld : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] ComputeShader terrainGenShader;
    [SerializeField] Material worldMaterial;

	int3 lastChunkPositionOfPlayer;

	Dictionary<int3, GameObject> activeChunks = new Dictionary<int3, GameObject>();

	public bool createChunks = false;
	[SerializeField] int index;


	int3[] chunkPositions = new int3[8 * VoxelData.cubicWorldSize + 10];

	void Start()
    {
		player.position = new Vector3(2 * 16, 20, 2 * 16);
		initialGeneration(10);
		lastChunkPositionOfPlayer = getChunkPosFromWorldPos(player.position);
		index = 5;
	}

    void initialGeneration(int size)
    {
		int initSize = size;

		int[] blockMap = new int[4096 * initSize * initSize];

		for (int x = 0; x < initSize; x++)
		{
			for (int z = 0; z < initSize; z++)
			{
				chunkPositions[x * initSize + z] = new int3(x * 16, 0, z * 16);
			}
		}

		ComputeBuffer blockMapCB = new ComputeBuffer(initSize * initSize * 4096, sizeof(int));
		blockMapCB.SetData(blockMap);

		ComputeBuffer chunkPositionsCB = new ComputeBuffer(8 * VoxelData.cubicWorldSize + 10, sizeof(int) * 3);
		chunkPositionsCB.SetData(chunkPositions);


		terrainGenShader.SetBuffer(0, "positions", chunkPositionsCB);
		terrainGenShader.SetBuffer(0, "outputBuffer", blockMapCB);

		terrainGenShader.Dispatch(0, initSize * initSize, 1, 1);

		blockMapCB.GetData(blockMap);

		blockMapCB.Dispose();
		chunkPositionsCB.Dispose();


		for (int x = 0; x < initSize; x++)
		{
			for (int z = 0; z < initSize; z++)
			{
				activeChunks[new int3(x, 0, z)] = StaticChunkRendering.GenerateChunk(blockMap, new int3(x * 16, 0, z * 16), worldMaterial, x * initSize + z);
			}
		}
	}

	void proceduralGeneration(int3 playerPos)
	{
		int size = 0;


		int i = 0;

		float startTime = Time.realtimeSinceStartup;

		for (int x = playerPos.x - VoxelData.viewDistanceInChonksXZ; x < playerPos.x + VoxelData.viewDistanceInChonksXZ; x++)
		{
			for (int y = playerPos.y - VoxelData.viewDistanceInChonksY; y < playerPos.y + VoxelData.viewDistanceInChonksY; y++)
			{
				for (int z = playerPos.z - VoxelData.viewDistanceInChonksXZ; z < playerPos.z + VoxelData.viewDistanceInChonksXZ; z++)
				{
					if(!activeChunks.ContainsKey(new int3(x, y, z)))
					{
						size++;
						chunkPositions[i] = new int3(x, y, z);
						i++;
					}
				}
			}
		}

		float endTime = Time.realtimeSinceStartup;
		Debug.Log("checking view distance and and allocating " + (endTime - startTime) * 1000);


		if (size <= 0)
			return;

		int initSize = size;
		int[] blockMap = new int[4096 * initSize];

		ComputeBuffer blockMapCB = new ComputeBuffer(initSize * 4096, sizeof(int));
		blockMapCB.SetData(blockMap);

		ComputeBuffer chunkPositionsCB = new ComputeBuffer(8 * VoxelData.cubicWorldSize + 10, sizeof(int) * 3);
		chunkPositionsCB.SetData(chunkPositions);



		terrainGenShader.SetBuffer(0, "positions", chunkPositionsCB);
		terrainGenShader.SetBuffer(0, "outputBuffer", blockMapCB);

		terrainGenShader.Dispatch(0, initSize * 16, 1, 1);

		

		startTime = Time.realtimeSinceStartup;

		blockMapCB.GetData(blockMap);

		endTime = Time.realtimeSinceStartup;
		Debug.Log("allocate and dispatch shader" + (endTime - startTime) * 1000);

		blockMapCB.Dispose();
		chunkPositionsCB.Dispose();

		

		


		for(int index = 0; index < initSize; index++)
		{
			activeChunks[chunkPositions[index]] = StaticChunkRendering.GenerateChunk(blockMap, chunkPositions[index] * 16, worldMaterial, index);
		}
	}


    void Update()
    {
		if (math.any(getChunkPosFromWorldPos(player.position) != lastChunkPositionOfPlayer))
		{
			proceduralGeneration(getChunkPosFromWorldPos(player.position));
			lastChunkPositionOfPlayer = getChunkPosFromWorldPos(player.position);
		}


	}


	int3 getChunkPosFromWorldPos(Vector3 worldPos)
	{
		int x = ((int)worldPos.x)/16;
		int y = ((int)worldPos.y)/16;
		int z = ((int)worldPos.z)/16;

		return new int3(x, y, z);
	}

}