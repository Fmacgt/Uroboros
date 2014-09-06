using UnityEngine;
using System.Collections;

public class CreateLevel : MonoBehaviour {

	public GameObject[] level_segments;
	public float init_radius;
	private float init_angle;
	public float speed;
	private bool playerEntered, notClosed, callFurther;
	private float angle;
	private float start_time;

	// Use this for initialization
	void Start () {
//		playerEntered = false;
//		notClosed = true;
//		callFurther = true;
//		angle = 0;
		Vector3 position = new Vector3(init_radius, init_radius, 0);
//		init_angle = 0;
//		int hide = (int)Random.Range(0,6);
		for(int i=0; i<6; i++)
		{
			float xPos = init_radius*Mathf.Cos(Mathf.Deg2Rad*init_angle);
			float yPos = init_radius*Mathf.Sin(Mathf.Deg2Rad*init_angle);
			int index = (int)Random.Range(0,level_segments.Length);
			position.Set(xPos,yPos,0);
			GameObject child = (GameObject)Instantiate(level_segments[index],position, Quaternion.identity); 
			child.transform.parent = gameObject.transform;
//			if(i == hide)
//			{
//				child.SetActive(false);
//			}
			child.transform.Rotate(0,0,(240+init_angle));
			init_angle += 60;
		}
		//start_time = Time.time;
		//makeTransparent(3,start_time,0);
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(Vector3.back, speed*Time.deltaTime);
//		angle += speed*Time.deltaTime;
//		StartCoroutine(makePlayerEnter(3.0f));
//		if(callFurther){
//			if(makeTransparent(3,start_time,0)) callFurther = false;
//		}
//		if(playerEntered && notClosed)
//		{
//			foreach(Transform child in transform)
//			{
//				child.gameObject.SetActive(true);
//			}
//			notClosed = false;
//		}
	}

	IEnumerator makePlayerEnter(float time)
	{
		yield return new WaitForSeconds(time);
		playerEntered = true;
	}

	/* set fade to 1 to fade out or 0 to fade in
	 * t is the time to fade,
	 * start_time is the time at which the function is called for the first time
	 */
	bool makeTransparent(float t, float start_time, int fade)
	{
		foreach(Transform child in transform)
		{
			child.collider2D.enabled = false;
			SpriteRenderer sr = child.gameObject.GetComponent<SpriteRenderer>();
			sr.material.SetColor("_Color",new Color(0,0,0,Mathf.Abs(fade-((Time.time - start_time)/t))));
		}
		if(t <= Time.time - start_time)
		{
			if(fade == 1)
				return true;
			foreach(Transform child in transform)
			{
				child.collider2D.enabled = true;
				SpriteRenderer sr = child.gameObject.GetComponent<SpriteRenderer>();
				sr.material.SetColor("_Color",new Color(0,0,0,1));
			}
			return true;
		}
		return false;
	}

}
