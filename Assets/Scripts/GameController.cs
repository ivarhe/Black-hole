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

interface IHandObject
{
    GameObject hand { get; set; }
    string focus { get; set; }
}

class HandObject : IHandObject
{
    public HandObject(GameObject hand, string focus)
    {
        this.hand = hand;
        this.focus = focus;
    }
    public GameObject hand { get; set; }
    public string focus { get; set; }
}

public class GameController : MonoBehaviour
{

    public GameObject aBlockPrefab, bBlockPrefab, ballPrefab, carPrefab, boxPrefab, handPrefab, armPrefab, player;

    public Sprite aBlockSprite, bBlockSprite, ballSprite, carSprite, boxSprite;
    public Sprite aBlockSpriteShine, bBlockSpriteShine, ballSpriteShine, carSpriteShine, boxSpriteShine;

    public int ShineDistance;

    private IToyObject[] spawnableObjects = new IToyObject[5];
    private List<IToyObject> spawnedObjects = new List<IToyObject>();

    private List<IHandObject> hands = new List<IHandObject>();
    private Vector3 pos;

    private Vector3[] startPos = new Vector3[14];

    [SerializeField] private float threshold = 2;
    private float _timeAccumulated;
    public float maxAmount = 10;

    public bool CanPlaceObj => _timeAccumulated > threshold && !Physics2D.OverlapCircle(pos, 4f) && transform.childCount < maxAmount;

    private Transform currentlyGrabbedObject;
    public LayerMask CollidableObjects;
    public LayerMask ToyObjects;

    public Transform holdpoint;

    public delegate void StartMoving();
    public static event StartMoving move;

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

    }

    // TODO: Add the rest of the objects here
    void makeShine(IToyObject obj)
    {
        if ((player.transform.position - obj.gameObject.transform.position).sqrMagnitude < 3 * ShineDistance) // check if the player is close to the object
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
        ArmController.StartPush += StartPush;
        ArmController.StopPush += StopPush;
    }


    void OnDisable()
    {
        ArmController.StartPush -= StartPush;
        ArmController.StopPush -= StopPush;
    }

    void StartPush(GameObject hand)
    {
        ActivateSlap(hand);
        //WaitToMove(hand);
    }

    void StopPush(GameObject hand)
    {
        PushAndMove(hand);
    }

    // Maybe remove parameter here!
    void PushAndMove(GameObject hand)
    {

        GameObject closest = getClosestToy(hand);

        if (move != null)
        {
            closest.GetComponent<Rigidbody2D>().AddForce((closest.transform.position - hand.transform.position) * 5, ForceMode2D.Impulse);
            closest.GetComponent<Rigidbody2D>().drag = 1f;
            move();
            DeactivateSlap(hand);
        }
    }

    // get closest toy to the hand using spawwnedObjects list
    public GameObject getClosestToy(GameObject hand)
    {
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = hand.transform.position;
        foreach (IToyObject toy in spawnedObjects)
        {
            Vector3 diff = toy.gameObject.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = toy.gameObject;
                distance = curDistance;
            }
        }
        return closest;
    }

    // spawn a new hand every minute
    void SpawnHand()
    {
        Vector3 startPos = findHandPos();
        GameObject tmp = Instantiate(handPrefab, startPos, Quaternion.identity);
        hands.Add(new HandObject(tmp, "RIGHT"));

        // rotate tmp object based on the start position
        if (startPos.x < 0)
        {
            tmp.transform.Rotate(0, 0, 180);
        }
        if (startPos.y < 0)
        {
            tmp.transform.Rotate(0, 0, 60);
        }
        if (startPos.y > 0)
        {
            tmp.transform.Rotate(0, 0, -60);
        }

    }

    // find a starting position for the hand from the list of starting positions and check if it is free
    Vector3 findHandPos()
    {
        int index = Random.Range(0, 14);
        Vector3 pos = startPos[index];
        bool freeSpawnLocation = false;
        for (int i = 0; i < 14; i++)
        {
            if (startPos[i] != new Vector3(0, 0, 0))
            {
                freeSpawnLocation = true;
                continue;
            }
        }
        if (!freeSpawnLocation)
        {
            return new Vector3(0, 0, 0);
        } 
        while (pos == new Vector3(0, 0, 0))
        {
            index = Random.Range(0, 14);
            pos = startPos[index];
        }
        startPos[index] = new Vector3(0, 0, 0);
        return pos;
    } 


    // Start is called before the first frame update
    List<Animator> animatorList = new List<Animator>();
    void Start()
    {
        spawnableObjects[0] = new ToyObject(aBlockPrefab, 5);
        spawnableObjects[1] = new ToyObject(bBlockPrefab, 8);
        spawnableObjects[2] = new ToyObject(ballPrefab, 3);
        spawnableObjects[3] = new ToyObject(carPrefab, 10);
        //spawnableObjects[4] = new ToyObject(boxPrefab, 15);
        SpawnObj();

        startPos[0] = new Vector3(-5.5f, 0f, 0);
        startPos[1] = new Vector3(-5.5f, 4f, 0);
        startPos[2] = new Vector3(-5.5f, 8f, 0);
        startPos[3] = new Vector3(5.5f, 8f, 0);
        startPos[4] = new Vector3(5.5f, 4f, 0);
        startPos[5] = new Vector3(5.5f, 0f, 0);
        startPos[6] = new Vector3(5.5f, -4f, 0);
        startPos[7] = new Vector3(5.5f, -8f, 0);
        startPos[8] = new Vector3(-5.5f, -8f, 0);
        startPos[9] = new Vector3(-5.5f, -4f, 0);
        startPos[10] = new Vector3(-2.5f, -9.5f, 0);
        startPos[11] = new Vector3(2.5f, -9.5f, 0);
        startPos[12] = new Vector3(-2.5f, 9.5f, 0);
        startPos[13] = new Vector3(2.5f, 9.5f, 0);

        //GameObject arm = Instantiate(armPrefab, new Vector3(), Quaternion.identity);

        GameObject tmp = Instantiate(handPrefab, startPos[5], Quaternion.identity);
        hands.Add(new HandObject(tmp, "RIGHT"));
        //tmp.SendMessage("SetHand", "RIGHT");
        startPos[5] = new Vector3(0, 0, 0);

        GameObject tmp2 = Instantiate(handPrefab, startPos[0], Quaternion.identity);
        tmp2.transform.Rotate(0, 0, 180);
        hands.Add(new HandObject(tmp2, "LEFT"));
        //tmp2.SendMessage("SetHand", "LEFT");
        startPos[0] = new Vector3(0, 0, 0);


        foreach (HandObject hand in hands)
        {
            animatorList.Add(hand.hand.GetComponent<Animator>()); //fill up your list with animators components from valve gameobjects
        }

        InvokeRepeating("SpawnHand", 0, 60);

    }

    void ActivateSlap(GameObject hand)
    {
        foreach (Animator animator in animatorList)
        {
            if (animator.gameObject == hand)
            {
                animator.Play("hand_slap");
            }
        }
    }

    void DeactivateSlap(GameObject hand)
    {
        foreach (Animator animator in animatorList)
        {
            if (animator.gameObject == hand)
            {
                animator.Play("hand_idle");
            }
        }
    }

    private void SpawnObj()
    {
        // find new position for next object
        pos = new Vector3();
        while (Physics2D.OverlapCircle(pos, 4f) || !isOutsideOfBed(pos))
        {
            if (spawnedObjects.Count == maxAmount)
            {
                return;
            }
            pos = new Vector3(Random.Range(-65, 65), Random.Range(-35, 35), 0);
            Debug.Log("new pos" + pos);
        }
        int whichItem = Random.Range(0, spawnableObjects.Length - 2); // change to -1 when box if fixed
        GameObject obj = Instantiate(spawnableObjects[whichItem].gameObject, pos, Quaternion.identity);
        obj.transform.parent = transform;
        spawnedObjects.Add(new ToyObject(obj, spawnableObjects[whichItem].strength));

    }

    private bool isOutsideOfBed(Vector3 pos)
    {
        return pos.x < -10 || pos.x > 10 || pos.y < -15 || pos.y > 15;
    }


}
