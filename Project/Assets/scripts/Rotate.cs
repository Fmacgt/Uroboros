using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour {

	public float speed = 0.1f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
//		foreach(Transform child in transform)
//		{
//			child.Rotate(Vector3.back,speed*Time.deltaTime);
//		}

		transform.Rotate(new Vector3(0,0,-1),speed*Time.deltaTime);
	}
}
