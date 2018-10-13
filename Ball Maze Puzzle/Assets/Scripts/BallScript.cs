using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour {

	private bool active = false;
	private Vector3 startPos;
	float originalSize = 0.1f;
	public float shrinkSpeed = 1;
	SphereCollider col;

	// Use this for initialization
	void Start () {
		startPos = transform.position;
		col = GetComponent<SphereCollider>();
	}



	private void Update() {
		if(!active) {
			GetComponent<Rigidbody>().useGravity = false;
			GetComponent<Rigidbody>().velocity = Vector3.zero;
			col.enabled = false;
		} else {
			GetComponent<Rigidbody>().useGravity = true;
			col.enabled = true;
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



	private void OnTriggerStay(Collider other) {
		if(other.tag == "Goal") {
			if(transform.localScale.x > 0) {
				transform.localScale -= Vector3.one * shrinkSpeed * Time.deltaTime;
			}
			if (transform.localScale.x <= 0.005f) {
				ResetBall();
			}
		}
	}



	public void ResetBall() {
		active = false;
		GetComponent<Rigidbody>().velocity = Vector3.zero;
		transform.position = startPos;
		transform.localScale = Vector3.one * originalSize;
	}
}
