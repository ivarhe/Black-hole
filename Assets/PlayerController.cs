using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    public float moveSpeed = 10f;
    public float collisionOffset = 0.05f;
    public ContactFilter2D movementFilter;
    Vector2 movementInput;
    Rigidbody2D rb;
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();

    public float distance = 0f;
    public Transform holdpoint;
    public LayerMask CollidableObjects;

    private Transform currentlyGrabbedObject;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("spacebar_down: ");
            Vector2 pos = GameObject.Find("Player").transform.position;
            Debug.Log(pos);
            if (!currentlyGrabbedObject)
            {
                Collider2D hit = Physics2D.OverlapCircle(pos, 1.5f, CollidableObjects);
                Debug.Log("heihei: " + hit);
                if (hit)
                {
                    if (hit.GetType() == typeof(PolygonCollider2D)) { return; }
                    currentlyGrabbedObject = hit.transform;
                    Debug.Log("Hit! ");
                }
            }
            else // release currently grabbed object
            {
                currentlyGrabbedObject = null;
            }

        }

        if (currentlyGrabbedObject)
        {
            currentlyGrabbedObject.position = holdpoint.position + Vector3.right * 0.18f;
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
