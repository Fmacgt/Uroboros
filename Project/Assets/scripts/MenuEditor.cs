using UnityEngine;
using System.Collections;

public class MenuEditor : MonoBehaviour {

	void Start()
	{
		BoxCollider col = (BoxCollider)collider;
		col.size = renderer.bounds.size;
	}
	/*
	public bool isQuit = false;

	void OnMouseEnter()
	{
		renderer.material.color = Color.green;
	}
	void OnMouseExit()
	{
		renderer.material.color = Color.white;
	}
	void OnMouseDown()
	{
		if(isQuit)
			Application.Quit();
		else
		{
			foreach(Transform t in GetComponentInChildren<Transform>())
			{
				t.gameObject.SetActive(true);
			}
		}
	}
	*/
}
