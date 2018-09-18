using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour {

	private bool active = false;
	private Vector3 startPos;

	// Use this for initialization
	void Start () {
		startPos = transform.position;
	}



	private void Update() {
		if(!active) {
			GetComponent<Rigidbody>().useGravity = false;
			GetComponent<Rigidbody>().velocity = Vector3.zero;
		} else {
			GetComponent<Rigidbody>().useGravity = true;
		}

		if(Input.GetKeyDown(KeyCode.Space)) {
			active = !active;
			if(!active) {
				ResetBall();
				Debug.Log("ResetBall");
			}
		}

		if(transform.position.y < -10) {
			ResetBall();
		}
	}



	private void OnTriggerEnter(Collider other) {
		if(other.tag == "Goal") {
			ResetBall();
		}
	}



	public void ResetBall() {
		active = false;
		GetComponent<Rigidbody>().velocity = Vector3.zero;
		transform.position = startPos;
	}
}
