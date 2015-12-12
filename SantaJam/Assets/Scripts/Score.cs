using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Score : MonoBehaviour {

	public int	totalScore;
	public Text	scoreText;

	// Use this for initialization
	void Start () {
		totalScore = 0;
		SetScoreText();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void CalcScore (int score) {
		totalScore += score;
		print(totalScore);
		SetScoreText();
	}

	void SetScoreText() {
		scoreText.text = "Total Score: " + totalScore.ToString();
	}
}
