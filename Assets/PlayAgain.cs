using UnityEngine;
using System.Collections;

public class PlayAgain : MonoBehaviour
{
	private GUIStyle buttonStyle;

	void OnGUI ()
	{
		GUILayout.BeginArea (new Rect (10, Screen.height / 2 + 100, Screen.width - 10, 200));

		if (GUILayout.Button ("Yes")) {
			Application.LoadLevel ("GameplayScene");
		}

		if (GUILayout.Button ("No")) {
			Application.LoadLevel ("TitleScreen");
		}

		GUILayout.EndArea ();
	}
}