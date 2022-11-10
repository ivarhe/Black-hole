using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GetStarted : MonoBehaviour
{

    public void PlayGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void StartOver() {
        SceneManager.LoadScene(0);
    }

}