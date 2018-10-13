using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellBehaviour : MonoBehaviour {

	public bool locked = false;

	public bool selected = true;
	private bool wasSelected = false;
	public bool previewSwap = false;

	public Vector3 setPosition;
	Vector3 previewSwapPosition;

	public float shiftSpeed = 1;
	public float shakeSpeed = 5;
	public float shakeIntensity = 1;
	public float snapDistance = 0.1f;

	private float shakeTimer = 0;

	BoxCollider col;



	private void Start() {
		col = GetComponent<BoxCollider>();
	}



	private void Update() {
		if(locked && col.enabled) {
			col.enabled = false;
		}

		if (transform.localPosition != setPosition && !selected && !previewSwap) {
			MoveTowardsPosition(setPosition);
		}

		if (selected) {
			shakeTimer += Time.deltaTime * shakeSpeed;
			transform.rotation = Quaternion.Euler(0, 0, Mathf.Sin(shakeTimer) * 10 * shakeIntensity);
			Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 3));
			transform.position = new Vector3(mousePos.x, mousePos.y, -0.3f);
		} else if(wasSelected) {
			wasSelected = false;
			transform.rotation = Quaternion.Euler(0, 0, 0);
		}

		if(previewSwap) {
			UpdatePreviewSwapPosition();
		}
	}



	void MoveTowardsPosition(Vector3 newPosition) {
		transform.localPosition += (newPosition - transform.localPosition).normalized * shiftSpeed * Time.deltaTime;
		if ((newPosition - transform.localPosition).magnitude < snapDistance) {
			transform.localPosition = newPosition;
			col.enabled = true;
		} else {
			col.enabled = false;
		}
	}



	public void GrabCell() {
		selected = true;
		wasSelected = true;
		shakeTimer = 0;
	}



	public void ReleaseCell() {
		selected = false;
	}



	public void PreviewSwap(Vector3 newPosition) {
		previewSwapPosition = newPosition;
		previewSwap = true;
	}



	public void SetSwap() {
		setPosition = previewSwapPosition;
		previewSwap = false;
	}



	public void UndoPreviewSwap() {
		previewSwap = false;
		previewSwapPosition = setPosition;
	}



	void UpdatePreviewSwapPosition() {
		if(previewSwap && transform.localPosition != previewSwapPosition) {
			MoveTowardsPosition(previewSwapPosition);
		}
	}
}
