using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LivesUI : MonoBehaviour
{
	Global global;
	Text livesText;

	void Start ()
	{
		GameObject globalObject = GameObject.Find ("GlobalObject");
		global = globalObject.GetComponent<Global> ();
		livesText = gameObject.GetComponent<Text> ();
	}

	void Update ()
	{
		livesText.text = "Lives: " + global.livesLeft.ToString ();
	}
}
