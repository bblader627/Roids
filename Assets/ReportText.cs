using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

public class ReportText : MonoBehaviour {
	private Text levelReportText;
	private int score;
	private int lives;
	private int multiplier;
	private int level;

	// Use this for initialization
	void Start () {
		score = 0;
		lives = 0;
		multiplier = 0;
		level = 0;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//Displays the report
	public void DisplayReport() {
		GameObject globalObj = GameObject.Find("GlobalObject");
		Global g = globalObj.GetComponent<Global>();
		score = g.score;
		lives = g.livesLeft;
		multiplier = g.multiplier;
		level = g.level;

		levelReportText = gameObject.GetComponent<Text>();
		
		string report = "Level " + level + "  Complete.\n\n\n";
		report += "Score: " + score + "\n";
		report += "Multiplier: " + multiplier + "\n";
		report += "Lives Remaining: " + lives + "\n";
		report += "Bonus: " + multiplier + " * " + lives + " = " + (multiplier * lives) + "\n";
		report += "Total: " + score + " + " + (multiplier * lives) + " = " + (score + (multiplier * lives));

		levelReportText.text = report;

		//update the score!
		g.score = (score + (multiplier * lives));
	}
}
