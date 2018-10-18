using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	bool canInteract = false;

	Transform selectedTile;
	Vector3 selectedTileStartPos;

	Transform hoverTile;
	Vector3 hoverTileStartPos;

	public GUIStyle style;

	LevelGenerator levelGen;




	private void Awake() {
		style.fontSize = 30;
		style.normal.textColor = Color.white;

		levelGen = transform.GetComponent<LevelGenerator>();
		SetInteractive(true);
	}



	private void OnGUI() {
		if (canInteract) {
			GUI.Label(new Rect(Screen.width / 2 - 200, Screen.height - 90, 200, 200), "Press \"Spacebar\" to start", style);
		}
	}



	// Update is called once per frame
	void Update () {
		if (canInteract) {
			if (Input.GetMouseButtonDown(0)) {
				selectedTile = SelectTile();
				if (selectedTile != null) {
					selectedTileStartPos = selectedTile.GetComponent<CellBehaviour>().setPosition;
					selectedTile.GetComponent<CellBehaviour>().GrabCell();
				}
			}

			if (selectedTile != null && !Input.GetMouseButton(0)) {
				if (hoverTile != null) {
					selectedTile.GetComponent<CellBehaviour>().setPosition = hoverTileStartPos;
					hoverTile.GetComponent<CellBehaviour>().SetSwap();
				}

				selectedTile.GetComponent<CellBehaviour>().ReleaseCell();
				selectedTile = null;
			} else if (selectedTile != null && Input.GetMouseButton(0)) {
				Transform obj = SelectTile();
				if (hoverTile == null && obj != null && obj != selectedTile) {
					hoverTile = obj;
					hoverTileStartPos = hoverTile.GetComponent<CellBehaviour>().setPosition;
					hoverTile.GetComponent<CellBehaviour>().PreviewSwap(selectedTileStartPos);
				} else if (hoverTile != null) {
					Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 3f));
					if (pos.x < hoverTileStartPos.x - 0.5f || pos.x > hoverTileStartPos.x + 0.5f || pos.y < hoverTileStartPos.y - 0.5f || pos.y > hoverTileStartPos.y + 0.5f) {
						hoverTile.GetComponent<CellBehaviour>().UndoPreviewSwap();
						hoverTile = null;
					}
				}
			}
		}
	}



	private Transform SelectTile() {
		RaycastHit hit = new RaycastHit();
		if (Physics.Raycast(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 2.9f)), Vector3.forward, out hit, 1f, 1 << 8)) {
			return hit.transform;
		}
		return null;
	}



	public bool CanLoadNextLevel() {
		int nextLevel = levelGen.FindNextLevel();
		if(nextLevel != -1 && nextLevel % 100 != 1) {
			return true;
		} else {
			return false;
		}
	}



	public void LoadNextLevel() {
		if (CanLoadNextLevel()) {
			levelGen.SetLevel(levelGen.FindNextLevel());
			levelGen.GenerateLevel();
			SetInteractive(true);
		}
	}



	public void ResetLevel() {
		levelGen.GenerateLevel();
		SetInteractive(true);
	}



	public void SetInteractive(bool canInteract) {
		this.canInteract = canInteract;
	}
}
