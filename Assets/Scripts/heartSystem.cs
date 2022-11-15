using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class heartSystem : MonoBehaviour
{

    public GameObject[] hearts;
    public int life;
    private bool dead;

    public static event Action onPlayerDeath;

    private void start() {
        life = hearts.Length;
    }
    // Update is called once per frame
    void Update()
    {
        if(dead==true) {
            Debug.Log("we dead");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    public void TakeDamage(int d) {
        if (life>=1) {
            life -= d;
            Destroy(hearts[life].gameObject);
            if(life <1) {
                dead = true;
            }
        }

    }
}
