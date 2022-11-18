using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmController : MonoBehaviour
{

    private LineRenderer lineRenderer;
    public LayerMask ToyObjects;
    private Vector3[] points;
    private GameObject Arm;
    public GameObject hand;
    float MOVE_VALUE = 2f;
    private int lastdirection = 1;

    /* Nyttige linker */
    //https://www.youtube.com/watch?v=2BH1yQXCpeU
    //https://github.com/llamacademy/line-renderer-collider/tree/main/Assets/Scripts

    public delegate void HitObject();
    public static event HitObject onHit;

    private bool canMove = true;


    void Update() { }

    void OnEnable()
    {
        GameController.onBreak += setCanMove;
        HandController.collision += collide;
    }

    void OnDisable()
    {
        GameController.onBreak += setCanMove;
        HandController.collision += collide;
    }

    void collide()
    {
        Debug.Log("Collision ARM");
        canMove = false;
        if (onHit != null)
        {
            StartCoroutine(Hit());
        }
    }

    IEnumerator Hit()
    {
        while (!canMove)
        {
            yield return new WaitForSeconds(1f);
            print("Hitting! " + canMove);
            if (onHit != null)
            {
                onHit();
            }
        }

    }


    private void setCanMove()
    {
        this.canMove = true;
        Debug.Log("Can move");
    }


    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        points = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(points);
        //Debug.Log(points.Length);
    }

    void Start()
    {
        StartCoroutine(startSpawn());
    }

    IEnumerator startSpawn()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            print("can move: " + canMove);
            Spawn();
        }
    }


    void Spawn()
    {
        if (!canMove) { StartCoroutine(Hit()); return; } // if canMove is false, then we hit something and we should not spawn a new arm
        Vector3[] newPoints = new Vector3[points.Length + 1];
        lineRenderer.GetPositions(newPoints);
        Vector3 lastPoint = newPoints[newPoints.Length - 2];

        Vector3 nextPoint = getNextPoint(newPoints[points.Length - 1]);
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


        newPoints[points.Length] = nextPoint;

        StartCoroutine(moveArm(newPoints, lastPoint, nextPoint));

    }

    IEnumerator moveArm(Vector3[] newPoints, Vector3 lastPoint, Vector3 nextPoint)
    {
        float direction = Mathf.Atan2(nextPoint.y - lastPoint.y, nextPoint.x - lastPoint.x) * Mathf.Rad2Deg;
        float time = 1f;
        float t = 0;

        lineRenderer.positionCount = newPoints.Length;
        this.points = newPoints;
        for (int i = 0; i < points.Length - 1; i++)
        {
            lineRenderer.SetPosition(i, points[i]);
        }

        while (t < 1)
        {
            if (!canMove) { break; }
            t += Time.deltaTime / time;

            // move hand
            hand.transform.position = Vector3.Lerp(lastPoint, nextPoint, t);
            //hand.GetComponent<Rigidbody2D>().AddForce(Vector3.Lerp(lastPoint, nextPoint, t) * 1f);
            hand.transform.rotation = Quaternion.Lerp(hand.transform.rotation, Quaternion.Euler(0, 0, direction), t);

            // move arm
            lineRenderer.SetPosition(points.Length - 1, Vector3.Lerp(lastPoint, nextPoint, t));

            yield return null;
        }
        lineRenderer.SetPosition(points.Length - 1, points[points.Length - 1]);
        GenerateMeshCollider();
    }



    private Vector3 getNextPoint(Vector3 oldPoint)
    {

        // TODO: Fikse alle retninger
        int[] upArray = { 0, 0, 1, 3 };
        int[] rightArray = { 0, 1, 1, 2 };
        int[] downArray = { 1, 2, 2, 3 };
        int[] leftArray = { 0, 2, 3, 3 };

        Vector3 spawnUp = new Vector3(oldPoint.x, oldPoint.y + MOVE_VALUE, 0);
        Vector3 spawnRight = new Vector3(oldPoint.x + MOVE_VALUE, oldPoint.y, 0);
        Vector3 spawnDown = new Vector3(oldPoint.x, oldPoint.y - MOVE_VALUE, 0);
        Vector3 spawnLeft = new Vector3(oldPoint.x - MOVE_VALUE, oldPoint.y, 0);

        int direction = rightArray[Random.Range(0, rightArray.Length)];

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
        }
        return oldPoint;
    }

    /* kok: https://github.com/llamacademy/line-renderer-collider/blob/main/Assets/Scripts/LineRendererSmoother.cs */
                public void GenerateMeshCollider()
                {

                    Mesh mesh = new Mesh();
                    lineRenderer.BakeMesh(mesh);

                    // Get triangles and vertices from mesh
                    int[] triangles = mesh.triangles;
                    Vector3[] vertices = mesh.vertices;

                    // Get just the outer edges from the mesh's triangles (ignore or remove any shared edges)
                    Dictionary<string, KeyValuePair<int, int>> edges = new Dictionary<string, KeyValuePair<int, int>>();
                    for (int i = 0; i < triangles.Length; i += 3)
                    {
                        for (int e = 0; e < 3; e++)
                        {
                            int vert1 = triangles[i + e];
                            int vert2 = triangles[i + e + 1 > i + 2 ? i : i + e + 1];
                            string edge = Mathf.Min(vert1, vert2) + ":" + Mathf.Max(vert1, vert2);
                            if (edges.ContainsKey(edge))
                            {
                                edges.Remove(edge);
                            }
                            else
                            {
                                edges.Add(edge, new KeyValuePair<int, int>(vert1, vert2));
                            }
                        }
                    }

                    // Create edge lookup (Key is first vertex, Value is second vertex, of each edge)
                    Dictionary<int, int> lookup = new Dictionary<int, int>();
                    foreach (KeyValuePair<int, int> edge in edges.Values)
                    {
                        if (lookup.ContainsKey(edge.Key) == false)
                        {
                            lookup.Add(edge.Key, edge.Value);
                        }
                    }

                    Destroy(gameObject.GetComponent<PolygonCollider2D>());

                    // Create empty polygon collider
                    PolygonCollider2D polygonCollider = gameObject.AddComponent<PolygonCollider2D>();
                    polygonCollider.pathCount = 0;

                    // Loop through edge vertices in order
                    int startVert = 0;
                    int nextVert = startVert;
                    int highestVert = startVert;
                    List<Vector2> colliderPath = new List<Vector2>();
                    while (true)
                    {

                        // Add vertex to collider path
                        colliderPath.Add(vertices[nextVert]);

                        // Get next vertex
                        nextVert = lookup[nextVert];

                        // Store highest vertex (to know what shape to move to next)
                        if (nextVert > highestVert)
                        {
                            highestVert = nextVert;
                        }

                        // Shape complete
                        if (nextVert == startVert)
                        {

                            // Add path to polygon collider
                            polygonCollider.pathCount++;
                            polygonCollider.SetPath(polygonCollider.pathCount - 1, colliderPath.ToArray());
                            colliderPath.Clear();

                            // Go to next shape if one exists
                            if (lookup.ContainsKey(highestVert + 1))
                            {

                                // Set starting and next vertices
                                startVert = highestVert + 1;
                                nextVert = startVert;

                                // Continue to next loop
                                continue;
                            }

                            // No more verts
                            break;
                        }
                    }
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
        Debug.Log("Can spawn");
        return true;
    }

    private RaycastHit2D[] hits;
    private bool canSpawn3(Vector3 pos)
    {
        hits = Physics2D.RaycastAll(pos, Vector2.zero, 10f);
        if (hits.Length > 0)
        {
            return false;
        }
        return true;
    }

}
