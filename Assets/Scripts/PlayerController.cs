using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    public float moveSpeed = 30f;
    public float collisionOffset = 0.05f;
    public ContactFilter2D movementFilter;
    Vector2 movementInput;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rb;
    Animator animator;
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();

    public float distance = 0f;
    public Transform holdpoint;
    public LayerMask CollidableObjects;
    public LayerMask ToyObjects;

    private Transform currentlyGrabbedObject;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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
            else // Release currently grabbed object
            {
                currentlyGrabbedObject = null;
            }

        }

        if (currentlyGrabbedObject)
        {
            currentlyGrabbedObject.position = holdpoint.position + Vector3.right * 0.18f;
        }

         if(transform.position.y >= 40) {
            transform.position = new Vector3(transform.position.x, 40, 0);
        }
        else if (transform.position.y <= -40) {
            transform.position = new Vector3(transform.position.x, -40,0);
        }

        if (transform.position.x >= 65) {
            transform.position = new Vector3(65, transform.position.y, 0);
        }
        else if (transform.position.x <= -65) {
            transform.position = new Vector3(-65, transform.position.y,0);
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

            animator.SetBool("isMoving", true);
        } else{
            animator.SetBool("isMoving", false);
        }
        // Set direction of sprite to movement direction
        if (movementInput.x < 0) {
            spriteRenderer.flipX = true;
        } else if (movementInput.x > 0) {
            spriteRenderer.flipX = false;
        }
    }

    void OnMove(InputValue movementValue)
    {
        movementInput = movementValue.Get<Vector2>();
    }


}
