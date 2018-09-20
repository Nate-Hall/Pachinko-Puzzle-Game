using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditorUI : MonoBehaviour {

	bool textPromptUp = false;
	string filename = "";

	private void OnGUI() {
		if (GUI.Button(new Rect(Screen.width * 0.89f, Screen.height * 0.1f, Screen.width * 0.1f, Screen.height * 0.15f), "Clear")) {
			GetComponent<LevelEditor>().InitGrid();
		}

		if (GUI.Button(new Rect(Screen.width * 0.89f, Screen.height * 0.3f, Screen.width * 0.1f, Screen.height * 0.15f), "Save")) {
			if (!textPromptUp) {
				textPromptUp = true;
			}
		}

		if(textPromptUp) {
			GUI.Label(new Rect(Screen.width * 0.5f, Screen.height * 0.2f, Screen.width * 0.8f, Screen.height * 0.2f), "Enter File Name:");
			GUI.TextField(new Rect(Screen.width * 0.5f, Screen.height * 0.6f, Screen.width * 0.8f, Screen.height * 0.2f), filename, 100);

			if(Input.GetKey(KeyCode.Return)) {
				textPromptUp = false;
				GetComponent<LevelEditor>().SaveGrid(filename);
				Debug.Log("test");

				//BUG: Doesn't close if text has been entered
			}
		}
	}
}
