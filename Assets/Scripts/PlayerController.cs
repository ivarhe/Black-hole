using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

    public float moveSpeed = 30f;
    public float collisionOffset = 0.05f;
    public ContactFilter2D movementFilter;
    Vector2 movementInput;
    Rigidbody2D rb;
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();

    public float distance = 0f;
    public Transform holdpoint;
    public LayerMask CollidableObjects;
    public LayerMask ToyObjects;

    private Transform currentlyGrabbedObject;

    public List<GameObject> hearts;




    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        ToyController.collision += ToyCollide;
    }

    void OnDisable()
    {
        ToyController.collision += ToyCollide;
    }


    void ToyCollide(GameObject toy)
    {
        Debug.Log("Toy: " + toy);
        if (currentlyGrabbedObject == toy.transform)
        {
            currentlyGrabbedObject = null;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("spacebar_down: ");
            Vector2 pos = GameObject.Find("Player").transform.position;
            if (!currentlyGrabbedObject)
            {
                Collider2D hit = Physics2D.OverlapCircle(pos, 2f, ToyObjects);
                if (hit)
                {
                    if (hit.GetType() == typeof(PolygonCollider2D)) { return; }
                    currentlyGrabbedObject = hit.transform;
                }
            }
            else // release currently grabbed object
            {
                currentlyGrabbedObject = null;
            }

        }

        if (currentlyGrabbedObject)
        {
            Debug.Log("grabbed: " + currentlyGrabbedObject.name);
            currentlyGrabbedObject.position = holdpoint.position + Vector3.right * 0.18f;

        }

    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log("collide with player " + col.gameObject.name);
        if (col.gameObject.tag == "arm" || col.gameObject.tag == "hand")
        {
            // get the direction of the collision
            Vector2 direction = col.contacts[0].point - (Vector2)transform.position;
            direction = -direction.normalized;
            // move the player away from the collision
            transform.position = (Vector2)transform.position + direction * collisionOffset * 100;

            if (hearts.Count > 0)
            {
                GameObject heart = hearts[hearts.Count - 1];
                hearts.Remove(heart);
                Destroy(heart);
            }
            else
            {
                SceneManager.LoadScene("GameOver");
            }
            rb.MovePosition(rb.position - movementInput * moveSpeed * 20 * Time.fixedDeltaTime);
        }
    }



    private void FixedUpdate()
    {
        if (movementInput != Vector2.zero)
        {
            int count = rb.Cast(
                movementInput,
                movementFilter,
                castCollisions,
                moveSpeed * Time.fixedDeltaTime + collisionOffset
            );

            if (count == 0)
            {
                rb.MovePosition(rb.position + movementInput * moveSpeed * Time.fixedDeltaTime);
            }
        }
    }

    void OnMove(InputValue movementValue)
    {
        movementInput = movementValue.Get<Vector2>();
    }


}
