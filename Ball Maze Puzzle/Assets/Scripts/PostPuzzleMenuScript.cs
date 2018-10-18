using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PostPuzzleMenuScript : MonoBehaviour {

	GameManager manager;
	Button nextButton;
	Animator anim;



	private void Start() {
		manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
		nextButton = transform.Find("NextButton").GetComponent<Button>();
		anim = GetComponent<Animator>();
	}



	public void Activate() {
		manager.SetInteractive(false);
		anim.SetTrigger("Appear");
		CheckActiveButtons();
	}



	void Deactivate() {
		anim.SetTrigger("Disappear");
	}



	public void CheckActiveButtons() {
		if(manager.CanLoadNextLevel()) {
			nextButton.interactable = true;
		} else {
			nextButton.interactable = false;
		}
	}



	public void OnMenuButtonClick() {
		Deactivate();
		StartCoroutine("FadeToMainMenu");
	}



	public void OnReplayButtonClick() {
		manager.ResetLevel();
		Deactivate();
		manager.SetInteractive(true);
	}



	public void OnNextButtonClick() {
		if(manager.CanLoadNextLevel()) {
			manager.LoadNextLevel();
		} else {
			OnMenuButtonClick();
		}

		Deactivate();
		manager.SetInteractive(true);
	}



	IEnumerator FadeToMainMenu() {
		GameObject.FindGameObjectWithTag("ScreenFade").GetComponent<Animator>().SetTrigger("Darken");
		yield return new WaitForSeconds(1);
		SceneManager.LoadScene("MainMenu");
	}
}
