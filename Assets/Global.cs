using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

public class Global : MonoBehaviour
{
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
	public List<int> scores;
	public List<string> names;
	public int multiplier;
	public GameObject reportObj;
	public Text reportTextObj, scoreText, livesText, multiplierText, levelText;

	void Awake ()
	{
		DontDestroyOnLoad (this);
	}

	void Start ()
	{
		score = 0;
		timer = 0;
		level = 1;
		livesLeft = 3;

		asteroidsRemaining = 9 * 3;
		maxBullets = 4;
		numberOfBullets = 0;
		multiplier = 1;

		spawnPeriod = 10.0f;
		numberSpawnedEachPeriod = 1;

		// Hide the report and report text
		reportObj.GetComponent<LevelReport> ().enabled = false;
		reportTextObj.enabled = false;

		names = new List<string> ();
		scores = new List<int> ();
		LoadLeaderboard ();

		originInScreenCoords = Camera.main.WorldToScreenPoint (new Vector3 (0, 0, 0));

		SpawnAsteroids (3);
	}

	void SpawnAsteroids (int numberOfAsteroids)
	{
		float width = Screen.width;
		float height = Screen.height;
		for (int i = 0; i < numberOfAsteroids; i++) {
			float horizontalPos = Random.Range (0.0f, width);
			float verticalPos = Random.Range (0.0f, height);
			Instantiate (objToSpawn, Camera.main.ScreenToWorldPoint (
				new Vector3 (horizontalPos, verticalPos, originInScreenCoords.z)), Quaternion.identity);
		}
	}

	public void StartLevel ()
	{
		// Hide the report again.
		reportObj.GetComponent<LevelReport> ().enabled = false;
		reportTextObj.enabled = false;

		// Enable the HUD
		scoreText.enabled = true;
		livesText.enabled = true;
		levelText.enabled = true;
		multiplierText.enabled = true;

		// Spawn the correct number of asteroids and reset the position of the ship.
		// then reset all the info
		level++;
		asteroidsRemaining = (level + 2) * 9;
		livesLeft = 3;
		timer = 0;
		spawnPeriod = 10.0f;

		// Destroy all the mults in the scene and set multiplier to 1
		Object[] multipliers;
		multipliers = GameObject.FindGameObjectsWithTag ("Multiplier");
		foreach (Object mult in multipliers) {
			Destroy (mult);
		}
		multiplier = 1;

		// Destroy all the bullets and num to zero
		Object[] bullets;
		bullets = GameObject.FindGameObjectsWithTag ("Bullet");
		foreach (Object bullet in bullets) {
			Destroy (bullet);
		}
		numberOfBullets = 0;

		// Destroy all the UFOs
		Object[] ufos;
		ufos = GameObject.FindGameObjectsWithTag ("UFO");
		foreach (Object ufo in ufos) {
			Destroy (ufo);
		}

		// Destroy all ufo bullets
		Object[] ufoBullets;
		ufoBullets = GameObject.FindGameObjectsWithTag ("UFOBullet");
		foreach (Object ufoBullet in ufoBullets) {
			Destroy (ufoBullet);
		}

		// Before transforming ship, give it the shield again.
		GameObject shipObj = GameObject.FindGameObjectWithTag ("Ship");
		Ship s = shipObj.GetComponent<Ship> ();
		s.Respawn ();

		shipObj.GetComponent<Rigidbody> ().Sleep ();//This should reset the forces on the ship.
		Transform shipTransform = shipObj.transform;
		shipTransform.position = Vector3.zero;
		SpawnAsteroids (level + 2);
	}

	void EndOfLevelReport ()
	{
		// Resolves issue with accumulating score on report screen
		asteroidsRemaining = -1;

		// Disable the other text
		scoreText.enabled = false;
		livesText.enabled = false;
		levelText.enabled = false;
		multiplierText.enabled = false;

		// Pause game
		Time.timeScale = 0;
		reportObj.GetComponent<LevelReport> ().enabled = true;
		reportTextObj.enabled = true;
		reportTextObj.GetComponent<ReportText> ().DisplayReport ();
	}

	void Update ()
	{
		if (asteroidsRemaining == 0) {
			EndOfLevelReport ();
		}

		// Spawn UFOs at very top left corner of screen with a random direction
		timer += Time.deltaTime;
		if (timer > spawnPeriod) {
			timer = 0;
			Instantiate (ufoToSpawn, Camera.main.ScreenToWorldPoint (new Vector3 (0.0f, 0.0f, originInScreenCoords.z)), Quaternion.identity);
		}
	}

	void LoadLeaderboard ()
	{
		StreamReader reader = new StreamReader (File.OpenRead (@"leaderboard.csv"));
		
		while (!reader.EndOfStream) {
			var line = reader.ReadLine ();
			var values = line.Split (',');
			
			names.Add (values [0].ToString ());
			scores.Add (int.Parse (values [1]));
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
		if (score < scores [9]) {
			//Less than the lowest score, so no way it gets added
			return;
		}
		
		// Otherwise, find where we put it
		for (int i = 0; i < 10; i++) {
			if (score >= scores [i]) {
				int tempScore = scores [i];
				string tempName = names [i];
				scores [i] = score;
				names [i] = name;

				for (int j = i + 1; j < 10; j++) {
					int tempScore2;
					string tempName2;
					
					tempScore2 = scores [j];
					scores [j] = tempScore;
					tempScore = tempScore2;
					
					tempName2 = names [j];
					names [j] = tempName;
					tempName = tempName2;
				}

				break;
			}
		}

		SaveLeaderboard ();
	}
}
