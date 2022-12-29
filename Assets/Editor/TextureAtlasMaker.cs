using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
public class TextureAtlasMaker : EditorWindow
{
	int textureSizeInPixels = 16;
	int atlasSizeInTextures = 3;
	int atlasSize;

	Texture2D atlas;
	Texture2D overlay;

	Object[] rawTextures = new Object[256];
	List<Texture2D> sortedTextures = new List<Texture2D>();

	BlockList blockList;

	[MenuItem("Minecraft IV/Atlas Packer")]
	public static void ShowWindow()
	{
		EditorWindow.GetWindow(typeof(TextureAtlasMaker));
	}

	private void OnGUI()
	{
		atlasSize = textureSizeInPixels * atlasSizeInTextures;

		GUILayout.Label("Texture Atlas Packer", EditorStyles.boldLabel);

		textureSizeInPixels = EditorGUILayout.IntField("TextureSize", textureSizeInPixels);
		atlasSizeInTextures = EditorGUILayout.IntField("Atlas Size", atlasSizeInTextures);

		GUILayout.Label(atlas);

		if (GUILayout.Button("Load Textures"))
		{
			blockList = Resources.Load<BlockList>("ScriptableObjects/BlockList");
			
			//loadTextures();
			pack();
			packOverlay();
		}

		if(GUILayout.Button("Save Atlas"))
		{
			
			byte[] bytes = atlas.EncodeToPNG();
			byte[] bytes1 = overlay.EncodeToPNG();

			try
			{
				File.WriteAllBytes(Application.dataPath + "/Textures/atlas.png", bytes);
				File.WriteAllBytes(Application.dataPath + "/Textures/overlay.png", bytes1);
			}
			catch
			{
				Debug.Log("couldn't save");
			}
		}

	}

	//void loadTextures()
	//{
	//	sortedTextures.Clear();
	//	//rawTextures = Resources.LoadAll("AtlasPacker", typeof(Texture2D));

	//	int index = 0;
	//	foreach(Object tex in rawTextures)
	//	{
	//		Texture2D t = (Texture2D)tex;
	//		if (t.width == textureSizeInPixels && t.height == textureSizeInPixels)
	//			sortedTextures.Add(t);
	//		else
	//			Debug.Log("shader dont work");


	//		index++;
	//	}

	//	Debug.Log("success");

	//}

	void pack()
	{
		int xSize = textureSizeInPixels * 6;
		int ySize = textureSizeInPixels * (blockList.types.Length - 1);

		atlas = new Texture2D(xSize, ySize);
		//Color[] pixels = new Color[xSize * ySize];

		for(int blockIndex = 1; blockIndex < blockList.types.Length; blockIndex++)
		{
			for(int faceIndex = 0; faceIndex < 6; faceIndex++)
			{
				if(blockList.types[blockIndex].textureFaces.Length == 1)
				{
					Texture2D currentTex = blockList.types[blockIndex].textureFaces[0];
					int xPixel = faceIndex * textureSizeInPixels;
					int yPixel = (blockIndex - 1) * textureSizeInPixels;

					atlas.SetPixels(xPixel, yPixel, textureSizeInPixels, textureSizeInPixels, currentTex.GetPixels(0, 0, textureSizeInPixels, textureSizeInPixels));
				}
				else
				{
					Texture2D currentTex = blockList.types[blockIndex].textureFaces[faceIndex];
					int xPixel = faceIndex * textureSizeInPixels;
					int yPixel = (blockIndex - 1) * textureSizeInPixels;

					atlas.SetPixels(xPixel, yPixel, textureSizeInPixels, textureSizeInPixels, currentTex.GetPixels(0, 0, textureSizeInPixels, textureSizeInPixels));
				}

				
			}
		}

		atlas.Apply();
	}

	void packOverlay()
	{
		int xSize = textureSizeInPixels * 6;
		int ySize = textureSizeInPixels * (blockList.types.Length - 1);

		overlay = new Texture2D(xSize, ySize);
		//Color[] pixels = new Color[xSize * ySize];

		for (int blockIndex = 1; blockIndex < blockList.types.Length; blockIndex++)
		{
			for (int faceIndex = 0; faceIndex < 6; faceIndex++)
			{
				if (blockList.types[blockIndex].overlayTextureFaces.Length == 1)
				{
					Texture2D currentTex = getSolidSquareTexture(new Color(0f, 0f, 0f, 0f), textureSizeInPixels);
					int xPixel = faceIndex * textureSizeInPixels;
					int yPixel = (blockIndex - 1) * textureSizeInPixels;

					overlay.SetPixels(xPixel, yPixel, textureSizeInPixels, textureSizeInPixels, currentTex.GetPixels(0, 0, textureSizeInPixels, textureSizeInPixels));
				}
				else
				{
					if(blockList.types[blockIndex].overlayTextureFaces[faceIndex] == null)
					{
						Texture2D currentTex = getSolidSquareTexture(new Color(0f, 0f, 0f, 0f), textureSizeInPixels);
						int xPixel = faceIndex * textureSizeInPixels;
						int yPixel = (blockIndex - 1) * textureSizeInPixels;

						overlay.SetPixels(xPixel, yPixel, textureSizeInPixels, textureSizeInPixels, currentTex.GetPixels(0, 0, textureSizeInPixels, textureSizeInPixels));
						
					}
					else
					{
						Texture2D currentTex = blockList.types[blockIndex].overlayTextureFaces[faceIndex];
						int xPixel = faceIndex * textureSizeInPixels;
						int yPixel = (blockIndex - 1) * textureSizeInPixels;

						overlay.SetPixels(xPixel, yPixel, textureSizeInPixels, textureSizeInPixels, currentTex.GetPixels(0, 0, textureSizeInPixels, textureSizeInPixels));
					}

					
				}


			}
		}

		overlay.Apply();
	}

	Texture2D getSolidSquareTexture(Color color, int size)
	{
		Texture2D newTexture = new Texture2D(size, size);
		Color[] pixels = new Color[size * size];

		for(int i = 0; i < size * size; i++)
		{
			pixels[i] = color;
		}
		newTexture.SetPixels(pixels);
		newTexture.Apply();
		return newTexture;
	}


}
