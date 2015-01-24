using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelUI : MonoBehaviour {
	Global globalObj;
	Text levelText;
	// Use this for initialization
	void Start () {
		GameObject g = GameObject.Find ("GlobalObject");
		globalObj = g.GetComponent<Global>();
		levelText = gameObject.GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		levelText.text = "Level: " + globalObj.level.ToString ();
	}
}
