
using UnityEngine;
using System.Collections;

// 
public class Spawner : MonoBehaviour
{
	public struct Config
	{
		public float diffCoeff;
		public float platformRatio;
	}

	// prefab for the levels
	public GameObject levelPrefab;

	public float diffCoeff = 12.0f;
	public float platformRatio = 1.0f;

	// the two levels at any moment
	public GameObject inner;
	public GameObject outer;

	public LevelText levelText;

	public Progress progressRing;

	private PlayerScript plyr;
	private CameraScript camObject;
	private int levelCount=1;
	private float maxSpeed = 20f;
	private Platform outerPlatform = null;

	void Start()
	{
		inner = null;
		outer = null;

		plyr = GameObject.Find ("Player").GetComponent<PlayerScript>();
		camObject = GameObject.Find("Menu Camera").GetComponent<CameraScript>();
	}

	void Update()
	{
		if (outerPlatform != null && progressRing != null) {
			progressRing.setStrength(outerPlatform._getAngSpeed() * 10.0f / maxSpeed + 5f);
		}
	}

	///////////////////////////////////////////////////////////////////////////
	
	public void setConfig(Config config)
	{
		diffCoeff = config.diffCoeff;
		platformRatio = config.platformRatio;
	}

	// create a new level
	// NOTE: called when the (outer) level reaches the outer ring
	public void spawn(float low)
	{
		// create a new inner level, at the position of the spawner's game object
		// use default rotation, may switch to random angle later?
		inner = (GameObject)Instantiate(levelPrefab, transform.position,
				Quaternion.identity);

		// get the difficulty range based on the average 'speed' of past
		// time span
		low *= diffCoeff;
		if (low <= 0.0005f)
			low = 6f;

		//Debug.Log("diff: " + low);

		Platform platform = inner.GetComponent<Platform>();
		platform.generate(low, low * 2f, levelCount);
		platform.ratio = platformRatio;
	}

	public bool isEmpty()
	{
		return inner == null && outer == null;
	}

	public GameObject getInner(){
				return inner;
		}

	public GameObject getOuter(){
		return outer;
	}
	// move the inner level into the 'ring' of outer level
	// NOTE: called when the player enters the inner level
	public void changeRing()
	{
		if(camObject == null)
			camObject = GameObject.Find("Menu Camera").GetComponent<CameraScript>();
		else
			if(!plyr.rigidbody2D.isKinematic)
				camObject.jiggle = true;

		if (outer == null) {
			// there should be one and only one ring now
			outer = GameObject.Find("Platform");
			Platform plat = outer.GetComponent<Platform>();
			plat.ratio = platformRatio;

			outerPlatform = plat;
		} else {
			levelCount++;
			//if(levelCount%5==0)
				plyr.IncreaseJumps(levelCount);
			levelText.setValue (levelCount);

			Platform plat=outer.GetComponent<Platform>();
			plat.die();

			outer = inner;
			inner = null;

			outerPlatform = outer.GetComponent<Platform>();
		}
	}

	public void resetLevelCount()
	{
		if(getInner() == null)
			levelCount = 1;
		else
			levelCount = 0;
		levelText.setValue(levelCount);
	}
	/*
	void OnGUI()
	{
		GUI.Label(new Rect(300, 0, 100, 40), "Level: " + levelCount);
	}*/
}
