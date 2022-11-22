using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

    public float moveSpeed = 30f;
    public float collisionOffset = 0.05f;
    public float HOLD_OFFSET = 4f;
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

    public List<GameObject> hearts;
    private bool isLeft = false;

    public delegate void PlayerDamageEvent();
    public static event PlayerDamageEvent onPlayerDamage;




    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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
                Collider2D hit = Physics2D.OverlapCircle(pos, 6f, ToyObjects);
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
            Debug.Log("grabbed: " + currentlyGrabbedObject.name);
            if (isLeft)
            {
                currentlyGrabbedObject.position = new Vector2(transform.position.x - HOLD_OFFSET, transform.position.y);
            }
            else
            {
                currentlyGrabbedObject.position = new Vector2(transform.position.x + HOLD_OFFSET, transform.position.y);
            }
            //currentlyGrabbedObject.position = holdpoint.position + Vector3.right * HOLD_OFFSET;

        }

    }

    void MoveTowards(Vector3 pos)
    {
        rb.position = Vector3.Lerp(transform.position, pos, moveSpeed * Time.deltaTime);
    }
    bool isBouncing;

    private void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log("collide with player " + col.gameObject.name);
        if (col.gameObject.tag == "arm" || col.gameObject.tag == "hand")
        {
            // get direction of collision
            isBouncing = true;
            Invoke("StopBounce", 0.3f);

            Debug.Log("collide with arm 3");
            onPlayerDamage();
            if (hearts.Count > 1)
            {
                GameObject heart = hearts[hearts.Count - 1];
                hearts.Remove(heart);
                Destroy(heart);
            }
            else
            {
                GameObject heart = hearts[hearts.Count - 1];
                Destroy(heart);
            }
            //rb.MovePosition(rb.position - movementInput * moveSpeed * 20 * Time.fixedDeltaTime);
        }
    }

    void StopBounce()
    {
        isBouncing = false;
    }

    private bool canSpawn(Vector3 pos)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(pos, 2f);
        if (colliders.Length > 0)
        {
            return false;
        }
        return true;
    }


    private void FixedUpdate()
    {
        if (movementInput != Vector2.zero)
        {
            int count = 0; /*rb.Cast(
                movementInput,
                movementFilter,
                castCollisions,
                moveSpeed * Time.fixedDeltaTime// + collisionOffset
            );*/

            if (count == 0)
            {
                if (!isBouncing)
                {
                    rb.MovePosition(rb.position + movementInput * moveSpeed * Time.fixedDeltaTime);
                }
                else
                {
                    // find a safe spot to move to
                    Vector3 pos = transform.position;
                    Vector3 newPos = pos - (Vector3)movementInput * moveSpeed * Time.fixedDeltaTime;
                    for (int i = 0; i < 10; i++)
                    {
                        //newPos = pos - (Vector3)movementInput * i;
                        Debug.Log("newPos: " + newPos);
                        if (canSpawn(newPos))
                        {
                            break;
                        }
                    }
                    rb.MovePosition(newPos);   

                }
            }

            animator.SetBool("isMoving", true);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }
        // Set direction of sprite to movement direction
        if (movementInput.x < 0)
        {
            isLeft = true;
            spriteRenderer.flipX = true;
        }
        else if (movementInput.x > 0)
        {
            isLeft = false;
            spriteRenderer.flipX = false;
        }
    }

    void OnMove(InputValue movementValue)
    {
        movementInput = movementValue.Get<Vector2>();
    }


}
