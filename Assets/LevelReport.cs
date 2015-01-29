using UnityEngine;
using System.Collections;

public class LevelReport : MonoBehaviour {
	private GUIStyle buttonStyle;
	// Use this for initialization
	void Start () {
		
	}
	// Update is called once per frame
	void Update () {
		
	}

	void OnGUI (){
		GUILayout.BeginArea(new Rect(10, Screen.height / 2 + 100,
		                             Screen.width -10, 200));
		// Load the main scene
		// The scene needs to be added into build setting to be loaded!
		if (GUILayout.Button("Continue"))
		{
			// Unpause game!
			Time.timeScale = 1;

			//start level
			GameObject globalObj = GameObject.Find("GlobalObject");
			Global g = globalObj.GetComponent<Global>();
			g.StartLevel();

		}
		GUILayout.EndArea();
	}
}