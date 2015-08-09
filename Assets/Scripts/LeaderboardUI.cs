using UnityEngine;
using System.Collections;

public class LeaderboardUI : MonoBehaviour
{
	private GUIStyle buttonStyle;
	
	void OnGUI ()
	{
		GUILayout.BeginArea (new Rect (10, Screen.height / 2 + 100, Screen.width - 10, 200));

		if (GUILayout.Button ("Continue")) {
			Application.LoadLevel ("PlayAgainScene");
		}

		GUILayout.EndArea ();
	}
}