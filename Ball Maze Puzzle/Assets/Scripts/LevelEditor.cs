﻿using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using UnityEngine;

public class LevelEditor : MonoBehaviour {

	Transform levelEditorCanvas;

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
		obj.layer = 9;

		levelEditorCanvas = obj.transform;

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

			Vector2 arrayPos = CalculateArrayPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));
			if (arrayPos.x > -1) {

				arrayPos = new Vector2((gridValues.GetLength(0) - 1) * arrayPos.x, (gridValues.GetLength(1) - 1) * arrayPos.y);

				UpdateColourGrid((int)arrayPos.x, (int)arrayPos.y, 3);
			}
			
		}

		levelEditorGridMaterial.mainTexture = GenerateTexture();
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
		gridValues[x, y] = currentColor;
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
	}

	public void SaveGrid(string filename) {
		// convert array to text and save
		if (filename != "") {
			List<string> linesToWrite = new List<string>();
			for (int x = 0; x < gridValues.GetLength(0); x++) {
				StringBuilder line = new StringBuilder();
				for (int y = 0; y < gridValues.GetLength(1); y++) {
					line.Append(gridValues[x, y].ToString()).Append(" ");
				}
				linesToWrite.Add(line.ToString());
			}
			System.IO.File.WriteAllLines("Assets/Resources/" + filename + ".txt", linesToWrite.ToArray());
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
