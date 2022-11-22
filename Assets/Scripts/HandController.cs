using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{

    public delegate void HandCollision();
    public static event HandCollision collision;
    Animator animator;

    void start()
    {
        animator = GetComponent<Animator>(); // To have our animator while the script is running.
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == 11) // 11 is the layer of the toys
        {
            if (collision != null)
            {
                collision();
            }
            animator.SetTrigger("Slap");
        }
    }
}
