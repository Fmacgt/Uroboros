
using UnityEngine;
using System.Collections;

public class RepulsionFieldOuter : MonoBehaviour
{
	public string playerMask;

	void OnTriggerExit2D(Collider2D obj)
	{
		if (obj.tag == playerMask) {
			Application.LoadLevel(Application.loadedLevel);
		}
	}
}
