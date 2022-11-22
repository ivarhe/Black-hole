using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class heartSystem : MonoBehaviour
{

    public GameObject[] hearts;
    public int life;
    public bool dead;

    void OnEnable()
    {
        PlayerController.onPlayerDamage += TakeDamage;
    }

    void OnDisable()
    {
        PlayerController.onPlayerDamage -= TakeDamage;
    }

    private void start()
    {
        life = hearts.Length;
    }

    public void TakeDamage()
    {
        if (life >= 1)
        {
            life -= 1;
            Destroy(hearts[life].gameObject);
            if (life < 1)
            {
                dead = true;
                PlayerPrefs.SetInt("Dead", 1);
                Debug.Log("we dead");
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }

    }
}
