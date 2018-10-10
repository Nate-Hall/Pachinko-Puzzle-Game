using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	private int selectedTileX, selectedTileY;
	private Vector3 selectedTileStartPos;
	public GUIStyle style;




	private void Start() {
		style.fontSize = 30;
		style.normal.textColor = Color.white;
	}



	private void OnGUI() {
		GUI.Label(new Rect(Screen.width/2 - 200, Screen.height - 90, 200, 200), "Press \"Spacebar\" to start", style);
	}



	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonUp(0)) {
			Transform obj = SelectTile();
			if (obj != null) {
				obj.GetComponent<CellBehaviour>().GrabCell();
			}
		}
	}



	private Transform SelectTile() {
		RaycastHit hit = new RaycastHit();
		if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, float.MaxValue, 1 << 8)) {
			return hit.transform;
		}
		return null;
	}
}
