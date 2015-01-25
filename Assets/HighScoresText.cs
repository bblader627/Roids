using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

public class HighScoresText : MonoBehaviour {
	private Text highScoresText;
	private List<int> scores;
	private List<string> names;

	// Use this for initialization
	void Start () {

		names = new List<string>();
		scores = new List<int>();
		LoadLeaderboard ();

		highScoresText = gameObject.GetComponent<Text>();

		string highScores = "";
		for(int i = 0; i < 10; i++) {
			highScores += (i + 1) + ") " + names[i]  + " - " + scores[i] + "\n\n";
		}

		highScoresText.text = highScores;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void LoadLeaderboard() {
		StreamReader reader = new StreamReader (File.OpenRead (@"leaderboard.csv"));
		
		
		while (!reader.EndOfStream) {
			var line = reader.ReadLine ();
			var values = line.Split(',');
			
			names.Add(values[0].ToString());
			scores.Add(int.Parse(values[1]));
		}
		
		//Debug.Log ("Highest score is " + names [0] + " " + scores [0]);
	}
}
