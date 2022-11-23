using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GetStarted : MonoBehaviour
{
    private Score scoreStarter; 
    private heartSystem life;
    
    public Animator animator;

    public void PlayGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        animator.SetTrigger("FadeOut");

        scoreStarter = FindObjectOfType<Score>();
        scoreStarter.scoreCount = 0;
        PlayerPrefs.SetFloat("Score", 0);
        scoreStarter.ScoreIncreasing = true;

    }

    public void Update() {
        if(life.dead == true){
            scoreStarter.ScoreIncreasing = false;
        }
    }

    public void StartOver() {
        animator.SetTrigger("FadeOut");
        Debug.Log("Start over!");
        //animator.SetTrigger("FadeIn");
        SceneManager.LoadScene(0);
    }

}
