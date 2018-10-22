using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gradienttexture : MonoBehaviour {

	float falloff = 0;

	// Use this for initialization
	void Start () {
		Texture2D tex = new Texture2D(128, 128);
		//tex.filterMode = FilterMode.Point;

		for (int x = 0; x < 128; x++) {
			for (int y = 0; y < 128; y++) {
				tex.SetPixel(x, y, Color.blue);
			}
		}

		tex.Apply();
		GetComponent<Renderer>().material.mainTexture = tex;
	}
}
