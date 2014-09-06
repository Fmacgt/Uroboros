using UnityEngine;
//using UnityEditor;
using System.Collections;

public class selection : MonoBehaviour {
	public string upKey = "Vertical";
	public string selectKey = "Jump";

	public GameObject[] optionList;
	public string[] function;
	public GameObject cam;
	private GameObject[] activeList;
	private GameObject[] prevLevel;
	private int count;
//	private bool turnInactive;
	private bool goDown, goUp; //going down the menu tree or going up
	private Transform parent; //parent of the current expanded list
	private Vector3 destination;
	public float speed, scaleFactor;
	private AudioClip[] au_clips;
	private AudioClip[] previews;
	private AudioClip menuMusic;

//for now will be set when play is selected;using this play a preview.
	private bool isSongList;
	private AudioSource au_previewSource;

//required to fade out 
	private bool isFade;
	private float fadeAlpha, fadeSpeed;
	private Color textColor;

	private bool arrowKeyDown = false;

	// Use this for initialization
	void Start () {
		count = 0;
		foreach(GameObject go in optionList)
		{
			if(go.activeSelf)
				count++;
		}
		activeList = new GameObject[count];
		int i =0;
		foreach(GameObject go in optionList)
		{
			if(go.activeSelf)
			{
				activeList[i] = go;
				i++;
			}
		}
		au_clips = new AudioClip[function.Length];
		previews = new AudioClip[function.Length];
		menuMusic = (AudioClip)Resources.Load("MenuMusic");
		for(i=0; i < au_clips.Length; i++)
		{
			au_clips[i] = (AudioClip) Resources.Load(function[i]); 
			previews[i] = (AudioClip) Resources.Load(function[i]+"Preview"); 
		}
		parent = transform.parent;
//		turnInactive = false;
		goDown = false;
		goUp = false;
		count = 0;
		activeList[count].renderer.material.color = Color.green;
		destination = transform.localPosition;
		isFade = false;
		fadeAlpha = 1;
		textColor = activeList[0].renderer.material.color;
		fadeSpeed = 2;
	}
	
	// Update is called once per frame
	void Update()
	{
		transform.localPosition = Vector3.Lerp(transform.localPosition, destination,Time.deltaTime*speed);
		if(prevLevel != null)
		{
			if(goDown)
			{
				foreach(GameObject g in prevLevel)
				{
					g.transform.localPosition = Vector3.Lerp(g.transform.localPosition, new Vector3(-11, g.transform.localPosition.y, g.transform.localPosition.z),Time.deltaTime*speed);
					if(Mathf.Abs(g.transform.localPosition.x + 11) < 1f)
					{
						//g.SetActive(false);
						g.transform.localPosition = new Vector3(-11, g.transform.localPosition.y, g.transform.localPosition.z);
						transform.localScale = new Vector3(activeList[0].GetComponent<BoxCollider>().size.x * scaleFactor,
						                                   transform.localScale.y,
						                                   transform.localScale.z);
						activeList[0].renderer.material.color = Color.green;
						//renderer.enabled = true;
						goDown = false;
					}
				}
			}

			foreach(GameObject g in activeList)
			{
				g.transform.localPosition = Vector3.Lerp(g.transform.localPosition, new Vector3(0, g.transform.localPosition.y,0),Time.deltaTime*speed);
			}
		}

		if(goUp)
		{
			foreach(GameObject g in activeList)
			{
				g.transform.localPosition = Vector3.Lerp(g.transform.localPosition, new Vector3(0, g.transform.localPosition.y,0),Time.deltaTime*speed);
				if((g.transform.localPosition - new Vector3(0, g.transform.localPosition.y,0)).magnitude  <= 0.1f)
				{
					g.transform.localPosition = new Vector3(0, g.transform.localPosition.y,0);
					goUp = false;
					//renderer.enabled = true;
					transform.localScale = new Vector3(activeList[0].GetComponent<BoxCollider>().size.x * scaleFactor,
					                                   transform.localScale.y,
					                                   transform.localScale.z);
				}
			}

		}
		if(isFade)
			fadeOut();

	}
	void LateUpdate () {
//		if(Input.GetKeyDown(KeyCode.DownArrow))
		if (Input.GetAxis(upKey) != 0f) {
			if (!arrowKeyDown && Input.GetAxis(upKey) < 0f) 
			{
				arrowKeyDown = true;

				if(count< (activeList.Length-1))
				{
					activeList[count].renderer.material.color = Color.white;
					count++;
					destination = activeList[count].transform.localPosition;
					activeList[count].renderer.material.color = Color.green;

				}
				else
				{
					activeList[count].renderer.material.color = Color.white;
					count = 0;
					destination = activeList[count].transform.localPosition;
					//				transform.localScale = new Vector3(activeList[count].GetComponent<BoxCollider>().size.x * scaleFactor,
					//				                                   transform.localScale.y,
					//				                                   transform.localScale.z);
					activeList[count].renderer.material.color = Color.green;
				}

				if(isSongList && count < previews.Length)
				{
					cam.GetComponent<AudioSource>().clip = previews[count];
					cam.GetComponent<AudioSource>().Play();
				}
				else if(isSongList)
				{
					cam.GetComponent<AudioSource>().clip = menuMusic;
					cam.GetComponent<AudioSource>().Play();
				}
			}
			//else if(Input.GetKeyDown(KeyCode.UpArrow)){
			else if(!arrowKeyDown && Input.GetAxis(upKey) > 0f){
				arrowKeyDown = true;

				if(count > 0)
				{
					activeList[count].renderer.material.color = Color.white;
					count--;
					destination = activeList[count].transform.localPosition;
					//				transform.localScale = new Vector3(activeList[count].GetComponent<BoxCollider>().size.x * scaleFactor,
					//				                                   transform.localScale.y,
					//				                                   transform.localScale.z);
					activeList[count].renderer.material.color = Color.green;
					activeList[count].GetComponentInChildren<Renderer>().material.color = Color.green;
				}
				else
				{
					activeList[count].renderer.material.color = Color.white;
					count = activeList.Length-1;
					destination = activeList[count].transform.localPosition;
					activeList[count].renderer.material.color = Color.green;
				}
				if(isSongList && count < previews.Length)
				{
					cam.GetComponent<AudioSource>().clip = previews[count];
					cam.GetComponent<AudioSource>().Play();
				}
				else if(isSongList)
				{
					cam.GetComponent<AudioSource>().clip = menuMusic;
					cam.GetComponent<AudioSource>().Play();
				}
			}
		} else {
			arrowKeyDown = false;
		}

		//if(Input.GetKeyDown(KeyCode.Space))
		if(Input.GetButtonDown(selectKey))
		{
			if(activeList[count].GetComponent<TextMesh>().text == "Play")
			{
				//renderer.enabled = false;
				goDown = true;
				parent = activeList[count].transform;
				GameObject[] temp = activeList;
				int i = 0;
				activeList = new GameObject[temp[count].GetComponent<Transform>().childCount];
				foreach(Transform t in temp[count].transform)
				{
					t.gameObject.SetActive(true);
					activeList[i] = t.gameObject;
					i++;
				}
				i = 0;
				foreach(GameObject go in activeList)
				{
					go.transform.parent = transform.parent;
					if(go.transform.childCount>0)
					{
						float highScore = PlayerPrefs.GetFloat ("HighScore_" + i.ToString());
						go.transform.GetChild(0).GetComponent<TextMesh>().text = "Score To Beat: "+ highScore.ToString();
					}
					i++;
				}
				prevLevel = temp;
				destination = new Vector3(0, activeList[0].transform.localPosition.y,transform.localPosition.z);
				isSongList = true;
				count = 0;
				cam.GetComponent<AudioSource>().clip = previews[count];
				cam.GetComponent<AudioSource>().Play();
			}
			else
			{
				if(activeList[count].GetComponent<TextMesh>().text == "Exit")
				{
					cam.GetComponent<CameraScript>().animateCamera = true;
					Application.Quit();
				}
				else if(activeList[count].GetComponent<TextMesh>().text == "Back")
				{
					//renderer.enabled = false;
					activeList[count].renderer.material.color = Color.white;
					goUp = true;
					GameObject[] temp = activeList;
					activeList = prevLevel;
					foreach(GameObject go in temp)
					{
						go.transform.parent = parent;
						go.SetActive(false);
					}
					prevLevel = null;
					destination = new Vector3(0, activeList[0].transform.localPosition.y,transform.localPosition.z);
					isSongList = false;
					count = 0;
				}
				/*
				else if (activeList[count].GetComponent<TextMesh>().text == "Add Song")
				{
					string filePath = UnityEditor.EditorUtility.OpenFilePanel("Select Song", "", "mp3");
					if(filePath != null)
					{
						string songName = filePath.Substring(filePath.LastIndexOf('/')+1);
						Debug.Log(songName);
						
						FileUtil.CopyFileOrDirectory (filePath, "Assets/Resources/"+songName);
//						AudioSource a = AudioClip.Create();
//						a.clip = www.GetAudioClip(true);
//						AssetDatabase.CreateAsset(a,"Assets/Prefabs/Songs/Song_10.prefab");
					}
				}
				*/
				else
				{
					PlayerPrefs.SetInt("SongIndex",count);
					//Application.LoadLevel("MasterScene");
					cam.GetComponent<CameraScript>().setSong(au_clips[count]);
					cam.GetComponent<CameraScript>().setToScenePos();
					cam.GetComponent<CameraScript>().animateCamera = true;
				}
			}
		}
	}

	public void fadeOut()
	{
		isFade = true;
		fadeAlpha = Mathf.Lerp(fadeAlpha, 0,Time.deltaTime * fadeSpeed);
		textColor.a = fadeAlpha;
		foreach(GameObject g in activeList)
		{
			g.renderer.material.SetColor("_Color",textColor); 
		}
		if(fadeAlpha <= 0.01f)
		{
			isFade = false;
			fadeAlpha = 1;
			textColor.a = 1;
			gameObject.transform.parent.gameObject.SetActive(false);
		}
	}
	public void reReadHighScores()
	{
		if(!isSongList)
			return;
		int i = 0;
		foreach(GameObject go in activeList)
		{
			if(go.transform.childCount>0)
			{
				float highScore = PlayerPrefs.GetFloat ("HighScore_" + i.ToString());
				go.transform.GetChild(0).GetComponent<TextMesh>().text = "Score To Beat: "+ highScore.ToString();
			}
			i++;
		}

	}
}
