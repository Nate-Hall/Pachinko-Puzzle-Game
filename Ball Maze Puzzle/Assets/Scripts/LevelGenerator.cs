using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour {

	public Transform backboard;
	public Transform tileBack;

	public string levelCode;

	public Transform gridParent;



	private void Start() {
		levelCode = "123";
		GenerateLevelObjects(LoadLevelFromFile(levelCode));
	}



	public LevelDetails LoadLevelFromFile(string filename) {
		LevelDetails level = new LevelDetails();
		level.chapter = int.Parse(filename.Substring(0, 1));
		level.levelNumber = int.Parse(filename.Substring(1, 2));

		// convert text to array and load
		string line = "";

		// Read the file and display it line by line.  
		System.IO.StreamReader file = new System.IO.StreamReader("Assets/Resources/" + filename + ".txt");

		if ((line = file.ReadLine()) != null) {
			foreach (string str in line.Split(' ')) {
				if (str != " ") {
					switch(str.Substring(0, 1)) {
						case "r":
							level.rows = int.Parse(str.Substring(1));
							break;
						case "c":
							level.columns = int.Parse(str.Substring(1));
							break;
						case "d":
							level.divisions = int.Parse(str.Substring(1));
							break;
						case "w":
							level.windDirection = int.Parse(str.Substring(1));
							break;
					}
				}
			}
		}

		level.gridValues = new int[(level.divisions * level.rows) + level.rows + 1, (level.divisions * level.columns) + level.columns + 1];
		int x = 0;
		int y = level.gridValues.GetLength(1) - 1;
		while ((line = file.ReadLine()) != null) {
			foreach (string str in line.Split(' ')) {
				if (str != " ") {
					level.gridValues[x, y] = int.Parse(str);
					x++;
				}
			}
			x = 0;
			y--;
		}

		file.Close();

		return level;
	}



	void GenerateLevelObjects(LevelDetails level) {
		int[,] cellValues = new int[level.divisions, level.divisions];

		for (int row = 0; row < level.rows; row++) {
			for (int column = 0; column < level.columns; column++) {
				for (int i = 0; i < level.divisions; i++) {
					for (int j = 0; j < level.divisions; j++) {
						cellValues[j, i] = level.gridValues[1 + row + (row * level.divisions) + j, (1 + column + (column * level.divisions) + i)];
					}
				}
				List<Transform> obstacles = LevelMeshGenerator.GenerateMeshes(cellValues);
				Transform grid = new GameObject().transform;
				grid.name = "Cell: " + row.ToString() + ", " + column.ToString();
				grid.parent = gridParent;
				grid.position = new Vector3(((level.rows - 1) * -0.5f) + row, ((level.columns - 1) * -0.5f) + column, 0);
				foreach (Transform obj in obstacles) {
					obj.parent = grid;
					obj.localPosition = Vector3.zero;
				}
			}
		}
	}



	[System.Serializable]
	public struct LevelDetails {
		public int chapter;
		public int levelNumber;
		public int rows;
		public int columns;
		public int divisions;
		public int windDirection;
		public int[,] gridValues;
	}
}
