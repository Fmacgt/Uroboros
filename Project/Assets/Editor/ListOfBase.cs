using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class Hierarchy {
	[SerializeField]
	private string name;
	public string function;
	public int parent;
	private Hierarchy upper;
	private int level;
	private GameObject go;
	private static Hierarchy sel = null;

	public static Hierarchy getSelected()
	{
		return sel;
	}
	public void setGameObject(GameObject g)
	{
		g.GetComponent<TextMesh>().text = name;
		go = g;
	}
	public void setPosition (Vector3 v, Vector3 selected)
	{
		if(!go.activeSelf)
			return;
		go.transform.position += v;
		if(go.transform.position == selected)
		{
			sel = this;
			go.GetComponent<MeshRenderer>().material.color = Color.red;
			//go.transform.position += new Vector3(0,0,2f);
		}
		else
		{
			go.GetComponent<MeshRenderer>().material.color = Color.white;
//			if(go.transform.position.z == 2f)
//			{
//				go.transform.position -= new Vector3(0,0,2f);
//			}
		}
	}
	public void setUpper(Hierarchy h)
	{
		upper = h;
		if(upper != null)
			level = upper.getLevel() + 1;
	}
	public Hierarchy getUpper()
	{
		return upper;
	}
	public int getLevel()
	{
		return level;
	}
	public void setActive(bool tf)
	{
		go.SetActive(tf);
	}
	public void doFunc(GameObject cam)
	{
		cam.GetComponent<CameraScript>().animateCamera = true;
		if(string.Compare(function,"Exit") == 0)
		{
			Application.Quit();
		}
		else
		{
			cam.GetComponent<CameraScript>().scene = function;
		}
	}
}

[System.Serializable]
public class ListOfBase:MonoBehaviour {

	public Hierarchy[] list;
	public GameObject text3d;
//	private GameObject[] menuItems;
	private Vector3 pos, initPos, oneStep, stepDown;
	private List<int> parents;
	private int rowCount;
	private int atPos, atLevel;
	private Vector3 maxTimeLeft;
	private float timeLeft, timeShave;
	private GameObject cam;
	private float startTime;
	public float selectionTime = 3f;
	private string func = null;

	void Start()
	{
		parents = new List<int>();
		atPos = -1;
		atLevel = 0;
		initPos = transform.position;
		oneStep = new Vector3 (5f,0,0);
		stepDown = new Vector3 (0,-3f,0);
		cam = GameObject.FindGameObjectWithTag("MainCamera");
		timeLeft = 5;
		timeShave = selectionTime/timeLeft;

		for (int i = 0; i<list.Length; i++)
		{
			parents.Add(list[i].parent);
			int index = list[i].parent;
			if(index == 0)
				list[i].setUpper(null);
			else
				list[i].setUpper(list[index-1]);
		}
		parents = parents.Distinct().ToList<int>();
		rowCount = 0;
		foreach (Hierarchy h in list)
		{
			if(h.parent == 0)
				rowCount++;
		}
		//menuItems = new GameObject[list.Length];

		foreach (int i in parents)
		{
			pos = initPos;
			foreach(Hierarchy h in list)
			{
				if(i == h.parent)
				{
					pos += oneStep;	
					GameObject g = (GameObject) Instantiate(text3d, pos + (stepDown*h.getLevel()), 
					                                        Quaternion.identity);
					h.setGameObject(g);
				}
			}
		}

		foreach ( Hierarchy h in list)
		{
			if (h.getLevel() > atLevel)
				h.setActive(false);
			else
				h.setActive(true);
		}
	}
	void Update()
	{
		if(startTime != 0 && atPos != -1)
		{
			GetComponentInChildren<TextMesh>().text = timeLeft.ToString();
			if(Time.time - startTime >= ((6-timeLeft)*timeShave))
			{
				timeLeft--;
			}
		}
		else
		{
			GetComponentInChildren<TextMesh>().text = "";
			timeLeft = 5;
		}
		if(startTime != 0 &&(Time.time - startTime >= selectionTime))
		{
			if(hasChild(Hierarchy.getSelected()))
			{
				atLevel++;
				rowCount = 0;
				foreach ( Hierarchy h in list)
				{
					int lvl = h.getLevel();
					if (lvl > atLevel || 
					    lvl == atLevel && h.getUpper() != Hierarchy.getSelected())
						h.setActive(false);
					else
					{
						h.setActive(true);
						if(h.getLevel() == atLevel) rowCount++;
					}
				}
				transform.position += stepDown;
				atPos = -1;
			}
			else
			{
				Hierarchy.getSelected().doFunc(cam);
			}
			startTime = 0;
		}
		if(Input.GetKeyDown(KeyCode.Space))
		{
			startTime = Time.time;
			timeLeft = 5;
			atPos++;
			if(atPos < rowCount)
			{
				foreach(Hierarchy h in list)
				{
					if( h.getLevel() == atLevel)
						h.setPosition(oneStep*-1, transform.position);
				}
			}
			else
			{
				foreach(Hierarchy h in list)
				{
					if( h.getLevel() == atLevel)
						h.setPosition(oneStep*rowCount, transform.position);
				}
				atPos = -1;
			}
		}
	}
	bool hasChild(Hierarchy h)
	{
		for(int i = 0; i<list.Length; i++)
		{
			if(h == list.ElementAt(i).getUpper())
				return true;
		}
		return false;
	}
}
