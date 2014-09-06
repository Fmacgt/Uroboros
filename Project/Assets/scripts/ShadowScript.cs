using UnityEngine;
using System.Collections;

public class ShadowScript : MonoBehaviour {

	private float fadeAlpha = 0.3f; 
	private float fadeSpeed = 1.0f;	
	
	private float scale = 1.0f;
	private float scaleDelta = 0.1f;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		scale+=scaleDelta;
		
		transform.localScale = new Vector3(scale,scale,scale);
		
		fadeAlpha -= fadeSpeed * Time.deltaTime;
		if (fadeAlpha <= 0.0f) {
			// destroy the platform after it fades out
			GameObject.Destroy(gameObject);
		}
		
		gameObject.renderer.material.SetColor("_Color", new Color(1f, 1f, 1f, fadeAlpha));
	}
}
