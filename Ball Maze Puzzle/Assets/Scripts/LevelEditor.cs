using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LevelEditor : MonoBehaviour { 

	public Material levelEditorGridMaterial;

	[Range(1, 5)]
	public int gridRows = 3;
	[Range(1, 5)]
	public int gridColumns = 3;
	[Range(1, 45)]
	public int divisions = 9;

	int[,] gridValues;
	public ColourDetails[] colourDetails = new ColourDetails[5];

	// Use this for initialization
	void Start () {
		GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Quad);
		obj.name = "Canvas";
		obj.GetComponent<Renderer>().sharedMaterial = levelEditorGridMaterial;

		InitGrid();

		colourDetails[0].name = "Tile Background";
		colourDetails[1].name = "Border";
		colourDetails[2].name = "Obstacles";
		colourDetails[3].name = "Ball";
		colourDetails[4].name = "Goal";
	}

	// Use this for initialization
	void Update() {
		if(Input.GetMouseButton(0)) {
			//Find pixel at mouse click and change in array
			//Possibly by using edge of quad to camera
		}

		levelEditorGridMaterial.mainTexture = GenerateTexture();
	}

	private void OnValidate() {
		InitGrid();
	}

	void InitGrid() {
		gridValues = new int[(gridColumns * divisions) + gridColumns + 1, (gridRows * divisions) + gridRows + 1];

		for (int x = 0; x < gridValues.GetLength(0); x++) {
			for (int i = 0; i < gridValues.GetLength(1); i += gridValues.GetLength(1) / gridRows) {
				gridValues[x, i] = 1;
			}
		}
		for (int y = 0; y < gridValues.GetLength(1); y++) {
			for (int i = 0; i < gridValues.GetLength(0); i += gridValues.GetLength(0) / gridColumns) {
				gridValues[i, y] = 1;
			}
		}
	}

	Texture2D GenerateTexture() {
		Texture2D tex = new Texture2D(gridValues.GetLength(0), gridValues.GetLength(1));

		for (int x = 0; x < gridValues.GetLength(0); x++) {
			for (int y = 0; y < gridValues.GetLength(1); y++) {
					tex.SetPixel(x, y, colourDetails[gridValues[x, y]].colour);
			}
		}

		tex.Apply();
		return tex;
	}

	[System.Serializable]
	public struct ColourDetails {
		public string name;
		public Color colour;
	}
}
