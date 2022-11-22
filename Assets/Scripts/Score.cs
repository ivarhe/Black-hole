using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Score : MonoBehaviour
{
    // Start is called before the first frame update
   
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;

    public TextMeshProUGUI ifHighScore;
    public float scoreCount;
    public static float highScoreCount;

    public float pointsPerSecond;

    public bool ScoreIncreasing;
    // Update is called once per frame

    void Start() {
            highScoreCount = PlayerPrefs.GetFloat("HighScore");
    }
    void Update()
    {
        if (ScoreIncreasing) {
            scoreCount += pointsPerSecond * Time.deltaTime;
        }

        scoreText.text = "Your Score: " + Mathf.Round(scoreCount);

        if (scoreCount > highScoreCount) {
            highScoreCount = scoreCount;
            PlayerPrefs.SetFloat("HighScore",highScoreCount);
        }
        highScoreText.text = "HighScore: " + Mathf.Round(highScoreCount);
        Debug.Log("highscore: " + highScoreCount);
    }

    void setTexts() {
        if (scoreCount < highScoreCount) {
            ifHighScore.text = "hey";
        }
    }

    //if highscore: send en tekst
    //else: en annen
}
