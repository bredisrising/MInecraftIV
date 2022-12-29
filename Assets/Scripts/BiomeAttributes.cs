using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MinecraftIV/Biome Attributes")]
public class BiomeAttributes : ScriptableObject
{
    public string biomeName;

    public int solidGroundHeight;
    public int terrainHeight;
    public float terrainScale;
}
