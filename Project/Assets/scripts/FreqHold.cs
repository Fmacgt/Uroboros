
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// stores and provide delayed freq-generated angular acceleration
public class FreqHold : MonoBehaviour
{
	public struct Config
	{
		public AudioSource src;
		public int channelNum;
		public int sampleNum;
		public int[] samplePts;
		public int weightNum;
		public float[] weights;
		public float delay;
	}

	// the audio clip for extracting frequency
	public AudioSource src;
	public int channelNum = 256;

	// the frequency to use for sampling
	//public int freq = 0;

	public int[] samplePts;
	public float[] weights;
	public float maxAmp = 0.5f;

	// how many seconds delay
	public float delay = 0.3f;

	// for volume "diving" after player enters a ring
	public float initValue = 0.6f;		// initial/regular volume before/after diving
	public float p0 = -0.5f;			// volume at 'control point 0'
	public float p1 = 1.5f;				// volume at 'control point 1'
	public float transitValue = 0.75f;	// volume when transition begins
	public float diveTime = 1f;			// total time of diving
	public float transitTime = 0.2f;	// time in diveTime for transition

	private bool diving = false;
	private float diveTicks = 0f;
	private float e, f, g;				// for bezier
	private float m, n, t;				// still for bezier

	private Queue<float> timeQueue;
	private Queue<float> ampQueue;

	// used for current, instead of delayed amp
	private float currentAmp;
	private float delayedAmp;
	private float avgAmp;
	private float sum;

	private float[] samples;
	private float ticks = 0.0f;
	
	private float len;

	public bool activated;

	void Awake()
	{
		timeQueue = new Queue<float>();
		ampQueue = new Queue<float>();

		samples = new float[channelNum];
		sum = 0.0f;

		activated = false;
	}

	void Update()
	{
		if (activated) {
			ticks += Time.deltaTime;

			samples = src.GetSpectrumData(channelNum, 0, FFTWindow.BlackmanHarris);

			float amp = _sampleAmp();
			sum += amp;
			//Debug.Log(amp);

			// append new data to the queue
			timeQueue.Enqueue(ticks);
			ampQueue.Enqueue(amp);

			currentAmp = amp;

			_filter();

			if (diving) {
				diveTicks += Time.deltaTime;
				if (diveTicks < diveTime - transitTime) {
					// stage 1: change the volume using bezier interpolation
					t = diveTicks / (diveTime - transitTime);

					// bezier interpolation
					e = Mathf.Lerp(initValue, p0, t);
					f = Mathf.Lerp(p0, p1, t);
					g = Mathf.Lerp(p1, transitValue, t);

					m = Mathf.Lerp(e, f, t);
					n = Mathf.Lerp(f, g, t);

					src.volume = Mathf.Clamp(Mathf.Lerp(m, n, t), 0f, 1f);
				} else if (diveTicks < diveTime) {
					// stage 2: transit the volume back to the initial value linearly
					t = (transitTime + diveTicks - diveTime) / transitTime;

					src.volume = Mathf.Clamp(
							Mathf.Lerp(transitValue, initValue, t), 0f, 1f);
				} else {
					// end of diving, reset the volume
					src.volume = initValue;

					diving = false;
				}
			}
		}
	}

	///////////////////////////////////////////////////////////////////////////
	
	public void setConfig(Config config)
	{
		// copy values
		src = config.src;
		//src.volume = initValue;

		channelNum = config.channelNum;
		samples = new float[channelNum];

		samplePts = new int[config.sampleNum];
		for (int i = 0; i < config.sampleNum; i++) {
			samplePts[i] = config.samplePts[i];
		}

		weights = new float[config.weightNum];
		for (int i = 0; i < config.weightNum; i++) {
			weights[i] = config.weights[i];
		}

		delay = config.delay;

		// reset all data
		timeQueue.Clear();
		ampQueue.Clear();

		sum = 0f;
		ticks = 0f;
		delayedAmp = 0f;
		currentAmp = 0f;

		activated = false;
	}

	public void activate()
	{
		activated = true;
	}

	public void deactivate()
	{
		activated = false;
	}

	public void dive()
	{
		diving = true;
		diveTicks = 0f;
	}

	// obtain the amplitude at the moment
	public float getAmp()
	{
		//Debug.Log(delayedAmp);
		return delayedAmp;
	}

	public float getCurrentAmp()
	{
	//	Debug.Log(currentAmp);
		return currentAmp;
	}

	public float getAvgAmp()
	{
		if (sum < 0.0f || delay <= 0.0005f)
			return 0.0f;

//		Debug.Log("Count: " + ampQueue.Count + ", " + sum/ampQueue.Count);
		return sum / ampQueue.Count;
	}

	void _filter()
	{
		delayedAmp = 0.0f;

		float t0 = timeQueue.Peek();
		while (ticks - t0 > delay) {
			delayedAmp = ampQueue.Peek();
			sum -= delayedAmp;

			// pop it up
			timeQueue.Dequeue();
			ampQueue.Dequeue();

			// try to get the latest by checking the next time
			t0 = timeQueue.Peek();
		}
	}

	float _sampleAmp()
	{
		float sum = 0.0f;

		for (int i = 0; i < samplePts.Length; i++) {
			sum += samples[samplePts[i]] * weights[i];
		}

		return (sum > maxAmp) ? maxAmp : sum;
	}
}
