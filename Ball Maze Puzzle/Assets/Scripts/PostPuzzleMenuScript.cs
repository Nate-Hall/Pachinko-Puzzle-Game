using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PostPuzzleMenuScript : MonoBehaviour {

	GameManager manager;
	Button nextButton;



	private void Start() {
		manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
		nextButton = transform.Find("NextButton").GetComponent<Button>();
		CheckActiveButtons();
	}


	public void CheckActiveButtons() {
		if(manager.CanLoadNextLevel()) {
			nextButton.interactable = true;
		} else {
			nextButton.interactable = false;
		}
	}



	public void OnMenuButtonClick() {

	}



	public void OnReplayButtonClick() {

	}



	public void OnNextButtonClick() {
		if(manager.CanLoadNextLevel()) {
			manager.LoadNextLevel();
		} else {
			OnMenuButtonClick();
		}
	}
	
}
