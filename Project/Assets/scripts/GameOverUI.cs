
using UnityEngine;
using System.Collections;

public class GameOverUI : MonoBehaviour
{
	public GUIText title;
	public GUIText scoreLabel;
	public GUIText highScoreLabel;

//	public KeyCode upKey;
//	public KeyCode downKey;
//	public KeyCode selectKey;
	public string upKey = "Vertical";
	public string downKey = "Vertical";
	public string selectKey = "Jump";

	public GUIText retryBtn;
	public GUIText menuBtn;
	public GUIText resumeBtn;

	// y-coordinate of the whole 'game over' stuffs after it comes in
	public float targetPos = 0.7f;
	public float flyInSpeed = 1f;
	public float scrubTime = 0.3f;
	public float showBtnDelay = 0.5f;	// time before showing the 'retry' 
										// and 'menu' button

/*Add audio source to camera and use this*/
	public GameObject cam; 

	// audio souce that will be 'scrubbed' when game over
	public AudioSource toDistort;
	public Progress progress;

	private float scrubTicks = 0.0f;

	private bool flyingIn = false;
	private bool flyingOut = false;
	private bool scrubbing = false;
	private bool delayingBtn = false;
	private bool selectMode = false;
	private bool songDong = false;

	private bool arrowKeyDown = false;

	// which menu item is being selected
	// select 'retry' by default
	private int selectIdx = 0;
	private int optionCount = 2;

	private AudioClip clipToDistort;

	private Vector3 initPos;

	void Awake()
	{
		initPos = new Vector3(0.5f, -0.5f, 0f);
		// initially out of screen
		transform.position = initPos;

		if (title == null) {
			title = gameObject.GetComponent<GUIText>();
		}

		/*
		if (scoreLabel == null) {
			scoreLabel = GameObject.Find("ScoreValue").GetComponent<GUIText>();
		}
		*/
		if (highScoreLabel == null) {
			highScoreLabel = 
				GameObject.Find("HighScoreValue").GetComponent<GUIText>();
		}

		if (retryBtn == null) {
			retryBtn = GameObject.Find("Retry").GetComponent<GUIText>();
		}
		if (menuBtn == null) {
			menuBtn = GameObject.Find("Menu").GetComponent<GUIText>();
		}
		if (resumeBtn == null) {
			resumeBtn = GameObject.Find("Resume").GetComponent<GUIText>();
		}

		// hide the buttons
		retryBtn.gameObject.SetActive(false);
		menuBtn.gameObject.SetActive(false);
		resumeBtn.gameObject.SetActive(false);
	}

	void Update()
	{
		if (flyingIn) {
			if (scrubbing) {
				// scrub the sound
				toDistort.time = Random.Range(0, clipToDistort.length);

				scrubTicks += Time.deltaTime;
				if (scrubTicks > scrubTime) {
					scrubbing = false;
				
					// stop the music
					toDistort.Stop();
					toDistort.time = 0;

					// reuse scrubTicks for button delay
					scrubTicks = 0f;
				}
			}

			// fly in 'game over' display
			transform.Translate(Vector3.up * flyInSpeed * Time.deltaTime);
			if (transform.position.y > targetPos) {
				transform.position = new Vector3(0.5f, targetPos, 0f);

				delayingBtn = true;
				flyingIn = false;
			}
		} else if (flyingOut) {
			transform.Translate(Vector3.down * flyInSpeed * Time.deltaTime);
			if (transform.position.y < initPos.y) {
				transform.position = initPos;

				flyingOut = false;
			}
		} else if (delayingBtn) {
			// reuse scrubTicks here
			scrubTicks += Time.deltaTime;
			if (scrubTicks > showBtnDelay) {
				delayingBtn = false;
				selectMode = true;

				// display the buttons
				retryBtn.gameObject.SetActive(true);
				menuBtn.gameObject.SetActive(true);
			}
		} else if (selectMode) {
			// some bad codes here...
			//if (Input.GetKeyDown(upKey)) {
			if (Input.GetAxis(upKey) != 0f) {
				if (!arrowKeyDown && Input.GetAxis(upKey) > 0f) {
					arrowKeyDown = true;	// prevent repeat triggering

					selectIdx = (selectIdx - 1 + optionCount) % optionCount;
				} else if (!arrowKeyDown && Input.GetAxis(downKey) < 0f) {
					arrowKeyDown = true;

					selectIdx = (selectIdx + 1) % optionCount;
				}
			} else {
				arrowKeyDown = false;
			}

			if (Input.GetButtonDown(selectKey)) {
				if (selectIdx - optionCount + 2 == 0) {
					if (optionCount == 3) {
						// needs to resume everything before reloading the scene
						resume();
					}

					GameObject.Find("Player").rigidbody2D.isKinematic = true;
					
//					Application.LoadLevel(Application.loadedLevel);
					GameObject.FindGameObjectWithTag("GameController").GetComponent<Base>().changeToScene();
				} else if (selectIdx - optionCount + 2 == 1){
					//Application.LoadLevel(0);
					if (optionCount == 3) {
						resume ();
					}

					GameObject.Find("Player").rigidbody2D.isKinematic = true;

					// fly away
					flyOut();

					// hide level text
					GameObject levelObj = GameObject.Find("LevelGroup");
					if (levelObj != null) {
						levelObj.SetActive(false);
					}

					cam.GetComponent<CameraScript>().animateCamera = true;
					cam.GetComponent<CameraScript>().setToInitPos();				
				} else {
					resume();
				}
			}

			if (selectIdx - optionCount + 2 == 0) {
				// select 'retry'
				if (optionCount == 3)
					resumeBtn.color = Color.white;
				retryBtn.color = Color.green;
				menuBtn.color = Color.white;
			} else if (selectIdx - optionCount + 2 == 1) {
				// select 'menu'
				if (optionCount == 3)
					resumeBtn.color = Color.white;
				retryBtn.color = Color.white;
				menuBtn.color = Color.green;
			} else {
				// select 'resume'
				resumeBtn.color = Color.green;
				retryBtn.color = Color.white;
				menuBtn.color = Color.white;
			}
		}
	}

	// to update the highscore value......XD
	private void updateHighscore()
	{
		// read song index
		int songIdx = PlayerPrefs.GetInt("SongIndex");

		// use 0 for score as we dont use it anymore
		setScores(0f,
				PlayerPrefs.GetFloat("HighScore_" + songIdx.ToString()));
	}

	// make the game over screen fly in
	public void flyIn(bool levelCleared)
	{
		flyingIn = true;
		optionCount = 2;		// hide 'resume'
		selectIdx = 0;			// always points to the first option when shown
		
		if (levelCleared) {
			scrubbing = false;
			title.text = "SONG COMPLETED!";
		} else {
			scrubbing = true;
			scrubTicks = 0f;
			title.text = "GAME OVER";
		}

		// disable the tracking of the progress bar
		if (progress != null)
			progress.tracking = false;
	}

	public void flyOut()
	{
		flyingOut = true;
	}

	// may not be appropriate to put pause() and resume() here
	public void pause()
	{
		// display 'paused' screen
		transform.position = new Vector3(0.5f, targetPos, 0f);
		guiText.text = "GAME PAUSED";
		
		optionCount = 3;		// allow 'resume' to be selected
		selectIdx = 0;			// always points to the first option when shown
		selectMode = true;
		
		// show all three buttons
		resumeBtn.gameObject.SetActive(true);
		retryBtn.gameObject.SetActive(true);
		menuBtn.gameObject.SetActive(true);
		
		// pause everything else
		//		toDistort.Pause();
		AudioListener.pause = true;

		// get the latest highscore
		updateHighscore();
		
		FreqHold freqHold = GameObject.Find("FreqHold").GetComponent<FreqHold>();
		freqHold.deactivate();
	}
	
	public void resume()
	{
		// hide 'paused' screen
		transform.position = new Vector3(0.5f, -0.5f, 0f);
		
		selectMode = false;
		
		// hide all three buttons
		resumeBtn.gameObject.SetActive(false);
		retryBtn.gameObject.SetActive(false);
		menuBtn.gameObject.SetActive(false);
		
		// resume everything else
		//toDistort.Play();
		AudioListener.pause = false;

		FreqHold freqHold = GameObject.Find("FreqHold").GetComponent<FreqHold>();
		freqHold.activate();
		
		PlayerScript playerScript = GameObject.Find("Player").GetComponent<PlayerScript>();
		playerScript.SendMessage("Resume");
	}
	
	// display the score and the high score
	public void setScores(float score, float highScore)
	{
		//scoreLabel.text = score.ToString();

		int fraction = (int)((highScore - Mathf.Floor(highScore)) * 100);
		if (fraction < 10) {
			highScoreLabel.text = Mathf.Floor(highScore).ToString() + ".0" + fraction.ToString();
		} else {
			highScoreLabel.text = Mathf.Floor(highScore).ToString() + "." + fraction.ToString();
		}
	}

	public void setToDistort(AudioSource song)
	{
		toDistort = song;
		clipToDistort = toDistort.clip;
	}

	public void resetToInit()
	{
		transform.position = initPos;
		selectMode = false;
		flyingOut = false;
	}
}
