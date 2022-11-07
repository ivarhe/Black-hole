using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderController : MonoBehaviour
{

    private GameObject hand;

    public delegate void BreakObject();
    public static event BreakObject onBreak;

    public int strength;
    void OnEnable()
    {
        armController.onHit += breakDown;
    }

    void OnDisable()
    {
        armController.onHit += breakDown;
    }

    void Start()
    {
        hand = GameObject.Find("hand");
        strength = 3;
    }

    void breakDown()
    {
        // retrieve the player object
        Debug.Log("Breakdown" + strength);
        if ((hand.transform.position - this.transform.position).sqrMagnitude < 3 * 10)
        {
            Color col = new Color(Random.value, Random.value, Random.value);
            GetComponent<Renderer>().material.color = col;
            if (strength > 0)
            {
                strength--;
            }
            else
            {   
                Debug.Log("onBreak");
                onBreak();
                Destroy(this.gameObject);
            }
        }
    }
}
