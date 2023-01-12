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


	int[] amplitudes = {100, 50, 15}; 
	float[] frequencies = {.1f, .25f, .4f};

	int count = 0;

	public static float get2DPerlin (Vector2 position, float offset, float scale)
	{
		return Mathf.PerlinNoise((position.x + .1f) / VoxelData.chunkDim * scale + offset, 
			(position.y + .1f) / VoxelData.chunkDim * scale + offset);
	}

	public int getY(Vector3 position)
	{
		count++;

		

		int y = solidGroundHeight-100;

		for (int i = 0; i < amplitudes.Length; i++) 
		{
			y += Mathf.FloorToInt(amplitudes[i] * get2DPerlin(new Vector2(position.x, position.z), 1, frequencies[i]));
		}


		return y;
	}


	public static int getBlock(Vector3 position)
	{
		return 3;
	}

}
