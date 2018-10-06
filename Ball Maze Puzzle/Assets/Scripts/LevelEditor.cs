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

	Vector2 originMin;
	Vector2 originMax;
	Vector2 originSize;
	Vector2 halfPixelSize;



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

		originMin = new Vector2(levelEditorCanvas.GetComponent<Renderer>().bounds.min.x, levelEditorCanvas.GetComponent<Renderer>().bounds.min.y);
		originMax = new Vector2(levelEditorCanvas.GetComponent<Renderer>().bounds.max.x, levelEditorCanvas.GetComponent<Renderer>().bounds.max.y);
		originSize = new Vector2(originMax.x - originMin.x, originMax.y - originMin.y);
		halfPixelSize = new Vector2(originSize.x / gridValues.GetLength(0) / 2, originSize.y / gridValues.GetLength(1) / 2);

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
		GenerateMeshes();
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



	void GenerateMeshes() {
		for (int x = 0; x < gridValues.GetLength(0); x++) {
			for (int y = gridValues.GetLength(1) - 1; y >= 0; y--) {
				if (gridValues[x, y] == OBSTACLE_VALUE) {
					GenerateObstacle(x, y);
				}
			}
		}
	}



	void GenerateObstacle(int startX, int startY) {

		int[,] obstacleValues = gridValues;
		Queue<Vector2> obstacleCoords = new Queue<Vector2>();
		int currentX = startX;
		int currentTopY = startY;
		int currentBottomY = startY;
		int currentVertexSide = -1;

		// First Column check //

		if(CheckPixelInDirectionForObstacle(obstacleValues, startX + 1, startY+1)) {
			obstacleCoords.Enqueue(AddVertexOfPixelToQueue(startX, startY, -1, -1));
		} else {
			obstacleCoords.Enqueue(AddVertexOfPixelToQueue(startX, startY, -1, 1));
		}

		while(CheckPixelInDirectionForObstacle(obstacleValues, currentX, currentBottomY - 1)) {
			currentBottomY--;
		}
		obstacleCoords.Enqueue(AddVertexOfPixelToQueue(currentX, currentBottomY, -1, -1));

		if (CheckPixelInDirectionForObstacle(obstacleValues, startX + 1, startY + 1)) {
			currentTopY++;
		} else {
			if (!CheckPixelInDirectionForObstacle(obstacleValues, startX + 1, currentTopY) && CheckPixelInDirectionForObstacle(obstacleValues, startX + 1, currentTopY - 1)) {
				obstacleCoords.Enqueue(AddVertexOfPixelToQueue(startX, startY, 1, -1));
			} else {
				obstacleCoords.Enqueue(AddVertexOfPixelToQueue(startX, startY, 1, 1));
			}

			if (CheckPixelInDirectionForObstacle(obstacleValues, startX + 1, currentBottomY - 1)) {
				obstacleCoords.Enqueue(AddVertexOfPixelToQueue(startX, currentBottomY - 1, 1, -1));
			} else {
				obstacleCoords.Enqueue(AddVertexOfPixelToQueue(startX, currentBottomY, 1, -1));
			}
			currentVertexSide = 1;
		}

		for (int i = currentTopY; i >= currentBottomY; i--) {
			obstacleValues[startX, i] = 0;
		}

		currentX++;

		// Other Columns //

		bool endOfObstacle = false;

		while(!endOfObstacle) {
			endOfObstacle = true;
			int previousTopY = currentTopY;
			currentTopY = currentBottomY - 1;

			for (int y = currentTopY; y < obstacleValues.GetLength(1) - 1; y++) {
				if(CheckPixelInDirectionForObstacle(obstacleValues, currentX, y) && !CheckPixelInDirectionForObstacle(obstacleValues, currentX, y + 1)) {
					if (!CheckPixelInDirectionForObstacle(obstacleValues, currentX + 1, y) && CheckPixelInDirectionForObstacle(obstacleValues, currentX + 1, y - 1)) {
						obstacleCoords.Enqueue(AddVertexOfPixelToQueue(currentX, y, currentVertexSide, -1));
					} else if (previousTopY > y && !CheckPixelInDirectionForObstacle(obstacleValues, currentX + 1, y+1) && !CheckPixelInDirectionForObstacle(obstacleValues, currentX + 1, y) && !CheckPixelInDirectionForObstacle(obstacleValues, currentX + 1, y-1)) {
						obstacleCoords.Enqueue(AddVertexOfPixelToQueue(currentX, y, currentVertexSide, -1));
					} else {
						obstacleCoords.Enqueue(AddVertexOfPixelToQueue(currentX, y, currentVertexSide, 1));
					}
					currentTopY = y;
					previousTopY = currentTopY;
					endOfObstacle = false;
					for (y = currentBottomY; y > 0; y--) {
						if (CheckPixelInDirectionForObstacle(obstacleValues, currentX, y) && !CheckPixelInDirectionForObstacle(obstacleValues, currentX, y - 1)) {
							if (CheckPixelInDirectionForObstacle(obstacleValues, currentX+1, y-1)) {
								obstacleCoords.Enqueue(AddVertexOfPixelToQueue(currentX, y-1, currentVertexSide, -1));
								currentBottomY = y-1;
							} else {
								obstacleCoords.Enqueue(AddVertexOfPixelToQueue(currentX, y, currentVertexSide, -1));
								currentBottomY = y;
							}

							for (int i = currentTopY; i >= currentBottomY; i--) {
								obstacleValues[currentX, i] = 0;
							}

							break;
						}
					}
					break;
				} else if (!CheckPixelInDirectionForObstacle(obstacleValues, currentX, y) && !CheckPixelInDirectionForObstacle(obstacleValues, currentX, y+1) && !CheckPixelInDirectionForObstacle(obstacleValues, currentX, y+2)) {
					break;
				}
			}

			currentX++;
		}

		//int directionCount = 0;
		//int previousDirectionCount = -1;

		//obstacleCoords = AddVerticesOfPixelToQueue(obstacleCoords, startX, startY, directionCount, previousDirectionCount);

		//do {
		//	switch (directionCount) {
		//		case 0:
		//			nextX = currentX;
		//			nextY = currentY - 1;
		//			break;
		//		case 1:
		//			nextX = currentX + 1;
		//			nextY = currentY - 1;
		//			break;
		//		case 2:
		//			nextX = currentX + 1;
		//			nextY = currentY;
		//			break;
		//		case 3:
		//			nextX = currentX + 1;
		//			nextY = currentY + 1;
		//			break;
		//		case 4:
		//			nextX = currentX;
		//			nextY = currentY + 1;
		//			break;
		//		case 5:
		//			nextX = currentX - 1;
		//			nextY = currentY + 1;
		//			break;
		//		case 6:
		//			nextX = currentX - 1;
		//			nextY = currentY;
		//			break;
		//		case 7:
		//			nextX = currentX - 1;
		//			nextY = currentY - 1;
		//			break;
		//	}

		//	if (CheckPixelInDirectionForObstacle(nextX, nextY)) {
		//		obstacleCoords = AddVerticesOfPixelToQueue(obstacleCoords, nextX, nextY, directionCount, previousDirectionCount);
		//		previousDirectionCount = directionCount;
		//		currentX = nextX;
		//		currentY = nextY;
		//		if (directionCount > 1) {
		//			directionCount -= 2;
		//		} else {
		//			directionCount = 0;
		//		}
		//	} else {
		//		if (directionCount > 7) {
		//			nextX = startX;
		//			nextY = startY;
		//			obstacleCoords = AddAllVerticesOfPixelToQueue(startX, startY);
		//		} else {
		//			directionCount++;
		//		}
		//	}

		//} while (nextX != startX || nextY != startY);		

		//Vector2[] obstaclePoints = ExtendEdgesOfVertices(obstacleCoords);
		Vector2[] obstaclePoints = obstacleCoords.ToArray();
		LevelMeshGenerator.CreateObstacleObject(obstaclePoints);
	}



	bool CheckPixelInDirectionForObstacle(int[,] obstacleValues, int currentX, int currentY) {
		if(currentX < gridValues.GetLength(0) - 1 && currentX >= 0 && currentY < gridValues.GetLength(1) - 1 && currentY >= 0) {
			if(obstacleValues[currentX, currentY] == OBSTACLE_VALUE) {
				return true;
			}
		}

		return false;
	}



	Vector2 AddVertexOfPixelToQueue(int pixelX, int pixelY, int directionX, int directionY) {
		//Debug
		Transform obj = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
		obj.position = new Vector2(originMin.x + halfPixelSize.x +(halfPixelSize.x * pixelX * 2) + (directionX * halfPixelSize.x), originMin.y + halfPixelSize.y + (halfPixelSize.y * pixelY * 2) + (directionY * halfPixelSize.y));
		obj.localScale = Vector3.one * 0.01f;
		obj.GetComponent<Renderer>().material.color = Color.blue;
		obj.name = "POINT: " + pixelX.ToString() + ", " + pixelY.ToString() + ", " + directionX.ToString() + ", " + directionY.ToString();
		//Debug.Log("POINT: " + pixelX.ToString() + ", " + pixelY.ToString() + ", " + directionX.ToString() + ", " + directionY.ToString());
		//EndDebug

		return new Vector2(originMin.x + halfPixelSize.x + (halfPixelSize.x * pixelX * 2) + (directionX * halfPixelSize.x), originMin.y + halfPixelSize.y + (halfPixelSize.y * pixelY * 2) + (directionY * halfPixelSize.y));

	}



	Queue<Vector2> AddAllVerticesOfPixelToQueue(int pixelX, int pixelY) {
		Queue<Vector2> newVertices = new Queue<Vector2>();
		
		newVertices.Enqueue(new Vector2(originMin.x + (halfPixelSize.x * pixelX * 2), originMin.y + (halfPixelSize.y * pixelY * 2) + halfPixelSize.y));
		newVertices.Enqueue(new Vector2(originMin.x + (halfPixelSize.x * pixelX * 2) + halfPixelSize.x, originMin.y + (halfPixelSize.y * pixelY * 2)));
		newVertices.Enqueue(new Vector2(originMin.x + (halfPixelSize.x * pixelX * 2), originMin.y + (halfPixelSize.y * pixelY * 2) - halfPixelSize.y));
		newVertices.Enqueue(new Vector2(originMin.x + (halfPixelSize.x * pixelX * 2) - halfPixelSize.x, originMin.y + (halfPixelSize.y * pixelY * 2)));

		return newVertices;
	}



	Vector2[] ExtendEdgesOfVertices(Queue<Vector2> obstacleCoords) {
		float minX = obstacleCoords.Peek().x;
		float maxX = obstacleCoords.Peek().x;
		float minY = obstacleCoords.Peek().y;
		float maxY = obstacleCoords.Peek().y;

		Vector2[] points = obstacleCoords.ToArray();

		for (int i = 0; i < points.Length; i++) {
			if(points[i].x < minX) {
				minX = points[i].x;
			}
			if (points[i].x > maxX) {
				maxX = points[i].x;
			}
			if (points[i].y < minY) {
				minY = points[i].y;
			}
			if (points[i].y > maxY) {
				maxY = points[i].y;
			}
		}

		for (int i = 0; i < points.Length; i++) {

			if (minX == maxX) {
				if(i < points.Length/2) {
					points[i].Set(points[i].x + halfPixelSize.x, points[i].y);
				} else {
					points[i].Set(points[i].x - halfPixelSize.x, points[i].y);
				}
			} else if (points[i].x == minX) {
				points[i].Set(points[i].x - halfPixelSize.x, points[i].y);
			} else if (points[i].x == maxX) {
				points[i].Set(points[i].x + halfPixelSize.x, points[i].y);
			}

			if (minY == maxY) {
				if (i < points.Length / 2) {
					points[i].Set(points[i].y, points[i].y + halfPixelSize.y);
				} else {
					points[i].Set(points[i].y, points[i].y - halfPixelSize.y);
				}
			} else if (points[i].y == minY) {
				points[i].Set(points[i].y, points[i].y - halfPixelSize.y);
			} else if (points[i].y == maxY) {
				points[i].Set(points[i].y, points[i].y + halfPixelSize.y);
			}
		}

		return points;
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
