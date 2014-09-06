/***************************************************************
 * Controls the movement of the Ball.
 ***************************************************************
 * Change Log
 ***************************************************************
 * 140419
 * Making it restart only when the ball is fully BELOW the 
 * screen (K)
 **************************************************************/
using UnityEngine;
using System.Collections;

public class player : MonoBehaviour {
	public float timeInterval = 1;
	private float startTime;
	void Start()
	{
		startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if((Time.time - startTime) >= timeInterval)
		{
			rigidbody2D.velocity = new Vector2(2f,5f);
			startTime = Time.time;
		}
	}

}
