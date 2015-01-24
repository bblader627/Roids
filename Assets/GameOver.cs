using UnityEngine;
using System.Collections;

public class GameOver : MonoBehaviour {
	private GUIStyle buttonStyle;
	// Use this for initialization
	void Start () {
	}
	// Update is called once per frame
	void Update () {
	}
	public string test = "Test.";
	void OnGUI (){
		GUILayout.BeginArea(new Rect(10, Screen.height / 2 + 100,
		                             Screen.width -10, 200));
		test = GUILayout.TextField (test, 20);
		// Load the main scene
		// The scene needs to be added into build setting to be loaded!
		if (GUILayout.Button("Ok"))
		{
			Application.LoadLevel("LeaderboardScene");
		}
		GUILayout.EndArea();
	}
}