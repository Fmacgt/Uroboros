
using UnityEngine;
using System.Collections;

public class UpdateStrength : MonoBehaviour
{
	public float interval = 1.0f;
	public float baseInt = 0.05f;

	private float ticks = 0.0f;
	private float coeff = 100.0f;
	
	private float baseStrength = 0.0f;
	private float maxStrength = 10.0f;
	private float strength = 0.0f;
	private float damping = 0.01f;

	private FreqHold freqHold;

	void Start()
	{
		freqHold = GameObject.Find("FreqHold").GetComponent<FreqHold>();
	}

	void Update()
	{
//		ticks += Time.deltaTime;

		strength += getAccel(freqHold.getCurrentAmp()) * Time.deltaTime;
		strength -= strength * damping;

		if (strength > maxStrength)
			strength = maxStrength;
		if (strength < baseStrength)
			strength = baseStrength;

		ticks += strength * Time.deltaTime;
		
		Debug.Log(strength + ", " + ticks);

//		interval = baseInt / (0.0001f + freqHold.getCurrentAmp());
		
		/*
		if (ticks >= interval) {
			coeff *= -1.0f;
		} else if (ticks <= 0.05f) {
			coeff *= -1.0f;
		}
		*/

		renderer.material.SetFloat("_Strength", strength);
		renderer.material.SetFloat("_CurT", ticks);
	}

	float getAccel(float amp)
	{
		/*
		return amp * 100.0f;
		*/
		if (amp > 0.1f) {
			return 5f;
		} else if (amp > 0.05f) {
			return 3.5f;
		} else if (amp > 0.01f) {
			return 2.3f;
		} else if (amp > 0.005f) {
			return 1.15f;
		}

		return 0.5f;
	}
}
