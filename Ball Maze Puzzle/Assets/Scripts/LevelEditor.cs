using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using UnityEngine;

public class LevelEditor : MonoBehaviour {

	Transform levelEditorCanvas;

	public Material levelEditorGridMaterial;

	const int OBSTACLE_VALUE = 2;

	[Range(1, 5)]
	public int gridRows = 3;
	[Range(1, 5)]
	public int gridColumns = 3;
	[Range(1, 45)]
	public int divisions = 9;

	int[,] gridValues;
	public ColourDetails[] colourDetails = new ColourDetails[5];

	bool canEdit = true;

	int currentColour = 0;

	



	// Use this for initialization
	void Start () {
		GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Quad);
		obj.name = "LevelEditorCanvas";
		obj.GetComponent<Renderer>().sharedMaterial = levelEditorGridMaterial;
		obj.layer = 9;

		levelEditorCanvas = obj.transform;

		InitGrid();

		colourDetails[0].name = "Tile Background";
		colourDetails[1].name = "Border";
		colourDetails[2].name = "Obstacle";
		colourDetails[3].name = "Ball";
		colourDetails[4].name = "Goal";

		
		//debug
		//LevelMeshGenerator.CreateObstacleObject();
		//Vector3 a = new Vector3(0, 1, 0);
		//Vector3 b = new Vector3(1, 1, 0);
		//Vector3 c = new Vector3(3, 0, 0);
		//float j = Vector3.Cross((b - a).normalized, (c - b).normalized).z;
		//float k = Vector3.Dot((b - a).normalized, (c - b).normalized);
		//Debug.Log("Right 90 turn");
		//Debug.Log(j.ToString());
		//Debug.Log(k.ToString());

		//a = new Vector3(0, 1, 0);
		//b = new Vector3(1, 1, 0);
		//c = new Vector3(3, 2, 0);
		//j = Vector3.Cross((b - a).normalized, (c - b).normalized).z;
		//k = Vector3.Dot((b - a).normalized, (c - b).normalized);
		//Debug.Log("Left 90 turn");
		//Debug.Log(j.ToString());
		//Debug.Log(k.ToString());

		//a = new Vector3(-1, 1, 0);
		//b = new Vector3(1, 1, 0);
		//c = new Vector3(3, 1, 0);
		//j = Vector3.Cross((b - a).normalized, (c - b).normalized).z;
		//k = Vector3.Dot((b - a).normalized, (c - b).normalized);
		//Debug.Log("Straight");
		//Debug.Log(j.ToString());
		//Debug.Log(k.ToString());
	}

	// Use this for initialization
	void Update() {
		if(Input.GetMouseButton(0) && canEdit) {

			Vector2 arrayPos = CalculateArrayPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));
			if (arrayPos.x > -1) {

				arrayPos = new Vector2(gridValues.GetLength(0) * arrayPos.x, gridValues.GetLength(1) * arrayPos.y);

				UpdateColourGrid((int)arrayPos.x, (int)arrayPos.y, currentColour);
			}

			levelEditorGridMaterial.mainTexture = GenerateTexture();
		}
	}

	Vector2 CalculateArrayPosition(Vector3 mousePos) {
		float canvasXMin = levelEditorCanvas.GetComponent<Renderer>().bounds.min.x;
		float canvasXMax = levelEditorCanvas.GetComponent<Renderer>().bounds.max.x;
		float canvasYMin = levelEditorCanvas.GetComponent<Renderer>().bounds.min.y;
		float canvasYMax = levelEditorCanvas.GetComponent<Renderer>().bounds.max.y;

		if (mousePos.x > canvasXMin && mousePos.x < canvasXMax && mousePos.y > canvasYMin && mousePos.y < canvasYMax) {

			Vector2 arrayPos = new Vector2(0.5f + mousePos.x / (canvasXMax - canvasXMin), 0.5f + (mousePos.y / (canvasYMax - canvasYMin)));

			return arrayPos;
		} else {
			return new Vector2(-1, -1);
		}
	}

	void UpdateColourGrid(int x, int y, int currentColor) {
		if (gridValues[x, y] != 1) {
			gridValues[x, y] = currentColor;
		}
	}

	private void OnValidate() {
		InitGrid();
	}

	public void InitGrid() {
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

		levelEditorGridMaterial.mainTexture = GenerateTexture();
	}

	public void SaveGrid(string filename) {
		// convert array to text and save
		if (filename != "") {
			List<string> linesToWrite = new List<string>();
			for (int x = 0; x < gridValues.GetLength(0); x++) {
				StringBuilder line = new StringBuilder();
				for (int y = 0; y < gridValues.GetLength(1); y++) {
					if (y != gridValues.GetLength(1) - 1) {
						line.Append(gridValues[y, gridValues.GetLength(0) - 1 - x].ToString()).Append(" ");
					} else {
						line.Append(gridValues[y, gridValues.GetLength(0) - 1 - x].ToString());
					}
				}
				linesToWrite.Add(line.ToString());
			}
			System.IO.File.WriteAllLines("Assets/Resources/" + filename + ".txt", linesToWrite.ToArray());
		}
	}



	public void LoadGrid(string filename) {
		// convert text to array and load
		int x = 0;
		int y = gridValues.GetLength(1) - 1;
		string line;
		//string log = "";

		// Read the file and display it line by line.  
		System.IO.StreamReader file = new System.IO.StreamReader("Assets/Resources/" + filename + ".txt");
		while ((line = file.ReadLine()) != null) {
			foreach(string str in line.Split(' ')) {
				if (str != " ") {
					gridValues[x, y] = int.Parse(str);
					//log += gridValues[x, y].ToString();
					x++;
				}
			}
			//log += "\n";
			x = 0;
			y--;
		}

		file.Close();
		//Debug.Log(log);
		levelEditorGridMaterial.mainTexture = GenerateTexture();
		LevelMeshGenerator.GenerateMeshes(gridValues);
	}

	Texture2D GenerateTexture() {
		Texture2D tex = new Texture2D(gridValues.GetLength(0), gridValues.GetLength(1));
		tex.filterMode = FilterMode.Point;

		for (int x = 0; x < gridValues.GetLength(0); x++) {
			for (int y = gridValues.GetLength(1) - 1; y >= 0; y--) {
				tex.SetPixel(x, y, colourDetails[gridValues[x, y]].colour);
			}
		}

		tex.Apply();
		return tex;
	}



	public void SetCanEdit(bool canEdit) {
		this.canEdit = canEdit;
	}



	public void SetCurrentColour(int newColour) {
		currentColour = newColour;
	}



	[System.Serializable]
	public struct ColourDetails {
		public string name;
		public Color colour;
	}
}
