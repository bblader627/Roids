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

	public string name = "Enter name...";

	void OnGUI (){
		GUILayout.BeginArea(new Rect(10, Screen.height / 2 + 100,
		                             Screen.width -10, 200));
		name = GUILayout.TextField (name, 20);

		GameObject leaderboardObj = GameObject.Find("LeaderboardObject");
		Leaderboard l = leaderboardObj.GetComponent<Leaderboard>();

		l.lastPlayer = name;

		Debug.Log ("Last Player: " + l.lastPlayer);

		// Load the main scene
		// The scene needs to be added into build setting to be loaded!
		if (GUILayout.Button("Ok"))
		{
			Application.LoadLevel("LeaderboardScene");
		}
		GUILayout.EndArea();
	}
}