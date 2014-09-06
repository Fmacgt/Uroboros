
using UnityEngine;
using System.Collections;

public class Platform : MonoBehaviour
{
	// list of segments to pick
	public GameObject[] segments;
	public float[] difficulties;
	
	// number of segments for this platform
	// use 6 for now
	public int segmentNum = 6;
	
	// index of the segment left out at the beginning
	public int leftOutIdx = 0;
	
	public float angSpeed = 0.1f;
	public float baseAngSpeed = 0f;
	public float moveSpeed = 0.1f;
	
	public float threshold = 0.05f;
	public float ratio = 10.0f;
	public float damping = 0.07f;
	
	public Spawner spawner;
	public FreqHold freqHold;
	
	public GameObject player;

	private int radiusIndex = 0;	// index to the platform for radius
	
	public float initRadius = 1.8f;
	private float direction = 1.0f;
	private float dist = 1.0f;
	private float gapRatio = 1.25f;
	private float nonGapRatio = 1.12f;
	
	private float time = 0.0f;
	private float fadeAlpha = 0.0f;
	private float fadeSpeed = 0.8f;
	
	private bool notClosed;
	private bool outer=false;
	private bool inner=true;
	private bool active=false;
	private bool expandAndDestroy;
	private bool checkRadius = true;
	private bool fadingIn = false;
	private bool fadingOut = false;

	private float speedSum = 0.0f;
	private int speedCount = 0;

	private float scale = 0.5f;
	private float scaleStep = 0.001f;
	private float speedRatio = 0.8f;
	
	void Awake()
	{
		// obtain reference to the spawner
		spawner = GameObject.Find("Control").GetComponent<Spawner>();
		freqHold = GameObject.Find("FreqHold").GetComponent<FreqHold>();
		player = GameObject.Find ("Player");

		notClosed = true;
		expandAndDestroy = false;

		if (spawner.isEmpty()) {
			// this is the first platform, generate itself
			generate(6, 12, 0);

			// turn off fade in for the first platform
			fadingIn = false;
			foreach (Transform seg in transform) {
				seg.gameObject.renderer.material.SetColor("_Color", 
						new Color(1f, 1f, 1f, 1f));
				seg.gameObject.collider2D.enabled = true;
			}
		}
	}
	
	void Update()
	{
		if (fadingIn) {
			fadeIn();
		} else if (fadingOut) {
			fadeOut();
		}

		_updateSpeed();
		
		time += Time.deltaTime;
		
		/*if(outer)
			if(time>10.0f)
				if(spawner!=null){
					spawner.spawn ();
					GameObject.Destroy (gameObject);
					time=0;
				}
		/*if (time > 10.0f) {
			if (spawner != null) {
				spawner.spawn();

				GameObject.Destroy(gameObject);
			}
		}*/
	}
	
	void FixedUpdate()
	{
		// move each segment apart
		float radius=Vector3.Distance(transform.GetChild(radiusIndex).position,transform.position);
		float playerDistance = Vector3.Distance (player.transform.position, transform.position);

		if (checkRadius && !fadingIn && 
				playerDistance < radius + 0.15f)
		{
			checkRadius = false;

			changeRing ();
			Transform t;
			if(notClosed)
			{
				if(spawner.getInner() == null)
					t= transform;
				else
					t = spawner.getInner().transform;
				foreach(Transform child in t)
				{
					child.gameObject.SetActive(true);
				}
				notClosed = false;
				if(t != transform)
				{
					expandAndDestroy = true;
				}
			}
		}
		//Debug.Log (radius);
		if (!checkRadius && scale < 0.75f) {
			scale += scaleStep;
		}
		
		//if (radius < 3.8f && outer || expandAndDestroy) {
		if (radius < 4.5f && outer || expandAndDestroy) {
			/*
			bool closingGaps = false;
			foreach (Transform seg in transform) {
				if (Vector3.Distance(seg.position, transform.position) > radius) {
					closingGaps = true;
					break;
				}
			}
			*/

			for (int i = 0; i < transform.childCount; i++) {
				Transform seg = transform.GetChild (i);
				
				Vector3 dir = seg.position - transform.position;
				
				if (Vector3.Distance(seg.position, transform.position) > radius) {
					// gap platforms
					seg.Translate (dir.normalized * _getMoveSpeed () * -gapRatio * Time.deltaTime, Space.World);

					// clap the radius in case it overshots
					float d = Vector3.Distance(seg.position, transform.position);
					if (d < radius) {
						seg.Translate(dir.normalized * (radius - d), Space.World);
					}
				//} else if (!closingGaps) {
				} else {
					// non-gap platforms
					seg.Translate (dir.normalized * _getMoveSpeed () * Time.deltaTime, Space.World);
				}

				seg.localScale = new Vector3(scale, scale, scale);
			}
		} else if (!fadingOut && radius >= 3.5f && spawner.getInner()==null) {
			// NOTE: needs the '!fadingOut' here to prevent additional platform
			// from beging created

			// compute difficulty for the next platform
			float low = 0f;
			if (speedCount > 0)
				low = speedSum / speedCount;

			// reset accumulators
			speedSum = 0.0f;
			speedCount = 0;

			// create new platform
			spawner.spawn(low);
		}
		
		
		// rotate
		transform.RotateAround(Vector3.back, _getAngSpeed() * Time.deltaTime);
	}
	
	public void changeRing(){
		outer = true;
		inner = false;

		/*
		speedSum = 0.0f;
		speedCount = 0;
		*/

		spawner.changeRing ();
	}

	public void die()
	{
		// begin fading out
		fadingOut = true;
		fadeAlpha = 1.0f;

		/*
		for (int i = 0; i < transform.childCount; i++) {
			Transform seg = transform.GetChild (i);
			
			Vector3 dir = seg.position - transform.position;
			
			seg.Translate (dir.normalized * direction * _getMoveSpeed () * Time.deltaTime, Space.World);
				seg.Translate(Vector3.up * Time.deltaTime, Space.Self);
		}
	*/
	//	GameObject.Destroy(gameObject);
	}

	public float getRadius(){
		return Vector3.Distance (transform.GetChild (radiusIndex).position, transform.position);
	}

	// generate a platform whose 'difficulty' is in [low, high)
	public void generate(float low, float high, int levelCount)
	{
		int[] list = new int[segmentNum];
	
		// to generate random sequence of all possible segments
		int[] segList = new int[segments.Length];
		for (int i = 0; i < segments.Length; i++) {	
			segList[i] = i;	// initialize it
		}

		int trial = 0;
		//int leveltype=Mathf.FloorToInt(Random.Range(1,5));
		bool use2Gaps = false;

		// increment ratio of difficulty if use two gaps
		float twoGapRatio = 1.5f;

		// after how many ring does 2-gap rings has a chance to appear
		int twoGapLevel = 15;

		float sum = 0.0f;
		// max 30 trials
		while (trial < 300) {
			sum = 0.0f;

			// pick a random permutation
			for (int i = 0; i < 10; i++) {
				int a = Mathf.FloorToInt(Random.Range(0, segments.Length));
				int b = Mathf.FloorToInt(Random.Range(0, segments.Length));

				int tmp = segList[a];
			   	segList[a] = segList[b];
				segList[b] = tmp;
			}
			
			for (int i = 0; i < segmentNum/2; i++) {
				/*
				int rand = Mathf.FloorToInt(Random.value*segments.Length);
				list[i] = rand;
				list[i + 3] = rand;
				*/
				list[i] = segList[i];
				list[i + 3] = segList[i];

				sum += 2 * difficulties[list[i]];
			}

			if (sum >= low && sum < high) {
				// try make it 2-gap
				if (levelCount > twoGapLevel && 
						sum * twoGapRatio < high) {
					// can make it 2-gap
					// NOTE: since sum >= low, sum * twoGapRatio must > low
					use2Gaps = true;
				}

				// satisfies the constraint, break
				break;
			}

			trial++;
		}
		//Debug.Log("diff (" + low + ", " + high + "); actual: " + sum);

		// pick the one with lowest difficulty to be removed
		/*
		float min = difficulties[list[0]];
		int hide = 0;
		for (int i = 1; i < segmentNum; i++) {
			if (difficulties[list[i]] < min) {
				min = difficulties[list[i]];
				hide = i;
			}
		}
		*/
		float angPerSegment = Mathf.PI * 2.0f / segmentNum;

		//Debug.Log (leveltype);
		//if (leveltype != 3) {		
		if (!use2Gaps) {		
			for (int i = 0; i < segmentNum/3; i++) {
				GameObject seg = (GameObject)Instantiate (
					segments [list [i]], new Vector3 (0f, 0f, 0f), Quaternion.identity);

				GameObject seg2 = (GameObject)Instantiate (
					segments [list [i]], new Vector3 (0f, 0f, 0f), Quaternion.identity);
				GameObject seg3 = (GameObject)Instantiate (
					segments [list [i]], new Vector3 (0f, 0f, 0f), Quaternion.identity);

				// attach the new part to the platform
				seg.transform.parent = transform;

				if (i == 0) {
					// extrude platforms to hide a little bit
					seg.transform.position = new Vector3 (initRadius * gapRatio * Mathf.Cos (angPerSegment * i),
							initRadius * gapRatio * Mathf.Sin (angPerSegment * i), 0);
					seg.transform.RotateAround (Vector3.back, Mathf.PI * 2f / 3f - angPerSegment * i);

					seg2.transform.parent = transform;
					seg2.transform.position = new Vector3 (initRadius * gapRatio * Mathf.Cos (angPerSegment * (i + 2)),
							initRadius * gapRatio * Mathf.Sin (angPerSegment * (i + 2)), 0);
					seg2.transform.RotateAround (Vector3.back, Mathf.PI * 2f / 3f - angPerSegment * (i + 2));


					seg3.transform.parent = transform;
					seg3.transform.position = new Vector3 (initRadius * gapRatio * Mathf.Cos (angPerSegment * (i + 4)),
							initRadius * gapRatio * Mathf.Sin (angPerSegment * (i + 4)), 0);
					seg3.transform.RotateAround (Vector3.back, Mathf.PI * 2f / 3f - angPerSegment * (i + 4));
				} else {
					seg.transform.position = new Vector3 (initRadius * Mathf.Cos (angPerSegment * i),
							initRadius * Mathf.Sin (angPerSegment * i), 0);
					seg.transform.RotateAround (Vector3.back, Mathf.PI * 2f / 3f - angPerSegment * i);

					seg2.transform.parent = transform;
					seg2.transform.position = new Vector3 (initRadius * Mathf.Cos (angPerSegment * (i + 2)),
							initRadius * Mathf.Sin (angPerSegment * (i + 2)), 0);
					seg2.transform.RotateAround (Vector3.back, Mathf.PI * 2f / 3f - angPerSegment * (i + 2));


					seg3.transform.parent = transform;
					seg3.transform.position = new Vector3 (initRadius * Mathf.Cos (angPerSegment * (i + 4)),
							initRadius * Mathf.Sin (angPerSegment * (i + 4)), 0);
					seg3.transform.RotateAround (Vector3.back, Mathf.PI * 2f / 3f - angPerSegment * (i + 4));
				}


				if (i == 0) {
					//if (i == hide) {
					seg.gameObject.SetActive (false);
					seg2.gameObject.SetActive (false);
					seg3.gameObject.SetActive (false);
				}
				seg.renderer.material.SetColor ("_Color", new Color (1f, 1f, 1f, 0f));
				seg.collider2D.enabled = false;
				fadeAlpha = 0.0f;
				
				seg2.renderer.material.SetColor ("_Color", new Color (1f, 1f, 1f, 0f));
				seg2.collider2D.enabled = false;
				
				seg3.renderer.material.SetColor ("_Color", new Color (1f, 1f, 1f, 0f));
				seg3.collider2D.enabled = false;
			}
		}else{
			int type=Mathf.FloorToInt(Random.Range (1,2));
			for (int i = 0; i < segmentNum/2; i++) {
				// 1st segment of the i-th type
				GameObject seg = (GameObject)Instantiate (
					segments [list [i]], new Vector3 (0f, 0f, 0f), Quaternion.identity);
				
				// 2nd segment of the i-th type
				GameObject seg2 = (GameObject)Instantiate (
					segments [list [i]], new Vector3 (0f, 0f, 0f), Quaternion.identity);
				
				// attach the new part to the platform
				seg.transform.parent = transform;
				seg2.transform.parent = transform;
				
				if (i == 1) {
					// hidden segments
					seg.transform.position = new Vector3 (initRadius * gapRatio * Mathf.Cos (angPerSegment * i),
							initRadius * gapRatio * Mathf.Sin (angPerSegment * i), 0);
					seg.transform.RotateAround (Vector3.back, Mathf.PI * 2f / 3f - angPerSegment * i);

					if(type==1){
						seg2.transform.position = new Vector3 (initRadius * gapRatio * Mathf.Cos (angPerSegment * (i + (segmentNum/2))),
								initRadius * gapRatio * Mathf.Sin (angPerSegment * (i + (segmentNum/2))), 0);
						seg2.transform.RotateAround (Vector3.back, Mathf.PI * 2f / 3f - angPerSegment * (i + (segmentNum/2)));
					}else if(type==2){
						seg2.transform.position = new Vector3 (initRadius * gapRatio * Mathf.Cos (angPerSegment * (segmentNum - i - 1)),
								initRadius * gapRatio * Mathf.Sin (angPerSegment * (segmentNum - i - 1)), 0);
						seg2.transform.RotateAround (Vector3.back, Mathf.PI * 2f / 3f - angPerSegment * (segmentNum - i - 1));
					}
				} else {
					// regular segments
					seg.transform.position = new Vector3 (initRadius * nonGapRatio * Mathf.Cos (angPerSegment * i),
							initRadius * nonGapRatio * Mathf.Sin (angPerSegment * i), 0);
					seg.transform.RotateAround (Vector3.back, Mathf.PI * 2f / 3f - angPerSegment * i);

					if(type==1){
						seg2.transform.position = new Vector3 (initRadius * nonGapRatio * Mathf.Cos (angPerSegment * (i + (segmentNum/2))),
								initRadius * nonGapRatio * Mathf.Sin (angPerSegment * (i + (segmentNum/2))), 0);
						seg2.transform.RotateAround (Vector3.back, Mathf.PI * 2f / 3f - angPerSegment * (i + (segmentNum/2)));
					}else if(type==2){
						seg2.transform.position = new Vector3 (initRadius * nonGapRatio * Mathf.Cos (angPerSegment * (segmentNum - i - 1)),
								initRadius * nonGapRatio * Mathf.Sin (angPerSegment * (segmentNum - i - 1)), 0);
						seg2.transform.RotateAround (Vector3.back, Mathf.PI * 2f / 3f - angPerSegment * (segmentNum - i - 1));
					}
					/*
					seg.transform.position = new Vector3 (initRadius * Mathf.Cos (angPerSegment * i),
							initRadius * Mathf.Sin (angPerSegment * i), 0);
					seg.transform.RotateAround (Vector3.back, Mathf.PI * 2f / 3f - angPerSegment * i);

					if(type==1){
						seg2.transform.position = new Vector3 (initRadius * Mathf.Cos (angPerSegment * (i + (segmentNum/2))),
								initRadius * Mathf.Sin (angPerSegment * (i + (segmentNum/2))), 0);
						seg2.transform.RotateAround (Vector3.back, Mathf.PI * 2f / 3f - angPerSegment * (i + (segmentNum/2)));
					}else if(type==2){
						seg2.transform.position = new Vector3 (initRadius * Mathf.Cos (angPerSegment * (segmentNum - i - 1)),
								initRadius * Mathf.Sin (angPerSegment * (segmentNum - i - 1)), 0);
						seg2.transform.RotateAround (Vector3.back, Mathf.PI * 2f / 3f - angPerSegment * (segmentNum - i - 1));
					}
					*/
				}
				
				if (i == 1) {
					//if (i == hide) {
					seg.gameObject.SetActive (false);
					seg2.gameObject.SetActive (false);
				}
				seg.renderer.material.SetColor ("_Color", new Color (1f, 1f, 1f, 0f));
				seg.collider2D.enabled = false;
				fadeAlpha = 0.0f;
				
				seg2.renderer.material.SetColor ("_Color", new Color (1f, 1f, 1f, 0f));
				seg2.collider2D.enabled = false;
			}
		}
		
		for (int i = 0; i < segmentNum; i++) {
			if (transform.GetChild(i).gameObject.activeSelf) {
				radiusIndex = i;
				break;
			}
		}

		// begin fade in
		fadingIn = true;
	}

	void fadeIn()
	{
		fadeAlpha += fadeSpeed * Time.deltaTime;

		if (fadeAlpha > 1.0f) {
			fadeAlpha = 1.0f;

			fadingIn = false;
		}

		foreach (Transform seg in transform) {
			seg.gameObject.renderer.material.SetColor("_Color",
					new Color(1f, 1f, 1f, Mathf.Pow(fadeAlpha, 4.0f)));

			//if (!fadingIn) {
			if (Mathf.Pow(fadeAlpha, 4.0f) > 0.3f) {
				seg.gameObject.collider2D.enabled = true;
			}
		}
	}

	public void fadeOut()
	{
		fadeAlpha -= fadeSpeed * Time.deltaTime;
		if (fadeAlpha <= 0.0f) {
			// destroy the platform after it fades out
			GameObject.Destroy(gameObject);
		}

		foreach (Transform seg in transform) {
			seg.gameObject.renderer.material.SetColor("_Color",
					new Color(1f, 1f, 1f, fadeAlpha));
		}
	}
	
	public float _getAngSpeed()
	{
		return angSpeed;
	}
	
	float _getMoveSpeed()
	{
		return moveSpeed;
	}

	void _updateSpeed()
	{
		float accel = (freqHold.getAmp() > threshold) ? 0.4f * ratio : 0.0f;
		/*
		float amp = freqHold.getAmp();
		
		float accel=0f;
		if (amp > threshold * 2) {
			accel = 0.6f;
		} else if (amp > threshold) {
			accel = 0.3f;
		}
		*/
		//float accel = (amp > threshold) ? 0.5f * ratio : 0.0f;

		angSpeed += accel * Time.deltaTime;
		angSpeed -= angSpeed * damping;

		if (angSpeed < baseAngSpeed)
			angSpeed = baseAngSpeed;

		speedSum += angSpeed;
		speedCount++;

		if (inner) {
			angSpeed *= speedRatio;
		}
		//Debug.Log(angSpeed);
	}
}
