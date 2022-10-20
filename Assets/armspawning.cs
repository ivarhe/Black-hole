using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class armspawning : MonoBehaviour
{
    public GameObject armPrefab;
    public float respawnTime = 0f;

    private Collider2D[] colliders;
    private float radius = 100f;

    //private float x = 0.4f;
    //private float y = 0;

    public List<Vector3> listOfPosition = new List<Vector3>();

    //private Vector2 screenBounds;

    public float ARM_RADIUS = 0.049f;
    private float MOVE_OFFSET = 0.1f;

    // Use this for initialization
    void Start()
    {
        //screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        listOfPosition.Add(new Vector3(0, 0.4f, 0));
        listOfPosition.Add(new Vector3(0.4f, 0, 0));
        listOfPosition.Add(new Vector3(0, -0.4f, 0));
        listOfPosition.Add(new Vector3(-0.4f, 0, 0));
        StartCoroutine(spawnWave());
    }


    IEnumerator spawnWave()
    {
        while (true)
        {
            yield return new WaitForSeconds(respawnTime);
            Spawn();
        }
    }

    private void Spawn()
    {

        bool canSpawnHere = false;
        int safetyNet = 0;

        while (!canSpawnHere)
        {
            int index = Random.Range(0, listOfPosition.Count);
            Vector3 currentHead = listOfPosition[index];

            int[] upArray = { 0, 0, 1, 3 };
            int[] rightArray = { 0, 1, 1, 2 };
            int[] downArray = { 1, 2, 2, 3 };
            int[] leftArray = { 0, 2, 3, 3 };

            Vector3 spawnUp = new Vector3(currentHead.x, currentHead.y + MOVE_OFFSET, 0);
            Vector3 spawnRight = new Vector3(currentHead.x + MOVE_OFFSET, currentHead.y, 0);
            Vector3 spawnDown = new Vector3(currentHead.x, currentHead.y - MOVE_OFFSET, 0);
            Vector3 spawnLeft = new Vector3(currentHead.x - MOVE_OFFSET, currentHead.y, 0);

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
                            listOfPosition[index] = new Vector3(spawnUp.x, spawnUp.y, 0);
                            break;
                        case 1:
                            canSpawnHere = canSpawn(spawnRight);
                            if (!canSpawnHere) { break; }
                            GameObject arm1 = Instantiate(armPrefab, spawnRight, Quaternion.identity) as GameObject;
                            listOfPosition[index] = new Vector3(spawnRight.x, spawnRight.y, 0);
                            break;
                        case 3:
                            canSpawnHere = canSpawn(spawnLeft);
                            if (!canSpawnHere) { break; }
                            GameObject arm3 = Instantiate(armPrefab, spawnLeft, Quaternion.identity) as GameObject;
                            listOfPosition[index] = new Vector3(spawnLeft.x, spawnLeft.y, 0);
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
                            listOfPosition[index] = new Vector3(spawnUp.x, spawnUp.y, 0);
                            break;
                        case 1:
                            canSpawnHere = canSpawn(spawnRight);
                            if (!canSpawnHere) { break; }
                            GameObject arm1 = Instantiate(armPrefab, spawnRight, Quaternion.identity) as GameObject;
                            listOfPosition[index] = new Vector3(spawnRight.x, spawnRight.y, 0);
                            break;
                        case 2:
                            canSpawnHere = canSpawn(spawnDown);
                            if (!canSpawnHere) { break; }
                            GameObject arm2 = Instantiate(armPrefab, spawnDown, Quaternion.identity) as GameObject;
                            listOfPosition[index] = new Vector3(spawnDown.x, spawnDown.y, 0);
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
                            listOfPosition[index] = new Vector3(spawnRight.x, spawnRight.y, 0);
                            break;
                        case 2:
                            canSpawnHere = canSpawn(spawnDown);
                            if (!canSpawnHere) { break; }
                            GameObject arm2 = Instantiate(armPrefab, spawnDown, Quaternion.identity) as GameObject;
                            listOfPosition[index] = new Vector3(spawnDown.x, spawnDown.y, 0);
                            break;
                        case 3:
                            canSpawnHere = canSpawn(spawnLeft);
                            if (!canSpawnHere) { break; }
                            GameObject arm3 = Instantiate(armPrefab, spawnLeft, Quaternion.identity) as GameObject;
                            listOfPosition[index] = new Vector3(spawnLeft.x, spawnLeft.y, 0);
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
                            listOfPosition[index] = new Vector3(spawnUp.x, spawnUp.y, 0);
                            break;
                        case 2:
                            canSpawnHere = canSpawn(spawnDown);
                            if (!canSpawnHere) { break; }
                            GameObject arm2 = Instantiate(armPrefab, spawnDown, Quaternion.identity) as GameObject;
                            listOfPosition[index] = new Vector3(spawnDown.x, spawnDown.y, 0);
                            break;
                        case 3:
                            canSpawnHere = canSpawn(spawnLeft);
                            if (!canSpawnHere) { break; }
                            GameObject arm3 = Instantiate(armPrefab, spawnLeft, Quaternion.identity) as GameObject;
                            listOfPosition[index] = new Vector3(spawnLeft.x, spawnLeft.y, 0);
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
}
