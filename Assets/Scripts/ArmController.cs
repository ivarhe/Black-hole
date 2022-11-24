using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmController : MonoBehaviour
{

    //public LineRenderer lineRenderer;
    public LayerMask ToyObjects;

    //private GameObject Arm;
    public GameObject hand;
    float MOVE_VALUE = 0.005f;
    private int lastdirection = 1;

    /* Nyttige linker */
    //https://www.youtube.com/watch?v=2BH1yQXCpeU
    //https://github.com/llamacademy/line-renderer-collider/tree/main/Assets/Scripts

    public delegate void PushObject(GameObject hand);
    public static event PushObject StartPush;
    public static event PushObject StopPush;

    private bool canMove = true;

    Animator animator;


    void OnEnable()
    {
        ToyController.handCollision += Collide;
        GameController.move += Move;
    }

    void OnDisable()
    {
        ToyController.handCollision += Collide;
        GameController.move += Move;
    }

    void Collide(GameObject hand)
    {
        if (hand == this.hand)
        {
            this.canMove = false;
            //animator.SetTrigger("Slap");
            StartPush(hand);
            StartCoroutine(Push(hand));
        }
    }

/* // not working yet
    bool hitWall = false;

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "wall")
        {
            canMove = false;
            // rotate the hand 180 degrees when it collides with a wall
            //transform.Rotate(0f, 180f, 0f);
            hitWall = true;
            StartCoroutine(Turn());
        }
    }

    IEnumerator Turn()
    {
        //transform.Rotate(0f, 180f, 0f);
        yield return new WaitForSeconds(0.5f);
        canMove = true;
    }
    */

    IEnumerator Push(GameObject hand)
    {
        yield return new WaitForSeconds(5f);
        StopPush(hand);
    }

    void Move()
    {
        //animator.ResetTrigger("Slap");
        this.canMove = true;
    }

    /*
        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
            points = new Vector3[1];
            lineRenderer.GetPositions(points);
        }


        void SetTargetPosition(Vector3 targetPosition)
        {
            points[0] = targetPosition;
            lineRenderer.SetPositions(points);
            hand.transform.position = targetPosition;
            Debug.Log("targetPosition: " + targetPosition);
        }

        */

    void Start()
    {
        StartCoroutine(startSpawn());
        animator = GetComponent<Animator>();
    }

    IEnumerator startSpawn()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            print("can move: " + canMove);
            Spawn();
        }
    }


    void Spawn()
    {
        if (!canMove) { return; } // if canMove is false, then we hit something and we should not spawn a new arm
        StartCoroutine(moveArm());
    }

    IEnumerator moveArm()//Vector3 lastPoint, Vector3 nextPoint)
    {
        Vector3 playerPos = GameObject.Find("Player").transform.position;

        Vector3 lastPoint = hand.transform.position;
        Vector3 playerDirection = playerPos - transform.position;

        // get direction to player
        float angle = Mathf.Atan2(playerDirection.y, playerDirection.x) * Mathf.Rad2Deg;

        // find point 1f away in the direction of the player
        Vector3 playerPoint = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0) * 1f + transform.position;

        float direction = Mathf.Atan2(playerPoint.y - lastPoint.y, playerPoint.x - lastPoint.x) * Mathf.Rad2Deg;

        // if hitWall is true, then we should rotate the arm 180 degrees


        // get gurrent local rotation
        Vector3 currentRotation = transform.localEulerAngles;
        
        /*
        if (hitWall)
        {
            // rotate the arm 180 degrees
            currentRotation = new Vector3(currentRotation.x, currentRotation.y, currentRotation.z + 180);
            hitWall = false;
        }
        */ // not working as intended

        //Quaternion rotation = Quaternion.Euler(new Vector3(currentRotation.x, currentRotation.y, currentRotation.z));

        // get random rotation based on current rotation
        Vector3 randomRotation = new Vector3(currentRotation.x, currentRotation.y, currentRotation.z + Random.Range(-90, 90));

        //transform.rotation = Quaternion.FromToRotation (transform.up, hit.normal) * transform.rotation;

        // get randomRotation as quaternion
        Quaternion randomRotationQuaternion = Quaternion.Euler(randomRotation);

        // Lerp the rotation from the current rotation to the new rotation
        float t = 0f;
        while (t < 1f)
        {
            if (!canMove) { yield return null; }
            t += Time.deltaTime / 1f;

            if (Vector3.Distance(playerPos, this.hand.transform.position) < 30f)
            {
                // move hand
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, direction), t);
                transform.position = Vector3.Lerp(transform.position, transform.position + transform.right * MOVE_VALUE, t);
                //hand.GetComponent<Rigidbody2D>().AddForce(Vector3.Lerp(lastPoint, nextPoint, t) * 1f);
                yield return null;
            }
            else
            {

                transform.rotation = Quaternion.Lerp(transform.rotation, randomRotationQuaternion, t);
                // move forward using lerping
                transform.position = Vector3.Lerp(transform.position, transform.position + transform.right * MOVE_VALUE, t);
                //transform.position += transform.forward * Time.deltaTime * MOVE_VALUE;

                yield return null;
            }
        }


    }

    private int[] directions;

    void SetHand(string focus)
    {
        if (focus == "LEFT")
        {
            //transform.rotation = Quaternion.Euler(0, 0, -180);
            Debug.Log("LEFT");
        }
        else if (focus == "RIGHT")
        {
            directions = new int[] { 0, 1, 1, 2 };
        }
        else if (focus == "UP")
        {
            directions = new int[] { 0, 0, 1, 3 };
        }
        else if (focus == "DOWN")
        {
            directions = new int[] { 1, 2, 2, 3 };
        }
        else if (focus == "PLAYER")
        {
            directions = new int[] { 4 };
        }
    }

    private Vector3 getNextPoint(Vector3 oldPoint)
    {


        Vector3 spawnUp = new Vector3(oldPoint.x, oldPoint.y + MOVE_VALUE, 0);
        Vector3 spawnRight = new Vector3(oldPoint.x + MOVE_VALUE, oldPoint.y, 0);
        Vector3 spawnDown = new Vector3(oldPoint.x, oldPoint.y - MOVE_VALUE, 0);
        Vector3 spawnLeft = new Vector3(oldPoint.x - MOVE_VALUE, oldPoint.y, 0);



        // get player position
        Vector3 playerPos = GameObject.Find("Player").transform.position;

        Vector3 playerDirection = playerPos - transform.position;

        // get direction to player
        float angle = Mathf.Atan2(playerDirection.y, playerDirection.x) * Mathf.Rad2Deg;

        // find point 1f away in the direction of the player
        Vector3 playerPoint = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0) * 1f + transform.position;

        int direction = directions[Random.Range(0, directions.Length)];

        switch (direction)
        {
            case 0:
                /*
                if (!canSpawn(spawnUp)) { break; }
                */
                if (lastdirection == 2) { return oldPoint; }

                lastdirection = direction;
                return spawnUp;

            case 1:
                /*
                if (!canSpawn(spawnRight)) { break; }
                */
                lastdirection = direction;
                return spawnRight;
            case 2:
                /*
                if (!canSpawn(spawnDown)) { break; }
                */
                if (lastdirection == 0) { return oldPoint; }

                lastdirection = direction;
                return spawnDown;
            case 3:
                /*
                if (!canSpawn(spawnLeft)) { break; }
                */
                lastdirection = direction;
                return spawnLeft;
            case 4:
                return playerPoint;
        }
        return oldPoint;
    }


    private Collider2D[] colliders;

    private bool canSpawn2(Vector3 pos)
    {
        colliders = Physics2D.OverlapCircleAll(pos, 0.1f);
        if (colliders.Length > 0)
        {
            return false;
        }
        return true;
    }

    private bool canSpawn(Vector3 pos)
    {
        // colliders = Physics2D.OverlapCircleAll(transform.position, 100f);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(pos, 6f, ToyObjects);


        if (colliders.Length > 1)
        {
            if (colliders[0].GetType() == typeof(PolygonCollider2D))
            {
                Debug.Log("PolygonCollider2D");
            }
            return false;
        }
        return true;
    }

}
