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

    //public float ARM_RADIUS = 1f;
    //private float wait = 0f; 

    public GameObject aPrefab, bPrefab, hand, player;

    public Sprite aBlockSprite, bBlockSprite, aBlockSpriteShine, bBlockSpriteShine;
    private Vector3 pos;

    [SerializeField] private float threshold = 2;
    private float _timeAccumulated;
    public float maxAmount = 10;
    private List<IToyObject> objects = new List<IToyObject>();

    public bool CanPlaceObj => _timeAccumulated > threshold && !Physics2D.OverlapCircle(pos, 0.5f) && transform.childCount < maxAmount;

    public delegate void BreakObject();
    public static event BreakObject onBreak;

    private Transform currentlyGrabbedObject;
    public LayerMask CollidableObjects;

    public Transform holdpoint;

    void Update()
    {
        for (int i = 0; i < objects.Count; i++)
        {
            makeShine(objects[i]);
        }
        if (CanPlaceObj)
        {
            SpawnObj(aPrefab, 3);
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
                Collider2D hit = Physics2D.OverlapCircle(pos, 2f, CollidableObjects);
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

        if (transform.position.y >= 25)
        {
            transform.position = new Vector3(transform.position.x, 25, 0);
        }
        else if (transform.position.y <= -25)
        {
            transform.position = new Vector3(transform.position.x, -25, 0);
        }

        if (transform.position.x >= 45)
        {
            transform.position = new Vector3(45, transform.position.y, 0);
        }
        else if (transform.position.x <= -45)
        {
            transform.position = new Vector3(-45, transform.position.y, 0);
        }
    }

// TODO: Add the rest of the objects here
    private void makeShine(IToyObject obj)
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
        for (int i = 0; i < objects.Count; i++)
        {
            if ((hand.transform.position - objects[i].gameObject.transform.position).sqrMagnitude < 3 * 3)
            {
                Debug.Log("RETT I NEBBET! ");
                if (objects[i].strength > 0)
                {
                    objects[i].strength--;
                }
                else
                {
                    Debug.Log("Destroy");
                    Destroy(objects[i].gameObject);
                    objects.RemoveAt(i);
                    onBreak();
                }
            }
        }
        onBreak(); // Bug because it tries to move into itself, so we call onbreak anyway

    }

    // Start is called before the first frame update
    void Start()
    {
        hand = GameObject.Find("hand");
        SpawnObj(aPrefab, 5);

    }


    private void SpawnObj(GameObject prefab, float strength)
    {
        Debug.Log("SpawnObj");
        pos = new Vector3(Random.Range(-45, 45), Random.Range(-25, 25), 0);
        GameObject obj = Instantiate(prefab, pos, Quaternion.identity);
        obj.transform.parent = transform;
        objects.Add(new ToyObject(obj, strength));
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
