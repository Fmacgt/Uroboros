using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {

	//public KeyCode jump;
	//public KeyCode pauseKey;
	public string jump = "Jump";
	public string pauseKey = "Fire1";
	
	public float jumpSpeed = 20f;

	public GameObject orbObject, shadow;
	public int score = 0;

	public GameOverUI gameOverScreen;

	private bool gameOver = false;
	public bool gameWin = false;
	public bool gamePaused = false;

	public bool isLerping;
	private Vector3 destination;

	/*
	public float maxEN = 100f;
	public float energy = 100f;
	public float enUsage = 10.0f;
	*/

	private int noOfJumps;
	private int jumpsLeft;	
	private float oldTimeScale;
	private bool hasJumped;

	void Start()
	{
		isLerping = false;
		noOfJumps = 3;
		jumpsLeft = noOfJumps;
		createOrbs();
		hasJumped = false;
	}

	//public GUIStyle enFont;
	void OnCollisionEnter2D(Collision2D other){
		jumpsLeft = noOfJumps;
		foreach(Transform child in GetComponentInChildren<Transform>())
		{
			child.gameObject.SetActive(true);
		}
		if(hasJumped)
		{
			jumpsLeft--;
			foreach(Transform child in transform)
			{
				child.gameObject.SetActive(false);
				break;
			}
		}
	}
	
	// for resuming the game without pressing ESC
	void Resume()
	{
		gamePaused = false;
		
		Time.timeScale = oldTimeScale;
	}
	
	// Update is called once per frame
	void Update () {
//		Debug.Log (gameOver);
		if (gameOver || gameWin)
			return;
		
		//if (Input.GetKeyDown(pauseKey)) {
		if (Input.GetButtonDown(pauseKey)) {
			if (!gamePaused) {
				oldTimeScale = Time.timeScale;
				
				Time.timeScale = 0f;
				
				gameOverScreen.pause();
				
				gamePaused = true;
			} else {
				//Time.timeScale = oldTimeScale;
				
				// it will call Resume() indirectly
				gameOverScreen.resume();
			}
		}
		
		if (gamePaused)
			return;
		
		if(isLerping)
			lerpTo(destination);

		if (gameOver || rigidbody2D.isKinematic)
				return;
//		if(rigidbody2D.IsAwake())
//		{
//			rigidbody2D.Sleep();
//		}

		/*
		if (Input.GetKey (jump)) {
			transform.rigidbody2D.AddForce (new Vector2 (50,jumpForce));
		}
		*/
				/*
		if (Input.GetKeyDown(jump)) {
			if (energy > enUsage * Time.deltaTime) {
				transform.rigidbody2D.AddForce(
						//new Vector2(50 * Time.deltaTime, jumpForce * Time.deltaTime));
						new Vector2(25, jumpForce));

				energy -= enUsage * Time.deltaTime;
				if (energy <= 0f)
					energy = 0f;
			}
		} else {
			energy += enUsage * 0.375f * Time.deltaTime;
			if (energy > maxEN) {
				energy = maxEN;
			}
		}

		float percent = energy/maxEN;
		percent = Mathf.Sqrt(percent);
		renderer.material.SetColor("_Color",
			   	new Color(0f, percent, percent));
		*/
				//if (Input.GetKeyDown (jump)) {
		hasJumped = false;
				if (Input.GetButtonDown (jump)) {
						if (jumpsLeft > 0) {
								hasJumped = true;
								Instantiate (shadow, transform.position, Quaternion.identity);
								//transform.rigidbody2D.velocity =  new Vector2(transform.rigidbody2D.velocity.x,(0.5f+((0.5f/noOfJumps)*jumpsLeft)*jumpSpeed));
								transform.rigidbody2D.velocity =  new Vector2(transform.rigidbody2D.velocity.x,jumpSpeed);
								jumpsLeft--;
								foreach(Transform child in transform)
								{
									if(child.gameObject.activeSelf)
									{
										child.gameObject.SetActive(false);
										break;
									}
								}
						}
				}


						if (transform.position.y < -10f || transform.position.y > 15f ||
								transform.position.x < -18f || transform.position.x > 18f) {
								//Application.LoadLevel (Application.loadedLevel);
			
			int songIdx = GameObject.Find ("Control").GetComponent<ConfigLoader>().songIdx;
			float highScore = PlayerPrefs.GetFloat ("HighScore_" + songIdx.ToString());
			LevelText levelText = GameObject.Find ("LevelGroup").GetComponent<LevelText>();
			
			if (score + levelText.percent/100f > highScore) {
				highScore = score + levelText.percent/100f;
				
				PlayerPrefs.SetFloat ("HighScore_" + songIdx.ToString(), 
				                      score + levelText.percent/100f);
			}
			
			gameOver = true;
			rigidbody2D.isKinematic = true;
			rigidbody2D.Sleep();
			
			gameOverScreen.setScores (score + levelText.percent/100f, highScore);
			gameOverScreen.flyIn (false);
			/*	
			int highScore = PlayerPrefs.GetInt ("HighScore");
			if (score > highScore) {
				highScore = score;
				PlayerPrefs.SetInt ("HighScore", score);
			}
			gameOver = true;
			//Deactivating rigidBody
			rigidbody2D.isKinematic = true;
			rigidbody2D.Sleep();
			gameOverScreen.setScores (score, highScore);
			gameOverScreen.flyIn (false);
			*/
						}
				}

	public void IncreaseJumps(int levels){
		/*
		if (levels % 5 == 0) {
						noOfJumps++;
			createOrbs ();
				}
		*/
		score = levels;

	}
	/*
	void OnGUI()
	{
		GUI.Label(new Rect(50, 0, 100, 40), "Jumps: " + jumpsLeft);
	}*/

	public void createOrbs()
	{
		foreach(Transform child in transform)
		{
			Destroy(child.gameObject);
		}
		float orbAngle = 2*Mathf.PI / noOfJumps;
		float orbRadius = 0.1f;
		for(int i =0; i< noOfJumps; i++)
		{
			GameObject orb = (GameObject)Instantiate(orbObject,
			                                         transform.position+new Vector3(orbRadius*Mathf.Cos(orbAngle*i),orbRadius*Mathf.Sin (orbAngle*i),0),
			                                         Quaternion.identity);
			orb.transform.parent = transform;
			orb.SetActive(true);
		}
	}

	public void lerpTo (Vector3 dest)
	{
		destination = dest;
		isLerping = true;
		transform.localPosition = Vector3.Lerp(transform.localPosition, destination, Time.deltaTime * 2);

		if((transform.position-dest).magnitude <= 0.1f)
		{
			isLerping = false;
			transform.position = dest;
			rigidbody2D.isKinematic = false;
			rigidbody2D.AddForce(Vector2.zero);
		}
	}

	public void resetGameOver()
	{
		gameOver = false;
		noOfJumps = 3;
		createOrbs();
	}
}
