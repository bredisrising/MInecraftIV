using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Burst;

public class World : MonoBehaviour
{
	public int seed;

	public Transform player;
	public Vector3 spawnPosition;

    public Material worldMat;
	//public BlockType[] blockTypes;
	public BlockList blockList;
	public TerrainGenerator terrainGen;

	Chunk[,,] chunks = new Chunk[VoxelData.worldSizeInChonksXZ, VoxelData.worldSizeInChonksY, VoxelData.worldSizeInChonksXZ];

	List<ChunkCoord> activeChunks = new List<ChunkCoord>();
	List<ChunkCoord> chunksToGen = new List<ChunkCoord>();
	private bool isCreatingChunks = false;


	ChunkCoord playerLastChunkCoord;

	float center = VoxelData.worldSizeInBlocksXZ / 2f;

	public ComputeShader shader;
	private void Start()
	{
		seed = (int)System.DateTime.Now.Ticks;
		


		
		spawnPosition = new Vector3(center, VoxelData.worldSizeInBlocksY + 3, center);
		initGenerateChunks();
		playerLastChunkCoord = getChunkCoordFromWorldPos(player.position);
	}

	private void Update()
	{
		if (!getChunkCoordFromWorldPos(player.position).isEqual(playerLastChunkCoord))
		{
			checkViewDistance();
		}

		if (chunksToGen.Count > 0 && !isCreatingChunks)
		{
			//StartCoroutine(genChunks());
			genChunksImmediately();
		}
			

		playerLastChunkCoord = getChunkCoordFromWorldPos(player.position);
	}
	void initGenerateChunks()
	{
		for(int y = (VoxelData.worldSizeInChonksY) - VoxelData.viewDistanceInChonksY; y < VoxelData.worldSizeInChonksY; y++)
		{
			for (int x = (VoxelData.worldSizeInChonksXZ / 2) - VoxelData.viewDistanceInChonksXZ; x < (VoxelData.worldSizeInChonksXZ / 2) + VoxelData.viewDistanceInChonksXZ; x++)
			{
				for (int z = (VoxelData.worldSizeInChonksXZ / 2) - VoxelData.viewDistanceInChonksXZ; z < (VoxelData.worldSizeInChonksXZ / 2) + VoxelData.viewDistanceInChonksXZ; z++)
				{
					//createNewChunk(x, y, z);
					if(isChunkInWorld(new ChunkCoord(x, y, z)))
					{
						chunks[x, y, z] = new Chunk(new ChunkCoord(x, y, z), this, true);
						activeChunks.Add(new ChunkCoord(x, y, z));
					}
					
				}
			}
		}
		player.position = spawnPosition;
	}

	IEnumerator genChunks()
	{
		isCreatingChunks = true;

		while(chunksToGen.Count > 0)
		{
			chunks[chunksToGen[0].x, chunksToGen[0].y, chunksToGen[0].z].init(true);
			chunksToGen.RemoveAt(0);
			yield return null;
		}

		isCreatingChunks = false;
	}

	void genChunksImmediately()
	{
		isCreatingChunks = true;

		//while (chunksToGen.Count > 0)
		//{
		//	



		//	chunksToGen.RemoveAt(0);
		//}

		//NativeArray<byte> blockMapOfChunks = new NativeArray<byte>(chunksToGen.Count * 4096, Allocator.TempJob);
		//NativeArray<Vector3> chunkPositions = new NativeArray<Vector3>(chunksToGen.Count, Allocator.TempJob);

		//for (int i = 0; i < chunksToGen.Count; i++)
		//{
		//	chunks[chunksToGen[i].x, chunksToGen[i].y, chunksToGen[i].z].init(false);
		//	chunkPositions[i] = chunksToGen[i].toVector3();

		//}

		//ChunkDataGenJob job = new ChunkDataGenJob
		//{
		//	blockMapsForTheChunks = blockMapOfChunks,
		//	chunkPositions = chunkPositions,
		//};

		//JobHandle jobHandle = job.Schedule(chunksToGen.Count * 4096, 1024);
		//jobHandle.Complete();
		

		//for (int i = 0; i < chunksToGen.Count; i++)
		//{
		//	NativeArray<byte>.Copy(blockMapOfChunks, i * 4096, chunks[chunksToGen[i].x, chunksToGen[i].y, chunksToGen[i].z].blockMap, 0, 4096);
		//	chunks[chunksToGen[i].x, chunksToGen[i].y, chunksToGen[i].z].buildChunk();
		//}

		//blockMapOfChunks.Dispose();
		//chunkPositions.Dispose();

		//isCreatingChunks = false;

		//chunksToGen.Clear();

	}

	ChunkCoord getChunkCoordFromWorldPos(Vector3 pos)
	{
		int x = ((int)pos.x) / VoxelData.chunkDim;
		int y = ((int)pos.y) / VoxelData.chunkDim;
		int z = ((int)pos.z) / VoxelData.chunkDim;

		return new ChunkCoord(x, y, z);
	}

	void checkViewDistance()
	{
		ChunkCoord coord = getChunkCoordFromWorldPos(player.position);

		List<ChunkCoord> previouslyActiveChunks = new List<ChunkCoord>(activeChunks);
		activeChunks.Clear();
		for(int y = coord.y - VoxelData.viewDistanceInChonksY; y < coord.y + VoxelData.viewDistanceInChonksY; y++)
		{
			for(int x = coord.x - VoxelData.viewDistanceInChonksXZ; x < coord.x + VoxelData.viewDistanceInChonksXZ; x++)
			{
				for (int z = coord.z - VoxelData.viewDistanceInChonksXZ; z < coord.z + VoxelData.viewDistanceInChonksXZ; z++)
				{
					if (isChunkInWorld(new ChunkCoord(x, y, z)))
					{
						if (chunks[x, y, z] == null)
						{
							chunks[x, y, z] = new Chunk(new ChunkCoord(x, y, z), this, false);
							chunksToGen.Add(new ChunkCoord(x, y, z));
						}else if (!chunks[x, y, z].isActive)
						{
							chunks[x, y, z].isActive = true;
						}
						activeChunks.Add(new ChunkCoord(x, y, z));
					}

					//removes the ones that are still in view from the previous active chunks


					for (int i = 0; i < previouslyActiveChunks.Count; i++)
					{
						if (previouslyActiveChunks[i].isEqual(new ChunkCoord(x, y, z)))
						{
							previouslyActiveChunks.RemoveAt(i);
						}

					}

				}
			}
		}
		foreach (ChunkCoord c in previouslyActiveChunks)
		{
			chunks[c.x, c.y, c.z].isActive = false;
		}


	}

	public byte getBlock(Vector3 pos)
	{
		int yPos = (int)pos.y;

		// IMMUTABLE PASS
		if (!isBlockInWorld(pos))
		{
			return 0;
		}

		if(yPos == 0)
		{
			return 4;
		}

		int terrainHeight = terrainGen.getY(pos);

		if(yPos > terrainHeight)
		{
			return 0;
		}
		else if(yPos == terrainHeight)
		{
			return 2;
		}
		else if(yPos > terrainHeight - 20)
		{
			return 1;
		}
		else
		{
			return 3;
		}

	}

	//void createNewChunk(int x, int y, int z)
	//{
	//	chunks[x, y, z] = new Chunk(new ChunkCoord(x, y, z), this, );
	//	activeChunks.Add(new ChunkCoord(x, y, z));
	//}

	bool isChunkInWorld(ChunkCoord coord)
	{
		if(coord.x >= 0 && coord.x < VoxelData.worldSizeInChonksXZ && coord.z >= 0 && coord.z < VoxelData.worldSizeInChonksXZ && coord.y >= 0 && coord.y < VoxelData.worldSizeInChonksY)
		{
			return true;
		}

		return false;
	}

	bool isBlockInWorld(Vector3 pos)
	{
		if(pos.x >= 0 && pos.x < VoxelData.worldSizeInBlocksXZ && pos.z >= 0 && pos.z < VoxelData.worldSizeInBlocksXZ && pos.y >= 0 && pos.y < VoxelData.worldSizeInBlocksY)
		{
			return true;
		}
		return false;
	} 

	public bool checkForBlock(Vector3 pos)
	{
		ChunkCoord thisChunk = new ChunkCoord(pos);

		if (!isChunkInWorld(thisChunk) || pos.y < 0 || pos.y > VoxelData.worldSizeInBlocksY)
			return false;

		if (chunks[thisChunk.x, thisChunk.y, thisChunk.z] != null && chunks[thisChunk.x, thisChunk.y, thisChunk.z].isBlockMapCreated)
			return blockList.types[chunks[thisChunk.x, thisChunk.y, thisChunk.z].getBlockFromWorldPos(pos)].isSolid;

		return blockList.types[getBlock(pos)].isSolid;
	}

}


public struct ChunkDataGenJob : IJobParallelFor
{
	public NativeArray<byte> blockMapsForTheChunks;
	
	[ReadOnly]
	public NativeArray<Vector3> chunkPositions;


	public void Execute(int index)
	{
		//for (int x = 0; x < 16; x++)
		//{
		//	for (int y = 0; y < 16; y++)
		//	{
		//		for (int z = 0; z < 16; z++)
		//		{


		//			float2 noisePos;
		//			noisePos.x = x + chunkPositions[index].x;
		//			noisePos.y = z + chunkPositions[index].z;

		//			float yPos = Unity.Mathematics.noise.cnoise(noisePos) * 30 + 600;

		//			if (y + chunkPositions[index].y <= yPos)
		//			{
		//				blockMapsForTheChunks[(index * 4096) + x * 256 + y * 16 + z] = 3;
		//			}
		//			else
		//			{
		//				blockMapsForTheChunks[(index * 4096) + x * 256 + y * 16 + z] = 0;
		//			}
		//		}
		//	}
		//}

		int x = index % 4096;
		x /= 256;

		int y = index % 256 / 16;

		int z = index % 256 % 16;


		float2 noisePos = new float2(x + chunkPositions[index / 4096].x, z + chunkPositions[index / 4096].z);

		float yPos = noise.cnoise(noisePos);

		if (index % 5 <= 1f)
		{
			blockMapsForTheChunks[index] = 3;
		}
		else
		{
			blockMapsForTheChunks[index] = 0;
		}



	}
}



