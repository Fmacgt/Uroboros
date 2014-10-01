using UnityEngine;
using System.Collections;

public class OrbController : MonoBehaviour {

	private Vector2 origin;
	private bool justActivated;
	private int timer;
	// Use this for initialization
	void Start () {
		timer = 0;
		origin = transform.localPosition;
		justActivated = false;
		GetComponent<ParticleSystem>().enableEmission = false;
		//Debug.Log(transform.position.ToString());
	}
	
	// Update is called once per frame
	void Update () {
		if(justActivated){
			timer++;
			transform.localPosition = Vector3.MoveTowards(transform.localPosition,origin,0.4f);
			if(timer >= 60)
			{
				timer = 0;
				justActivated = false;
				GetComponent<ParticleSystem>().enableEmission = false;
			}
		}
	}
	void OnEnable(){
		GetComponent<ParticleSystem>().enableEmission = true;
		justActivated = true;
		timer = 0;
	}
	void OnDisable(){
		justActivated = false;
		transform.position = transform.localPosition.normalized * 4;
	}
}
