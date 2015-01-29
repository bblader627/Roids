using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class Global : MonoBehaviour {

	public GameObject objToSpawn;
	public GameObject ufoToSpawn;
	public float timer;
	public float spawnPeriod;
	public int numberSpawnedEachPeriod;
	public Vector3 originInScreenCoords;
	public int score;
	public int level;
	public int livesLeft;
	public int asteroidsRemaining;
	public int numberOfBullets;
	public int maxBullets;
	//public ArrayList names;
	//public ArrayList scores;
	public List<int> scores;
	public List<string> names;

	public int multiplier;

	void Awake() {
		DontDestroyOnLoad (this);
	}

	// Use this for initialization
	void Start () {
		score = 0;
		timer = 0;
		level = 1;
		livesLeft = 3;
		//spawn first wave of asteroids
		//spawnAsteroids(3);
		//asteroidsRemaining = 3; //do the max? and only minus when there's medium hits
		asteroidsRemaining = 9 * 3;
		maxBullets = 4;
		numberOfBullets = 0;

		multiplier = 0;

		spawnPeriod = 10.0f;
		numberSpawnedEachPeriod = 1;


		names = new List<string>();
		scores = new List<int>();
		LoadLeaderboard ();

		/*
So here's a design point to consider:
- is the gameplay constrained by the screen size in any
particular way?
That might sound like a weird question, but it's actually a
significant one for asteroids if you want the game to play like
Asteroids on arbitrary screen dimensions. It's mostly here for
pedagogical reasons, though. The value that actually matters
here is the depth value. Since the gameplay takes place on a XZplane,
and we're looking down the Y-axis,
we're mainly interested in what the Y value of 0 maps to in the
camera's depth.
*/
		originInScreenCoords =
			Camera.main.WorldToScreenPoint(new Vector3(0,0,0));

		SpawnAsteroids(3);
	}

	void SpawnAsteroids(int numberOfAsteroids) {
		float width = Camera.main.GetScreenWidth();
		float height = Camera.main.GetScreenHeight();
		for( int i = 0; i < numberOfAsteroids; i++ )
		{
			float horizontalPos = Random.Range(0.0f, width);
			float verticalPos = Random.Range(0.0f, height);
			Instantiate(objToSpawn,
			            Camera.main.ScreenToWorldPoint(
				new Vector3(horizontalPos,
			            verticalPos,originInScreenCoords.z)),
			            Quaternion.identity );
		}
	}

	void StartLevel(int levelNumber) {
		//spawn the correct number of asteroids and reset the position of the ship.
		//then reset all the info

		asteroidsRemaining = (levelNumber + 2) * 9;
		livesLeft = 3;
		level = levelNumber;
		timer = 0;
		spawnPeriod = 10.0f;
		multiplier = 0;

		//I gotta destroy all the bullets and reset the number of bullets to 0;
		Object[] bullets;
		bullets = GameObject.FindGameObjectsWithTag ("Bullet");
		foreach (Object bullet in bullets) {
			Destroy (bullet);
		}
		numberOfBullets = 0;

		//gotta destroy all the UFOs
		Object[] ufos;
		ufos = GameObject.FindGameObjectsWithTag ("UFO");
		foreach (Object ufo in ufos) {
			Destroy (ufo);
		}

		Transform shipTransform = GameObject.FindGameObjectWithTag ("Ship").transform;
		shipTransform.position = Vector3.zero;

		SpawnAsteroids (levelNumber + 2);
	}

	/*
	 * Loads in the leaderboard from a csv file.
	 */

	// Update is called once per frame
	void Update () {
		//Check if level is over.
		if (asteroidsRemaining == 0) {
			//Then start the next level
			level++;
			StartLevel(level);
		}

		//spawn ufos
		GameObject globalObj = GameObject.Find("GlobalObject");
		Global g = globalObj.GetComponent<Global>();
		//Physics engine handles movement, empty for now. }
		timer += Time.deltaTime;
		if (timer > spawnPeriod) {
			timer = 0;
			float horizontalPos = Random.Range(0.0f, Camera.main.GetScreenWidth());
			float verticalPos = Random.Range(0.0f, Camera.main.GetScreenHeight());
			Instantiate(ufoToSpawn, Camera.main.ScreenToWorldPoint(new Vector3(horizontalPos, verticalPos,originInScreenCoords.z)), Quaternion.identity );
		}
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
	
	void SaveLeaderboard() {
		StringBuilder output = new StringBuilder ();
		
		for (int i = 0; i < 10; i++) {
			string line = string.Format("{0},{1}{2}", names[i], scores[i], "\n");
			output.Append(line);
		}
		
		File.WriteAllText("leaderboard.csv", output.ToString());
	}
	
	//public void UpdateLeaderboard(string name, int score) {
	public void UpdateLeaderboard() {
		//Given the current score, find if and where it belongs in the list, and add it
		//First check if it is greater than the lowest score
		if (score < scores[9]) {
			//Less than the lowest score, so no way it gets added
			return;
		}
		
		//Otherwise, find where we put it
		for (int i = 0; i < 10; i++) {
			if(score >= scores[i]) {
				//found where to put it
				//so i have to save what was there, insert the new score, and then shift the rest down
				int tempScore = scores[i];
				string tempName = names[i];
				scores[i] = score;
				names[i] = name;
				
				//shift everything down
				for(int j = i + 1; j < 10; j++) {
					int tempScore2;
					string tempName2;
					
					tempScore2 = scores[j];
					scores[j] = tempScore;
					tempScore = tempScore2;
					
					tempName2 = names[j];
					names[j] = tempName;
					tempName = tempName2;
				}
				
				//cancel the loop
				break;
			}
		}

		SaveLeaderboard ();
	}
}
