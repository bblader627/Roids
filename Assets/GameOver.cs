using UnityEngine;
using System.Collections;

public class GameOver : MonoBehaviour
{
	private GUIStyle buttonStyle;
	private GameObject globalObject;
	private Global global;

	public string name = "Enter name...";

	void Start ()
	{
		globalObject = GameObject.Find ("GlobalObject");
		global = globalObject.GetComponent<Global> ();
	}

	void OnGUI ()
	{
		GUILayout.BeginArea (new Rect (10, Screen.height / 2 + 100,
		                             Screen.width - 10, 200));
		name = GUILayout.TextField (name, 20);

		global.name = name;

		Debug.Log ("Player score: " + global.score);

		if (GUILayout.Button ("Ok")) {
			global.UpdateLeaderboard ();
			Application.LoadLevel ("LeaderboardScene");
		}
		GUILayout.EndArea ();
	}
}