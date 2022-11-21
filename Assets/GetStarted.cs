using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GetStarted : MonoBehaviour
{
    private Score scoreStarter; 
    private heartSystem life;

    public void PlayGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        scoreStarter = FindObjectOfType<Score>();
        scoreStarter.scoreCount = 0;
        scoreStarter.ScoreIncreasing = true;



    }

    public void Update() {
        if(life.dead == true){
            scoreStarter.ScoreIncreasing = false;

        }
    }

    public void StartOver() {
        SceneManager.LoadScene(0);
    }

}
