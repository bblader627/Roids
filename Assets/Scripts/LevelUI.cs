using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelUI : MonoBehaviour
{
	Global global;
	Text levelText;

	void Start ()
	{
		GameObject globalObject = GameObject.Find ("GlobalObject");
		global = globalObject.GetComponent<Global> ();
		levelText = gameObject.GetComponent<Text> ();
	}

	void Update ()
	{
		levelText.text = "Level: " + global.level.ToString ();
	}
}
