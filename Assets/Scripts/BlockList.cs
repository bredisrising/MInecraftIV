using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MinecraftIV/BlockList")]
public class BlockList : ScriptableObject
{
    public Type[] types;

}

[System.Serializable]
public class Type
{
	public string blockName;
	public bool isSolid;

	//set to white for most except for like grass which changes around
	public Color blockColor;
	//public Gradient blockColorGradient;

	[Header("Texture Values")]
	//public int[] textureFaceIDs;
	public Texture2D[] textureFaces;
	[Header("\n\n\n\n\n\n\n\nTexture Overlay Values")]
	public Texture2D[] overlayTextureFaces;
}