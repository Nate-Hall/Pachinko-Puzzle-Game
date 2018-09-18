using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	private GameObject selectedTile;
	private Vector3 selectedTilePos;
	public GUIStyle style;

	public Transform selectParticle;

	public Transform frontPanel;
	private Animator frontPanelAnimator;



	private void Start() {
		style.fontSize = 30;
		style.normal.textColor = Color.white;

		frontPanelAnimator = frontPanel.GetComponent<Animator>();
		frontPanelAnimator.SetTrigger("OpenPanel");
	}



	private void OnGUI() {
		GUI.Label(new Rect(Screen.width/2 - 200, Screen.height - 90, 200, 200), "Press \"Spacebar\" to start", style);
	}



	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonUp(0)) {
			GameObject newTile = SelectTile();
			if(newTile == null || newTile == selectedTile) {
				selectedTile = null;
				selectParticle.GetComponent<ParticleSystem>().Stop();
				selectParticle.position = new Vector3(0, -100, 0);
			} else if(selectedTile == null) {
				selectedTile = newTile;
				selectedTilePos = selectedTile.transform.position;
				selectParticle.position = new Vector3(selectedTilePos.x, selectedTilePos.y, 0);
				selectParticle.GetComponent<ParticleSystem>().Play();
			} else {
				selectedTile.transform.position = newTile.transform.position;
				newTile.transform.position = selectedTilePos;
				selectedTile = null;
				selectParticle.GetComponent<ParticleSystem>().Stop();
				selectParticle.position = new Vector3(0, -100, 0);
			}

		}
	}



	private GameObject SelectTile() {
		RaycastHit hit = new RaycastHit();
		if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, float.MaxValue, 1 << 8)) {
			return hit.transform.gameObject;
		}
		Debug.Log("Pressed");
		return null;
	}
}
