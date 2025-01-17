﻿using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;

public class Leaderboard : MonoBehaviour
{
	public ArrayList names;
	public ArrayList scores;
	new public string name;
	public int score;

	void Awake ()
	{
		DontDestroyOnLoad (this);
	}

	void Start ()
	{
		names = new ArrayList ();
		scores = new ArrayList ();
		LoadLeaderboard ();
	}

	void LoadLeaderboard ()
	{
		StreamReader reader = new StreamReader (File.OpenRead (@"leaderboard.csv"));

		while (!reader.EndOfStream) {
			var line = reader.ReadLine ();
			var values = line.Split (',');
			
			names.Add (values [0]);
			scores.Add (values [1]);
		}
	}

	void SaveLeaderboard ()
	{
		StringBuilder output = new StringBuilder ();

		for (int i = 0; i < 10; i++) {
			string line = string.Format ("{0},{1}{2}", names [i], scores [i], "\n");
			output.Append (line);
		}

		File.WriteAllText ("leaderboard.csv", output.ToString ());
	}

	public void UpdateLeaderboard ()
	{
		// Given the current score, find if and where it belongs in the list, and add it
		// First check if it is greater than the lowest score
		if (score < (int)scores [9]) {
			// Less than the lowest score, so no way it gets added
			return;
		}

		//O therwise, find where we put it
		for (int i = 0; i < 10; i++) {
			if (score >= (int)scores [i]) {
				int tempScore = (int)scores [i];
				string tempName = names [i].ToString ();
				scores [i] = score;
				names [i] = name;

				for (int j = i + 1; j < 10; j++) {
					int tempScore2;
					string tempName2;

					tempScore2 = (int)scores [j];
					scores [j] = tempScore;
					tempScore = tempScore2;

					tempName2 = names [j].ToString ();
					names [j] = tempName;
					tempName = tempName2;
				}

				break;
			}
		}
	}
}
