using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{
    public Animator animator;
    private int levelToLoad;
    private Score scoreStarter; 
    private heartSystem life;

    void Start() {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FadeToLevel(1);
        }
        if(life.dead == true){
            scoreStarter.ScoreIncreasing = false;
        }
    }

    public void FadeToLevel(int levelIndex)
    {
        levelToLoad = levelIndex;
        animator.SetTrigger("FadeOut");
    }

    public void OnFadeComplete()
    {
        Debug.Log("Fade complete");
        SceneManager.LoadScene(levelToLoad);
    }

    public void PlayGame() {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        FadeToLevel(1);

        scoreStarter = FindObjectOfType<Score>();
        scoreStarter.scoreCount = 0;
        PlayerPrefs.SetFloat("Score", 0);
        scoreStarter.ScoreIncreasing = true;

    }

    public void StartOver() {
        FadeToLevel(0);
    }

}
