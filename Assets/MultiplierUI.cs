using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MultiplierUI : MonoBehaviour {
	Global globalObj;
	Text multiplierText;
	// Use this for initialization
	void Start () {
		GameObject g = GameObject.Find ("GlobalObject");
		globalObj = g.GetComponent< Global >();
		multiplierText = gameObject.GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		multiplierText.text = "Multiplier: " + globalObj.multiplier.ToString();
	}
}
