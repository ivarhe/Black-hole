using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IToyObject
{
    GameObject gameObject { get; set; }
    float strength { get; set; }
}

class ToyObject : IToyObject
{
    public ToyObject(GameObject gameObject, float strength)
    {
        this.gameObject = gameObject;
        this.strength = strength;
    }
    public GameObject gameObject { get; set; }
    public float strength { get; set; }
}

public class GameController : MonoBehaviour
{

    public GameObject aBlockPrefab, bBlockPrefab, ballPrefab, carPrefab, boxPrefab, hand, player;

    public Sprite aBlockSprite, bBlockSprite, ballSprite, carSprite, boxSprite;
    public Sprite aBlockSpriteShine, bBlockSpriteShine, ballSpriteShine, carSpriteShine, boxSpriteShine;

    private IToyObject[] spawnableObjects = new IToyObject[5];
    private List<IToyObject> spawnedObjects = new List<IToyObject>();
    private Vector3 pos;

    [SerializeField] private float threshold = 2;
    private float _timeAccumulated;
    public float maxAmount = 10;

    public bool CanPlaceObj => _timeAccumulated > threshold && !Physics2D.OverlapCircle(pos, 0.5f) && transform.childCount < maxAmount;

    public delegate void BreakObject();
    public static event BreakObject onBreak;

    private Transform currentlyGrabbedObject;
    public LayerMask CollidableObjects;
    public LayerMask ToyObjects;

    public Transform holdpoint;

    void Update()
    {
        for (int i = 0; i < spawnedObjects.Count; i++)
        {
            makeShine(spawnedObjects[i]);
        }
        if (CanPlaceObj)
        {
            SpawnObj();
            _timeAccumulated = 0;
            return;
        }
        _timeAccumulated += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("spacebar_down: ");
            Vector2 pos = GameObject.Find("Player").transform.position;
            Debug.Log(pos);
            if (!currentlyGrabbedObject)
            {
                Collider2D hit = Physics2D.OverlapCircle(pos, 2f, ToyObjects);
                Debug.Log("heihei: " + hit);
                if (hit)
                {
                    if (hit.GetType() == typeof(PolygonCollider2D)) { return; }
                    currentlyGrabbedObject = hit.transform;
                    /* If we want to change the sprite when grabbed */
                    //hit.GetComponent<SpriteRenderer>().sprite = bBoxDarkSprite;
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

// TODO: Add the rest of the objects here
    void makeShine(IToyObject obj)
    {
        if ((player.transform.position - obj.gameObject.transform.position).sqrMagnitude < 3 * 20) // check if the player is close to the object
        {
            if (obj.gameObject.CompareTag("a-block"))
            {
                obj.gameObject.GetComponent<SpriteRenderer>().sprite = aBlockSpriteShine;
                return;
            }
            if (obj.gameObject.CompareTag("b-block"))
            {
                obj.gameObject.GetComponent<SpriteRenderer>().sprite = bBlockSpriteShine;
                return;
            }
            if (obj.gameObject.CompareTag("ball"))
            {
                obj.gameObject.GetComponent<SpriteRenderer>().sprite = ballSpriteShine;
                return;
            }
            if (obj.gameObject.CompareTag("car"))
            {
                obj.gameObject.GetComponent<SpriteRenderer>().sprite = carSpriteShine;
                return;
            }
            if (obj.gameObject.CompareTag("box"))
            {
                obj.gameObject.GetComponent<SpriteRenderer>().sprite = boxSpriteShine;
                return;
            }
        }
        else
        {
            if (obj.gameObject.CompareTag("a-block"))
            {
                obj.gameObject.GetComponent<SpriteRenderer>().sprite = aBlockSprite;
                return;
            }
            if (obj.gameObject.CompareTag("b-block"))
            {
                obj.gameObject.GetComponent<SpriteRenderer>().sprite = bBlockSprite;
                return;
            }
            if (obj.gameObject.CompareTag("ball"))
            {
                obj.gameObject.GetComponent<SpriteRenderer>().sprite = ballSprite;
                return;
            }
            if (obj.gameObject.CompareTag("car"))
            {
                obj.gameObject.GetComponent<SpriteRenderer>().sprite = carSprite;
                return;
            }
            if (obj.gameObject.CompareTag("box"))
            {
                obj.gameObject.GetComponent<SpriteRenderer>().sprite = boxSprite;
                return;
            }
        }
    }

    void OnEnable()
    {
        ArmController.onHit += OnHit;
    }


    void OnDisable()
    {
        ArmController.onHit -= OnHit;
    }

    void OnHit()
    {
        GameObject closestToy = GetClosestToy();
        closestToy.GetComponent<Rigidbody2D>().AddForce((closestToy.transform.position - hand.transform.position) * 50, ForceMode2D.Impulse);
        closestToy.GetComponent<Rigidbody2D>().drag = 1;
        onBreak();
    }

    GameObject GetClosestToy()
    {
        GameObject bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = hand.transform.position;
        for (int i = 0; i < spawnedObjects.Count; i++) {
            Vector3 directionToTarget = spawnedObjects[i].gameObject.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr) {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = spawnedObjects[i].gameObject;
            }
        }
        return bestTarget;
    }

    // Start is called before the first frame update
    void Start()
    {
        spawnableObjects[0] = new ToyObject(aBlockPrefab, 5);
        spawnableObjects[1] = new ToyObject(bBlockPrefab, 8);
        spawnableObjects[2] = new ToyObject(ballPrefab, 3);
        spawnableObjects[3] = new ToyObject(carPrefab, 10);
        //spawnableObjects[4] = new ToyObject(boxPrefab, 15);
        hand = GameObject.Find("hand");
        SpawnObj();

    }


    private void SpawnObj()
    {
        int whichItem = Random.Range (0, spawnableObjects.Length - 2); // change to -1 when box if fixed
        Debug.Log("SpawnObj");
        pos = new Vector3(Random.Range(-45, 45), Random.Range(-25, 25), 0);
        GameObject obj = Instantiate(spawnableObjects[whichItem].gameObject, pos, Quaternion.identity);
        obj.transform.parent = transform;
        spawnedObjects.Add(new ToyObject(obj, spawnableObjects[whichItem].strength));
    }



    // need for later

    /*

    private void Awake () {
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Use this for initialization
    void Start()
    {
        //screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        startPositions.Add(new Vector3(0, 4f, 0));
        startPositions.Add(new Vector3(4f, 0, 0));
        startPositions.Add(new Vector3(0, -4f, 0));
        startPositions.Add(new Vector3(-4f, 0, 0));
        StartCoroutine(spawnWave());
    }


    IEnumerator spawnWave()
    {
        while (true)
        {
            yield return new WaitForSeconds(wait);
            Spawn();
        }
    }

    private void Spawn()
    {

        bool canSpawnHere = false;
        int safetyNet = 0;

        while (!canSpawnHere)
        {
            int index = Random.Range(0, startPositions.Count);
            Vector3 currentHead = startPositions[index];

            int[] upArray = { 0, 0, 1, 3 };
            int[] rightArray = { 0, 1, 1, 2 };
            int[] downArray = { 1, 2, 2, 3 };
            int[] leftArray = { 0, 2, 3, 3 };

            Vector3 spawnUp = new Vector3(currentHead.x, currentHead.y, 0);
            Vector3 spawnRight = new Vector3(currentHead.x, currentHead.y, 0);
            Vector3 spawnDown = new Vector3(currentHead.x, currentHead.y, 0);
            Vector3 spawnLeft = new Vector3(currentHead.x, currentHead.y, 0);

            int direction = 0;

            switch (index)
            {
                case 0:
                    direction = upArray[Random.Range(0, upArray.Length)];

                    switch(direction) {
                        case 0:
                            canSpawnHere = canSpawn(spawnUp);
                            if (!canSpawnHere) { break; }
                            GameObject arm0 = Instantiate(armPrefab, spawnUp, Quaternion.identity) as GameObject;
                            startPositions[index] = new Vector3(spawnUp.x, spawnUp.y, 0);
                            break;
                        case 1:
                            canSpawnHere = canSpawn(spawnRight);
                            if (!canSpawnHere) { break; }
                            GameObject arm1 = Instantiate(armPrefab, spawnRight, Quaternion.identity) as GameObject;
                            startPositions[index] = new Vector3(spawnRight.x, spawnRight.y, 0);
                            break;
                        case 3:
                            canSpawnHere = canSpawn(spawnLeft);
                            if (!canSpawnHere) { break; }
                            GameObject arm3 = Instantiate(armPrefab, spawnLeft, Quaternion.identity) as GameObject;
                            startPositions[index] = new Vector3(spawnLeft.x, spawnLeft.y, 0);
                            break;
                        }
                    break;

                case 1:
                    direction = rightArray[Random.Range(0, rightArray.Length)];

                    switch(direction) {
                        case 0:
                            canSpawnHere = canSpawn(spawnUp);
                            if (!canSpawnHere) { break; }
                            GameObject arm0 = Instantiate(armPrefab, spawnUp, Quaternion.identity) as GameObject;
                            startPositions[index] = new Vector3(spawnUp.x, spawnUp.y, 0);
                            break;
                        case 1:
                            canSpawnHere = canSpawn(spawnRight);
                            if (!canSpawnHere) { break; }
                            GameObject arm1 = Instantiate(armPrefab, spawnRight, Quaternion.identity) as GameObject;
                            startPositions[index] = new Vector3(spawnRight.x, spawnRight.y, 0);
                            break;
                        case 2:
                            canSpawnHere = canSpawn(spawnDown);
                            if (!canSpawnHere) { break; }
                            GameObject arm2 = Instantiate(armPrefab, spawnDown, Quaternion.identity) as GameObject;
                            startPositions[index] = new Vector3(spawnDown.x, spawnDown.y, 0);
                            break;
                        }

                    break;
                case 2:
                    direction = downArray[Random.Range(0, downArray.Length)];

                    switch(direction) {
                        case 1:
                            canSpawnHere = canSpawn(spawnRight);
                            if (!canSpawnHere) { break; }
                            GameObject arm1 = Instantiate(armPrefab, spawnRight, Quaternion.identity) as GameObject;
                            startPositions[index] = new Vector3(spawnRight.x, spawnRight.y, 0);
                            break;
                        case 2:
                            canSpawnHere = canSpawn(spawnDown);
                            if (!canSpawnHere) { break; }
                            GameObject arm2 = Instantiate(armPrefab, spawnDown, Quaternion.identity) as GameObject;
                            startPositions[index] = new Vector3(spawnDown.x, spawnDown.y, 0);
                            break;
                        case 3:
                            canSpawnHere = canSpawn(spawnLeft);
                            if (!canSpawnHere) { break; }
                            GameObject arm3 = Instantiate(armPrefab, spawnLeft, Quaternion.identity) as GameObject;
                            startPositions[index] = new Vector3(spawnLeft.x, spawnLeft.y, 0);
                            break;
                        }
                    break;
                case 3:
                    direction = leftArray[Random.Range(0, leftArray.Length)];
                    switch(direction) {
                        case 0:
                            canSpawnHere = canSpawn(spawnUp);
                            if (!canSpawnHere) { break; }
                            GameObject arm0 = Instantiate(armPrefab, spawnUp, Quaternion.identity) as GameObject;
                            startPositions[index] = new Vector3(spawnUp.x, spawnUp.y, 0);
                            break;
                        case 2:
                            canSpawnHere = canSpawn(spawnDown);
                            if (!canSpawnHere) { break; }
                            GameObject arm2 = Instantiate(armPrefab, spawnDown, Quaternion.identity) as GameObject;
                            startPositions[index] = new Vector3(spawnDown.x, spawnDown.y, 0);
                            break;
                        case 3:
                            canSpawnHere = canSpawn(spawnLeft);
                            if (!canSpawnHere) { break; }
                            GameObject arm3 = Instantiate(armPrefab, spawnLeft, Quaternion.identity) as GameObject;
                            startPositions[index] = new Vector3(spawnLeft.x, spawnLeft.y, 0);
                            break;
                        }
                    break;

            }

            safetyNet++;

            if (safetyNet > 50)
            {
                Debug.Log("Too many attempts");
                return;
            }

        }

    }


    private bool canSpawn(Vector3 pos)
    {
        colliders = Physics2D.OverlapCircleAll(transform.position, radius);

        for (int i = 0; i < colliders.Length; i++)
        {
            Vector3 centerpoint = colliders[i].bounds.center;
            float width = colliders[i].bounds.extents.x;
            float height = colliders[i].bounds.extents.y;

            float leftExtent = centerpoint.x - width - ARM_RADIUS;
            float rightExtent = centerpoint.x + width + ARM_RADIUS;
            float lowerExtent = centerpoint.y - height - ARM_RADIUS;
            float upperExtent = centerpoint.y + height + ARM_RADIUS;

            if (pos.x >= leftExtent && pos.x <= rightExtent)
            {
                if (pos.y >= lowerExtent && pos.y <= upperExtent)
                {
                    return false;
                }
            }
        }
        return true;
    }

    */
}
