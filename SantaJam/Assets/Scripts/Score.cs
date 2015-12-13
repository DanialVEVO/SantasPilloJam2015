using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Score : MonoBehaviour {

	public int	totalScore;
	
	public ParticleSystem pickupParticle;

    [SerializeField]
    int maxScore = 200;

    Text scoreText;

    RectTransform scoreBar;

    bool win = false;

    GameObject winScreen;

    // Use this for initialization
    void Start () {

        scoreBar = GameObject.Find("Canvas").transform.FindChild("ScoreBar").transform.FindChild("Image").GetComponent<RectTransform>(); ;
        
        scoreText = GameObject.Find("Canvas").transform.FindChild("Count").GetComponent<Text>();

        winScreen = GameObject.Find("Canvas").transform.FindChild("YouWon").gameObject;

        winScreen.SetActive(false);

        totalScore = 0;

		SetScoreText();




    }
	
	// Update is called once per frame
	void Update () {

       
	
	}

	public void CalcScore (int score) {

        if (win)
            return;

		if(score > 0){
			Instantiate(pickupParticle, transform.position, Quaternion.identity);
		}
		
		//totalScore = Mathf.Max(0, totalScore + score);
        totalScore = Mathf.Clamp(totalScore+score, 0, maxScore);

        if (totalScore == maxScore)
            SetEnd();

        //print(totalScore);
        SetScoreText();
	}

	void SetScoreText() {

		scoreText.text = "Total Score: " + totalScore.ToString();

        // Debug.Log((Mathf.Abs(scoreBar.offsetMin.x) * 2) * (totalScore / maxScore));

        float floatMax = maxScore;

        scoreBar.offsetMax = new Vector2((Mathf.Abs(scoreBar.offsetMin.x) * 2) * (totalScore / floatMax) + scoreBar.offsetMin.x, scoreBar.offsetMax.y);
	}

    void SetEnd()
    {
        win = true;

        winScreen.SetActive(true);

        GameObject.Find("LevelManagerObject").GetComponent<LevelManager>().EndLevelSpawn();
    }

}
