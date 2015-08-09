using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MultiplierUI : MonoBehaviour
{
	Global global;
	Text multiplierText;

	void Start ()
	{
		GameObject globalObj = GameObject.Find ("GlobalObject");
		global = globalObj.GetComponent< Global > ();
		multiplierText = gameObject.GetComponent<Text> ();
	}

	void Update ()
	{
		multiplierText.text = "Multiplier: " + global.multiplier.ToString ();
	}
}
