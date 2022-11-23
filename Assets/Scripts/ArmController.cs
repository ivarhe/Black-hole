using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmController : MonoBehaviour
{

    //public LineRenderer lineRenderer;
    public LayerMask ToyObjects;
    private List<Vector3> points;
    //private GameObject Arm;
    public GameObject hand;
    float MOVE_VALUE = 2f;
    private int lastdirection = 1;

    /* Nyttige linker */
    //https://www.youtube.com/watch?v=2BH1yQXCpeU
    //https://github.com/llamacademy/line-renderer-collider/tree/main/Assets/Scripts

    public delegate void PushObject(GameObject toy);
    public static event PushObject Push;

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
        this.canMove = false;
        animator.SetTrigger("Slap");
        Push(hand);
    }

    void Move() { 
        animator.ResetTrigger("Slap");
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
            yield return new WaitForSeconds(1f);
            print("can move: " + canMove);
            if (points == null)
            {
                points = new List<Vector3>();
                points.Add(transform.position);
            }
            Spawn();
        }
    }


    void Spawn()
    {
        if (!canMove) { return; } // if canMove is false, then we hit something and we should not spawn a new arm
        //Vector3[] newPoints = new Vector3[points.Length + 1];
        //lineRenderer.GetPositions(newPoints);
        Vector3 lastPoint = points.Last();

        Vector3 nextPoint = getNextPoint(lastPoint);
        if (nextPoint == new Vector3(100, 100, 100)) { return; } // if we can't find a new point, then we should not spawn a new arm

        if (nextPoint == new Vector3()) // Hit an object
        {
            /*
            if (onHit != null)
            {
                canMove = false;
                StartCoroutine(Hit());
            }
            */
            return;
        }
        points.Add(nextPoint);

        StartCoroutine(moveArm(lastPoint, nextPoint));

    }

    IEnumerator moveArm(Vector3 lastPoint, Vector3 nextPoint)
    {
        float direction = Mathf.Atan2(nextPoint.y - lastPoint.y, nextPoint.x - lastPoint.x) * Mathf.Rad2Deg;
        float time = 1f;
        float t = 0;


        while (t < 1)
        {
            //if (!canMove) { break; }
            while (!canMove) { yield return null; }
            t += Time.deltaTime / time;

            // move hand
            hand.transform.position = Vector3.Lerp(lastPoint, nextPoint, t);
            //hand.GetComponent<Rigidbody2D>().AddForce(Vector3.Lerp(lastPoint, nextPoint, t) * 1f);
            hand.transform.rotation = Quaternion.Lerp(hand.transform.rotation, Quaternion.Euler(0, 0, direction), t);

            yield return null;
        }
    }
    private int[] directions;

    void SetHand(string focus)
    {
        if (focus == "LEFT")
        {
            directions = new int[] { 0, 2, 3, 3 };
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
