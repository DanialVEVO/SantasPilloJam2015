using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Score : MonoBehaviour {

	public int	totalScore;
	
	public ParticleSystem pickupParticle;

    [SerializeField]
    int maxScore = 200;

    [SerializeField]
    float fadeSpeed = 1.0f;

    Text scoreText;

    RectTransform scoreBar;

    bool win = false;

    GameObject winScreen;

    Image hit;

    float red = 0.0f;

    float oneWay = 1.0f;

    // Use this for initialization
    void Start () {

        scoreBar = GameObject.Find("Canvas").transform.FindChild("ScoreBar").transform.FindChild("Image").GetComponent<RectTransform>(); ;
        
       // scoreText = GameObject.Find("Canvas").transform.FindChild("Count").GetComponent<Text>();

        winScreen = GameObject.Find("Canvas").transform.FindChild("YouWon").gameObject;

        winScreen.SetActive(false);

        hit = GameObject.Find("Canvas").transform.FindChild("Image").GetComponent<Image>();

       

        totalScore = 0;

		SetScoreText();




    }
	
	// Update is called once per frame
	void Update () {

       // Debug.Log(red);

        if (red > 0.0f)
        {
            red += Time.deltaTime * fadeSpeed * oneWay;

            if (red >= 1.0f)
            {
                red = 1.0f;
                oneWay = -1.0f;
            }

            hit.color = new Color(red, 0, 0, 1);

            Debug.Log(red + " and color: " + hit.color);

        }
        else
        {
            red = 0.0f;
            oneWay = 1.0f;

            hit.color = new Color(red, 0, 0, 1);
        }
	
	}

	public void CalcScore (int score) {

        if (win)
            return;

		if(score > 0){
			Instantiate(pickupParticle, transform.position, Quaternion.identity);
		}

        if (score < 0)
            red += Time.deltaTime;
		
		//totalScore = Mathf.Max(0, totalScore + score);
        totalScore = Mathf.Clamp(totalScore+score, 0, maxScore);

        if (totalScore == maxScore)
            SetEnd();

        //print(totalScore);
        SetScoreText();
	}

	void SetScoreText() {

		//scoreText.text = "Total Score: " + totalScore.ToString();

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

    void RedFlash()
    {
        Debug.Log("hit");

        hit.CrossFadeColor(new Color(1, 0, 0), 0.2f, false, true);
    }

   

}
