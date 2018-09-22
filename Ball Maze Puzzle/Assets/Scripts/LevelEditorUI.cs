using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditorUI : MonoBehaviour {

	bool textPromptUp = false;
	public string filename = "";

	private void OnGUI() {

		if(textPromptUp) {
			GUI.Label(new Rect(Screen.width * 0.5f, Screen.height * 0.2f, Screen.width * 0.8f, Screen.height * 0.2f), "Enter File Name:");
			GUI.TextArea(new Rect(Screen.width * 0.5f, Screen.height * 0.6f, Screen.width * 0.8f, Screen.height * 0.2f), filename, 100);

			if(Input.GetKey(KeyCode.Return)) {
				textPromptUp = false;
				GetComponent<LevelEditor>().SaveGrid(filename);
				Debug.Log("test");

				//BUG: Doesn't close if text has been entered
			}
		}
	}


	public void OnClearButtonClicked() {
		GetComponent<LevelEditor>().InitGrid();
	}



	public void OnSaveButtonClicked(string filename) {
		this.filename = filename;
		if (!textPromptUp) {
			textPromptUp = true;
		}
	}

}
