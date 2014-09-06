
using UnityEngine;
using System.Collections;

public class Progress : MonoBehaviour
{
	public AudioSource src;

	public bool tracking = true;	// set this to false to disable progress

	// for controlling the small ring
	public GameObject smallRing;

	public GameOverUI gameOverUI;
	public LevelText levelText;
	
	public float baseInterval = 1f;
	public float moveTime = 0.5f;
	public float startEmissive =10f;
	public float targetEmissive = 0.5f;
	public float targetPercent = 1.2f;
	public float outerStartPercent = 0.8f;

	private float length = 0.0f;
	public float p;				// percent of song

	private float ticks;
	private float range;

	private bool showingSmallRing = false;
	private float startPercent;
	private float interval;

	private static Progress singletonObj;

	void Awake()
	{
		//length = src.clip.length;
		resetToInit();
	}
	public static Progress singleton
	{
		get{
			if(singletonObj==null)
			{
				singletonObj = GameObject.FindObjectOfType<Progress>();
			}
		return singletonObj;
		}
	}
	public void resetToInit()
	{
		tracking = true;
		ticks = 0f;
		interval = baseInterval;
		range = Mathf.Abs (outerStartPercent - targetPercent);
		
		if (gameOverUI == null) {
			gameOverUI = GameObject.Find("GameOverTitle").GetComponent<GameOverUI>();
		}
		
		if (smallRing == null) {
			smallRing = GameObject.Find("ProgressSmallRing");
		}

		GameObject shadowRing = GameObject.Find ("ProgressShadow");
		shadowRing.renderer.material.SetFloat("_Percent", outerStartPercent);

		smallRing.SetActive(false);
	}
	void Update()
	{
		if (tracking) {
			p = src.time / length;
			levelText.setPercent((int)(p * 100f));
			if (p >= 1f) {
				// song completed
				PlayerScript playerScript = GameObject.Find("Player").GetComponent<PlayerScript>();
				int idx = GameObject.Find ("Control").GetComponent<ConfigLoader>().songIdx;
				
				float highScore = PlayerPrefs.GetFloat("HighScore_" + idx.ToString());
				if ((playerScript.score + 1f) > highScore) {
					highScore = playerScript.score + 1f;
					PlayerPrefs.SetFloat("HighScore_" + idx.ToString(), highScore);
				}
				playerScript.gameWin = true;
				//playerScript.gameObject.SetActive(false);
				playerScript.renderer.enabled = false;
				playerScript.rigidbody2D.isKinematic = true;
				
				gameOverUI.setScores(playerScript.score + 1f, highScore);
				gameOverUI.flyIn(true);
				//gameOverUI.flyIn(true);
			}

			renderer.material.SetFloat("_Percent", p * range + outerStartPercent);

			ticks += Time.deltaTime;
			if (!showingSmallRing) {
				if (ticks > interval) {
					ticks -= interval;

					// show the ring at starting state
					smallRing.SetActive(true);
					smallRing.renderer.material.SetFloat("_Percent", p * range + outerStartPercent);
					smallRing.renderer.material.SetFloat("_Emissive", startEmissive);

					startPercent = p * range + outerStartPercent;

					// set showing flag
					showingSmallRing = true;
				}
			} else {
				// update small ring state
				smallRing.renderer.material.SetFloat("_Percent", 
						Mathf.Lerp(startPercent, targetPercent, ticks / moveTime));
				smallRing.renderer.material.SetFloat("_Emissive",
						Mathf.Lerp(startEmissive, targetEmissive, ticks / moveTime));

				// check stopping condition
				if (ticks > moveTime) {
					ticks -= moveTime;

					// hide the small ring
					smallRing.SetActive(false);

					// reset showing flag
					showingSmallRing = false;
				}
			}
		}
	}

	public void setStrength(float strength)
	{
		renderer.material.SetFloat("_Emissive", strength);
		interval = baseInterval * 10f / strength;
	}

	public void setLength(float len)
	{
		length = len;
	}
}
