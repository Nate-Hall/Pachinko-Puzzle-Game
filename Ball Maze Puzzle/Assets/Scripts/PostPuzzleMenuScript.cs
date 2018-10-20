using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PostPuzzleMenuScript : MonoBehaviour {

	GameManager manager;
	Button nextButton;
	Animator anim;
	Animator lidAnim;



	private void Start() {
		manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
		nextButton = transform.Find("NextButton").GetComponent<Button>();
		anim = GetComponent<Animator>();
		lidAnim = GameObject.FindGameObjectWithTag("Front").GetComponent<Animator>();
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
		StartCoroutine("RestartLevelAnimation");
		Deactivate();
	}



	public void OnNextButtonClick() {
		if(manager.CanLoadNextLevel()) {
			StartCoroutine("LoadNextLevelAnimation");
		} else {
			OnMenuButtonClick();
		}

		Deactivate();
	}



	IEnumerator FadeToMainMenu() {
		lidAnim.SetTrigger("ClosePanel");
		yield return new WaitForSeconds(1);
		GameObject.FindGameObjectWithTag("ScreenFade").GetComponent<Animator>().SetTrigger("Darken");
		yield return new WaitForSeconds(1);
		SceneManager.LoadScene("MainMenu");
	}



	IEnumerator LoadNextLevelAnimation() {
		lidAnim.SetTrigger("ClosePanel");
		yield return new WaitForSeconds(1.5f);
		manager.LoadNextLevel();
		yield return new WaitForSeconds(0.1f);
		lidAnim.SetTrigger("OpenPanel");
		yield return new WaitForSeconds(1.5f);

		manager.SetInteractive(true);
	}



	IEnumerator RestartLevelAnimation() {
		lidAnim.SetTrigger("ClosePanel");
		yield return new WaitForSeconds(1.5f);
		manager.ResetLevel();
		yield return new WaitForEndOfFrame();
		lidAnim.SetTrigger("OpenPanel");
		yield return new WaitForSeconds(1.5f);

		manager.SetInteractive(true);
	}
}
