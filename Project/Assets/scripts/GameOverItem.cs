
using UnityEngine;
using System.Collections;

public class GameOverItem : MonoBehaviour
{
	public bool isRetryButton;

	public DataCollector collector;

	private GUIText label;

	void Start()
	{
		label = gameObject.GetComponent<GUIText>();

		if (collector == null) {
			collector = GameObject.Find("DataCollector").GetComponent<DataCollector>();
		}
	}

	void OnMouseEnter()
	{
		label.color = Color.green;
	}
	void OnMouseExit()
	{
		label.color = Color.white;
	}

	void OnMouseDown()
	{
		if (collector != null) {
			collector.close();
		}

		if (isRetryButton) {
			Application.LoadLevel(Application.loadedLevel);
		} else {
			Application.LoadLevel(0);
		}
	}
}
