using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Score : MonoBehaviour
{
    // Start is called before the first frame update

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI gameOverText;
    public float scoreCount;
    public static float highScoreCount;

    public float pointsPerSecond;

    public static bool dead = false;

    public bool ScoreIncreasing;
    // Update is called once per frame

    void Start()
    {
        highScoreCount = PlayerPrefs.GetFloat("HighScore");
        scoreCount = PlayerPrefs.GetFloat("Score");
        dead = PlayerPrefs.GetInt("Dead") == 1;
        finalScoreText.text = "Final Score: " + PlayerPrefs.GetFloat("FinalScore");
        highScoreText.text = "High Score: " + PlayerPrefs.GetFloat("HighScore");
        gameOverText.text = PlayerPrefs.GetString("GameOverText");
        //scoreText.text = "Your Score: " + Mathf.Round(scoreCount);
        if (dead)
        {
            scoreCount = 0;
            PlayerPrefs.SetFloat("Score", 0);
            dead = false;
            PlayerPrefs.SetInt("Dead", 0);
            PlayerPrefs.SetString("GameOverText", "Game Over");
        }
    }
    void Update()
    {
        if (ScoreIncreasing)
        {
            scoreCount += pointsPerSecond * Time.deltaTime;
            PlayerPrefs.SetFloat("Score", scoreCount);
            PlayerPrefs.SetFloat("FinalScore", Mathf.Round(scoreCount));
        }

        scoreText.text = "Your Score: " + Mathf.Round(scoreCount);

        if (scoreCount > highScoreCount)
        {
            highScoreCount = scoreCount;
            PlayerPrefs.SetFloat("HighScore", Mathf.Round(highScoreCount));
            PlayerPrefs.SetString("GameOverText", "New HighScore! You have managed to keep the monster away for the longest amount of time! But not enough, it seems...");
        }
        highScoreText.text = "HighScore: " + Mathf.Round(highScoreCount);
        Debug.Log("highscore: " + highScoreCount);
    }

}
