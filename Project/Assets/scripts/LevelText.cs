
using UnityEngine;
using System.Collections;

public class LevelText : MonoBehaviour
{
	public float baseSize = 20f;
	public float maxSize = 30f;
	public float resizeSpeed = 10f;
	
	public GUIText levelLabel;
	public GUIText percentLabel;
	
	public int level;
	public int percent;
	
	//private GUIText levelLabel;
	
	private float floatSize = 0f;
	private bool enlarging = false;
	private bool shrinking = false;

	public Vector2 oldPos;
	public Vector2 hidePos;

	void Start()
	{
		if (levelLabel == null) {
			levelLabel = GameObject.Find("LevelValue").GetComponent<GUIText>();
		}
		
		if (percentLabel == null) {
			percentLabel = GameObject.Find("PercentValue").GetComponent<GUIText>();
		}
		
		floatSize = levelLabel.fontSize;

		//hidePos = new Vector2(5f, 5f);
		
		level = 0;
		percent = 0;
	}

	void Update()
	{
		if (enlarging) {
			floatSize += resizeSpeed * Time.deltaTime;
			if (floatSize >= maxSize) {
				enlarging = false;
				shrinking = true;

				floatSize = maxSize;
			}

			levelLabel.fontSize = (int)floatSize;
		} else if (shrinking) {
			floatSize -= resizeSpeed * Time.deltaTime;
			if (floatSize <= baseSize) {
				shrinking = false;

				floatSize = baseSize;
			}

			levelLabel.fontSize = (int)floatSize;
		}
	}

	public void setValue(int level)
	{
		levelLabel.text = level.ToString();

		// 'flash' the string
		enlarging = true;
	}
	
	public void setPercent(int percent)
	{
		this.percent = percent;
		if (percent < 10) {
			percentLabel.text = "0" + percent.ToString();
		} else {
			percentLabel.text = percent.ToString();// + "%";
		}
	}

	public void hide()
	{
		transform.position = hidePos;
	}

	public void show()
	{
		transform.position = oldPos;
	}
}
