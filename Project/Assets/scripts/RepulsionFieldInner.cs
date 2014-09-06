
using UnityEngine;
using System.Collections;

public class RepulsionFieldInner : MonoBehaviour
{
	public float baseStrength = 400.0f;
	public float energyRatio = 1.0f;
	public float energyDrain = 20.0f;
	public string playerLayer;

	public float shrinkRate = 1f;

	public CircleCollider2D outerRing;

	private bool inEffect = false;
	private PlayerScript player;
	private Vector2 effectForce;

	void Start()
	{
		player = GameObject.Find("Player").GetComponent<PlayerScript>();
	}

	void FixedUpdate()
	{
		if (inEffect) {
			// apply repulsion force adjusted according to the player's energy
			/*
			effectForce = -player.transform.position.normalized * baseStrength *
				(1 + player.energy/player.maxEN) * energyRatio * Time.deltaTime;
			*/
			effectForce = -player.transform.position.normalized * baseStrength * Time.deltaTime;
			player.transform.rigidbody2D.AddForce(effectForce);

			/*
			// inside the field will drain player's energy
			player.energy -= energyDrain * Time.deltaTime;
			if (player.energy < 0f) {
				player.energy = 0f;
			}
			*/

			// every second the player inside the field, it shrinks
			outerRing.radius -= shrinkRate * Time.deltaTime;

			Debug.DrawRay(player.transform.position, effectForce/100f);
			/*
			player.rigidbody2D.AddForce(
					-player.position.normalized * baseStrength * Time.deltaTime);
			*/
		}
	}

	void OnTriggerExit2D(Collider2D obj)
	{
		if (!inEffect &&
				obj.tag == playerLayer) {
			inEffect = true;
		}
	}

	void OnTriggerEnter2D(Collider2D obj)
	{
		if (inEffect &&
				obj.tag == playerLayer) {
			inEffect = false;
		}
	}
}
