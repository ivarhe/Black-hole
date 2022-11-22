using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToyController : MonoBehaviour
{

    // toggle a bool when the player collides with the toy

    public delegate void ToyCollision(GameObject toy);
    public delegate void HandCollision(GameObject toy);
    public static event ToyCollision collision;
    public static event HandCollision handCollision;

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "player")
        {
            //set kinematic to false
            //GetComponent<Rigidbody2D>().isKinematic = true;
        }
        if (col.gameObject.tag == "arm")
        {
            Debug.Log("collide");
            if (collision != null)
            {
                collision(gameObject);
            }
        }
        if (col.gameObject.tag == "hand")
        {
            Debug.Log("collide with hand");
            if (handCollision != null)
            {
                handCollision(gameObject);
            }
        }
    }

    // cancel collision when the player leaves the toy
    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.tag == "player")
        {
            //set kinematic to false
            //GetComponent<Rigidbody2D>().isKinematic = false;
        }
    }


}
