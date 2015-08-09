using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreUI : MonoBehaviour
{
	Global global;
	Text scoreText;

	void Start ()
	{
		GameObject globalObject = GameObject.Find ("GlobalObject");
		global = globalObject.GetComponent< Global > ();
		scoreText = gameObject.GetComponent<Text> ();
	}

	void Update ()
	{
		scoreText.text = "Score: " + global.score.ToString ();
	}
}
