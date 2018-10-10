using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellBehaviour : MonoBehaviour {

	public bool locked = false;

	public bool selected = true;
	private bool wasSelected = false;

	public Vector3 setPosition;

	public float shiftSpeed = 1;
	public float shakeSpeed = 5;
	public float shakeIntensity = 1;
	public float snapDistance = 0.1f;

	private float shakeTimer = 0;
	

	private void Update() {
		if(transform.localPosition != setPosition && !selected) {
			transform.localPosition += (setPosition - transform.localPosition).normalized * shiftSpeed * Time.deltaTime;
			if((setPosition - transform.localPosition).magnitude < snapDistance) {
				transform.localPosition = setPosition;
			}
		}

		if(selected) {
			shakeTimer += Time.deltaTime * shakeSpeed;
			transform.rotation = Quaternion.Euler(0, 0, Mathf.Sin(shakeTimer) * 10 * shakeIntensity);
		} else if(wasSelected) {
			wasSelected = false;
			transform.rotation = Quaternion.Euler(0, 0, 0);
		}
	}



	public void GrabCell() {
		selected = true;
		wasSelected = true;
		shakeTimer = 0;
	}
}
