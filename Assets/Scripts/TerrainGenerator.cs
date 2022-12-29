using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MinecraftIV/Terrain Generator")]
public class TerrainGenerator : ScriptableObject
{
	public string biomeName;

	public int solidGroundHeight;
	public int terrainHeight;
	public float terrainScale;


    public static float get2DPerlin (Vector2 position, float offset, float scale)
	{
		return Mathf.PerlinNoise((position.x + .1f) / VoxelData.chunkDim * scale + offset, 
			(position.y + .1f) / VoxelData.chunkDim * scale + offset);
	}

	public int getY(Vector3 position)
	{
		return Mathf.FloorToInt(terrainHeight *
			get2DPerlin(new Vector2(position.x, position.z), 1, terrainScale)) + solidGroundHeight;
	}


	public static int getBlock(Vector3 position)
	{
		return 3;
	}

}
