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
    public float scoreCount;
    public float highScoreCount;

    public float pointsPerSecond;

    public bool ScoreIncreasing;
    // Update is called once per frame

    void Start() {
        
    }
    void Update()
    {
        if (ScoreIncreasing) {
            scoreCount += pointsPerSecond * Time.deltaTime;
        }

        scoreText.text = "Score: " + Mathf.Round(scoreCount);

        if (scoreCount > highScoreCount) {
            highScoreCount = scoreCount;
        }
        highScoreText.text = "Highscore " + Mathf.Round(highScoreCount);
        Debug.Log("highscore: " + highScoreCount);
    }

    //if start game: start timer
    //if player death: stop timer
}
