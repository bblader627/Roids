using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

public class ReportText : MonoBehaviour
{
	private Text levelReportText;
	private int score;
	private int lives;
	private int multiplier;
	private int level;

	void Start ()
	{
		score = 0;
		lives = 0;
		multiplier = 0;
		level = 0;
	}

	public void DisplayReport ()
	{
		GameObject globalObject = GameObject.Find ("GlobalObject");
		Global global = globalObject.GetComponent<Global> ();
		score = global.score;
		lives = global.livesLeft;
		multiplier = global.multiplier;
		level = global.level;

		levelReportText = gameObject.GetComponent<Text> ();
		
		string report = "Level " + level + "  Complete.\n\n\n";
		report += "Score: " + score + "\n";
		report += "Multiplier: " + multiplier + "\n";
		report += "Lives Remaining: " + lives + "\n";
		report += "Bonus: " + multiplier + " * " + lives + " = " + (multiplier * lives) + "\n";
		report += "Total: " + score + " + " + (multiplier * lives) + " = " + (score + (multiplier * lives));

		levelReportText.text = report;

		global.score = (score + (multiplier * lives));
	}
}
