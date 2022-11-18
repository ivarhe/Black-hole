using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{

    public delegate void HandCollision();
    public static event HandCollision collision;

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == 11) // 11 is the layer of the toys
        {
            if (collision != null)
            {
                collision();
            }
        }
    }
}
