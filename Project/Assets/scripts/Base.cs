using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Base : MonoBehaviour {

	private List<GameObject> controlSceneObj; //all objects in the scene
	private bool isMenu, hasChanged;	//this is set when there is a transition
	private GameObject fadeOutChildren; //only one complex object can fade out in a transition;
	public string songName;			//maybe have this songName and use Resource.Load
	private bool areFadingOut;			//Fade Effect for the children
	private float fadeOutAlpha;
	private bool isInnerClosed;			//needed only for menu Scene;
	private bool isFirstFrame;			//First Frame after everything is loaded.
	private bool isStartGame;		//Check if the Coroutine start game is to be called; 
	private bool isFirstTime;		//is the game loading for the first time?
	private GameObject player, control, cam; //so player and spawner objects will always be referenced
	private float startAnimTime, animTime;

	//for countdown
	private float countStartTime;
	public float countTime;
	public int countNo = 3;
	private int countDown;
	private bool isStartTimeSet;
	public GameObject timer;

	// Use this for initialization
	void Awake () {
		//Start with menu
		isMenu = true;

		//Coroutines required as loading takes one frame
		StartCoroutine("myLoadLevel", "menu");
		StartCoroutine("myLoadLevel", "MasterScene");

		//make the menu transition
		hasChanged = true;

		isStartGame = false;
		isFirstFrame = true;

		//animation
		isFirstTime = true;
		startAnimTime = Time.time;
		animTime = 2;
		countTime = countTime / countNo;
	}
	
	// Update is called once per frame
	void Update () {

/* any initial configuration should be done here*/
		if(isFirstFrame && controlSceneObj != null )
		{
			cam = findInList(controlSceneObj,"Menu Camera");
			player = findInList(controlSceneObj,"Player");
			control = findInList(controlSceneObj,"Control");

			GameObject obj = findInList(controlSceneObj,"GameOverTitle");
			obj.GetComponent<GameOverUI>().cam = cam;
			cam.GetComponent<AudioSource>().clip = (AudioClip)Resources.Load ("MenuMusic");
			cam.GetComponent<AudioSource>().Play();
			//obj = findInList(controlSceneObj, "LevelGroup");
			//Activate(obj,false);	
			
/* let the progress bar appear at the beginning of the game */
			obj = findInList(controlSceneObj,"ProgressRing");
			obj.GetComponent<Progress>().src = cam.GetComponent<AudioSource>();
			obj.GetComponent<Progress>().setLength(cam.GetComponent<AudioSource>().clip.length);
			obj.GetComponent<Progress>().resetToInit();
			isFirstFrame = false;

//trying to pulsate
			Pulsate.src = cam.GetComponent<AudioSource>();
			//resetAll();
		}

/* closing the inner platform */
//		if(!isInnerClosed && controlSceneObj!=null)
//		{
//			if(control.GetComponent<Spawner>().getInner() == null)
//				isInnerClosed = false;
//			else
//			{
//				GameObject inPlat = control.GetComponent<Spawner>().getInner();
//				foreach(Transform t in inPlat.transform)
//				{
//					t.position = inPlat.GetComponent<Platform>().initRadius * 1.2f * t.position.normalized;
//				}
//				ActivateAllChildren(inPlat, true);
//				isInnerClosed = true;
//			}
//		}

		if(controlSceneObj!=null && hasChanged)
		{
			if(isMenu)
			{
				changeToMenu(); //Maybe called from anywhere
				hasChanged = false;//May not be needed after all :)

			}
			else
			{
//				if(fadeOutChildren == null)
//				{
//				if(controlSceneObj == null)
//				{
//					Debug.Log ("control is deactivated");
//					return;
//				}
//				GameObject obj = findInList(controlSceneObj,"Player"));
//				if(obj == null)
//				{
//					Debug.Log ("cannot find player");
//					return;
//				}
//				//obj.transform.position = Vector2.Lerp(obj.transform.position,Vector2.zero,Time.deltaTime*3);
//				if(!obj.GetComponent<PlayerScript>().isLerping)
//				{
//					hasChanged = false;
//					obj.rigidbody2D.isKinematic = false;
//				}
//				}
			}
		}
		if (areFadingOut)
		{
			applyFadeOutToChildren();
		}

/*start the game only if the inner platform is closed*/
		if(isStartGame)
		{

			if(control.GetComponent<Spawner>().getInner() == null && !isStartTimeSet)
			{
				countStartTime = Time.time;
				isStartTimeSet = true;
				countDown = countNo;
				timer.GetComponent<GUIText>().enabled = true;
				timer.GetComponent<GUIText>().text = countDown+"";

			}
			if(isStartTimeSet && (Time.time - countStartTime) >= countTime)
			{
				countDown--;
				timer.GetComponent<GUIText>().text = countDown+"";
				countStartTime = Time.time;
//				if(countDown == 0)
//				{
					isStartGame = false;
					isStartTimeSet = false;
					timer.GetComponent<GUIText>().enabled = false;
					startGame();
//				}
			}
		}
/*---------------------------------------------------------------*/

		if(isFirstTime && ((Time.time - startAnimTime) > animTime))
		{
			Activate(findInList(controlSceneObj,"keyboard"),false);
			isFirstTime = false;
		}
	
	}

	private IEnumerator myLoadLevel(string scene)
	{
		Application.LoadLevelAdditive(scene);
		yield return new WaitForEndOfFrame();
		GameObject[] allObjects = FindObjectsOfType(typeof(GameObject)) as GameObject[];
		controlSceneObj = allObjects.ToList();
	}

	public void changeToMenu()
	{
		isMenu = true; 
		GameObject obj = findInList(controlSceneObj,"Menu_handle");
		Activate(obj,true);
		ActivateAllChildren(obj,true);
		obj.GetComponentInChildren<selection>().reReadHighScores();
		//MakeAllChildrenVisible(obj);
		obj = findInList(controlSceneObj,"LevelGroup");
		Activate(obj,false);
		obj = findInList(controlSceneObj,"GameOverTitle");
		Activate(obj,false);
		obj = findInList(controlSceneObj,"DataCollector");
		Activate(obj,false);
		player.transform.position = new Vector3(0,0,0);
		//setPlatform();
	}

	public void changeToScene()
	{
		player.transform.position = new Vector3(0,0,0);
		isStartGame = true;
//		//hasChanged = true;
		if(isMenu)
		{
			/*called from the menu scene */
			resetAll(); //just to be sure that everything is as expected.
			isStartGame = true;
//			startGame();
		}
		else
		{
			/*called from retry*/
			GameObject obj = findInList(controlSceneObj,"GameOverTitle");
			obj.GetComponent<GameOverUI>().resetToInit();
			obj = findInList(controlSceneObj,"LevelGroup");
			Activate(obj,false);
			isStartGame = true;
//			startGame();
		}
//		isMenu = false;
	}

	private void setPlatform()
	{
		//destroyChildrenWithFade(obj.GetComponent<Spawner>().getOuter());
		//obj.GetComponent<Spawner>().getOuter().GetComponent<Platform>().fadeOut();
		GameObject inPlat = control.GetComponent<Spawner>().getInner();
		if(inPlat == null)
			isInnerClosed = false;
		else
		{

			foreach(Transform t in inPlat.transform)
			{
				t.position = inPlat.GetComponent<Platform>().initRadius * 1.2f * t.position.normalized;
			}
			ActivateAllChildren(control.GetComponent<Spawner>().getInner(), true);
			isInnerClosed = true;
		}
	}

	protected void Activate(GameObject g, bool value)
	{
		if(g != null)
			g.SetActive(value);
	}
	
	protected void ActivateAllChildren(GameObject g, bool value)
	{
		if(g == null)
			return;

		foreach(Transform t in g.transform)
		{
			t.gameObject.SetActive(value);
		}
	}
	protected void destroyChildrenWithFade(GameObject g)
	{
		fadeOutChildren = g;
		areFadingOut = true;
		fadeOutAlpha = 1; 
	}
	void MakeAllChildrenVisible(GameObject g)
	{
		if(g == null)
			return;
		Color c;
		foreach(Transform t in g.transform)
		{
			c = t.renderer.material.color;
			c.a = 1;
			t.renderer.material.color = c;
		}
	}
	private void applyFadeOutToChildren(){
		if(fadeOutChildren == null)
		{
			areFadingOut = false;
			return;
		}
		fadeOutAlpha = Mathf.Lerp(fadeOutAlpha, 0, Time.deltaTime*2);
		foreach(Transform child in fadeOutChildren.transform)
		{
			child.renderer.material.SetColor("_Color", new Color(1,1,1,fadeOutAlpha));
			Debug.Log(fadeOutAlpha);
			if(fadeOutAlpha <= 0.01f)
			{
				Activate(child.gameObject,false);
			}
		}
		if(fadeOutAlpha <= 0.01f)
		{
			areFadingOut = false;
			fadeOutChildren = null;
			fadeOutAlpha = 1;
		}
	}

/*call with true if it should be pushed in
 *else with false. Update the rigidbody accordingly.
 *Disable/Enable SpriteRenderer of Player.
*/
	private void setPlayer(bool inside)
	{
		if(inside)
		{
			//obj.renderer.enabled = true;
			player.transform.position = new Vector3(0,0,0);
			player.rigidbody2D.isKinematic = false;
			//obj.GetComponent<PlayerScript>().lerpTo(new Vector3(0,0,0));
		}
		else
		{
			//ActivateAllChildren(obj,false);
			player.transform.position = new Vector3(0,0,0);
			player.rigidbody2D.isKinematic = true; 
			//obj.transform.localPosition = new Vector3(-7,0,0);
			//obj.renderer.enabled = false;
		}
	}

	private GameObject findInList(List<GameObject> l, string name)
	{
		foreach(GameObject item in l)
		{
			if(item == null)
				continue;
			if(item.name == name)
				return item;
		}
		return null;
	}

	private void resetAll()
	{
		foreach(GameObject g in controlSceneObj)
		{
			if(g!=null)
			{
				if(g.name == "Control")
				{
					g.GetComponent<Spawner>().resetLevelCount();
					g.GetComponent<ConfigLoader>().initConfig();
				}
				else if(g.name == "GameOverTitle")
					g.GetComponent<GameOverUI>().resetToInit();
				else if (g.name == "Player")
					g.GetComponent<PlayerScript>().resetGameOver();
				Progress.singleton.resetToInit();
			}
		}
	}

	private void startGame()
	{		
		GameObject obj = findInList(controlSceneObj,"LevelGroup");
		Activate(obj,true);
		LevelText lt = obj.GetComponent<LevelText>();
		lt.show();

		if(isMenu)
		{
			obj = findInList(controlSceneObj,"Menu_handle");
			Activate(obj, false);
			obj = findInList(controlSceneObj,"GameOverTitle");
			Activate(obj,true);
			obj = findInList(controlSceneObj,"DataCollector");
			Activate(obj,true);
			cam.GetComponent<AudioSource>().clip = cam.GetComponent<CameraScript>().getSong();
			cam.GetComponent<AudioSource>().Play();
			isMenu = false;
		}
		resetAll();
		player.rigidbody2D.isKinematic = false;
	}
}
