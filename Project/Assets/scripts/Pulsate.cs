using UnityEngine;
using System.Collections;

public class Pulsate : MonoBehaviour {

	public static AudioSource src;
	// Use this for initialization
	void Start () {
		transform.position = new Vector3(Random.Range(-5,8), Random.Range(-5,8),1);
	}
	
	// Update is called once per frame
	void Update () {
		if(src != null)
		{
			float[] spectrum = src.GetSpectrumData(256, 0, FFTWindow.Rectangular);
			Color new_color = Color.Lerp(Color.red, Color.blue, spectrum[0] * 56);
			renderer.material.SetColor("_BaseColor",new_color);
		}
	}
}
