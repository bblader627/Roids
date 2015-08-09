using UnityEngine;
using System.Collections;

public class LevelReport : MonoBehaviour
{
	private GUIStyle buttonStyle;

	void OnGUI ()
	{
		GUILayout.BeginArea (new Rect (10, Screen.height / 2 + 100, Screen.width - 10, 200));

		if (GUILayout.Button ("Continue")) {
			// Unpause game
			Time.timeScale = 1;

			// Start level
			GameObject globalObject = GameObject.Find ("GlobalObject");
			Global global = globalObject.GetComponent<Global> ();
			global.StartLevel ();

		}

		GUILayout.EndArea ();
	}
}