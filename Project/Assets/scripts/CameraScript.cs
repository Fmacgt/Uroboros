using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {
	public bool animateCamera;

	//for jiggling
	public bool jiggle;
	private float jigposx;
	private bool deactivateCalled;

	public string scene;
	Camera c;
	private Vector3 newPos;
	private Vector3 initPos;
	private AudioClip clipToPlay;

	// Use this for initialization
	void Start () {
		c = GetComponent<Camera>();
		animateCamera = false;
		initPos = transform.position;
		setToScenePos();
		scene = null;
		jigposx = 2.5f;
		jiggle = false;
		deactivateCalled =false;
	}
	
	// Update is called once per frame
	void Update () {
		if(animateCamera)
		{
			transform.position = Vector3.Lerp(transform.position,newPos,1.5f*Time.deltaTime);
			if((transform.position - newPos).magnitude <=0.1f)
			{
				transform.position = newPos;
				animateCamera = false;
				if(transform.position.x == 0)
				{

					GameObject.FindGameObjectWithTag("GameController").GetComponent<Base>().changeToScene();
				}
				else
				{
					GetComponent<AudioSource>().clip = (AudioClip) Resources.Load("MenuMusic");
					GetComponent<AudioSource>().Play();
					GameObject.FindGameObjectWithTag("GameController").GetComponent<Base>().changeToMenu();
				}
//				c.orthographicSize = Mathf.Lerp(c.orthographicSize,2,1.5f*Time.deltaTime);
			}
//			if(c.orthographicSize < 2.1)
//			{
//				animateCamera = false;
//				if(scene!= null)
//					Application.LoadLevel(scene);
//			}
		}
		if(jiggle)
		{
			//Debug.Log("Jiggling");
			transform.position = Vector3.Lerp(transform.position,new Vector3(jigposx, 0,transform.position.z),Time.deltaTime);
			jigposx *= -1;
			if(!deactivateCalled)
			{
				deactivateCalled = true;
				StartCoroutine("deactivateJiggle");
			}
		}

	}

	public void setSong(AudioClip au)
	{
//		AudioSource _as = GetComponent<AudioSource>();
//		Object.Destroy(_as);
		clipToPlay = au;
	}

	public AudioClip getSong()
	{
		return clipToPlay;
	}

	public void setToInitPos()
	{
		newPos = initPos;
	}
	public void setToScenePos()
	{
		newPos = new Vector3(0,0,transform.position.z);
	}
	private IEnumerator deactivateJiggle()
	{
		yield return new WaitForSeconds(0.5f);
		//Debug.Log("Stopped Jiggling");
		jiggle = false;
		transform.position = new Vector3(0,0,transform.position.z);
		deactivateCalled = false;
	}
}
