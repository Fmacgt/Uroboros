using UnityEngine;
using System.Collections;

public class SongSelecter : MonoBehaviour {

	void Start()
	{
		BoxCollider col = (BoxCollider)collider;
		col.size = renderer.bounds.size;
	}
/*	void OnMouseEnter()
	{
		renderer.material.color = Color.green;
	}
	void OnMouseExit()
	{
		renderer.material.color = Color.white;
	}
	void OnMouseDown()
	{
		Application.LoadLevel(scene);
	}*/
}
