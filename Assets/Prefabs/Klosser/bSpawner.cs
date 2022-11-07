using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bSpawner : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject bPrefab;
    private Vector2 pos;
    [SerializeField] private float threshold = 5;

    private float _timeAccumulated;

    public bool CanPlaceObj => _timeAccumulated > threshold;
    void Start()
    {
        SpawnObj();
        
    }

    void Update(){
        if (CanPlaceObj){
            SpawnObj();
            _timeAccumulated = 0;
            return;
        }
        _timeAccumulated += Time.deltaTime;
    }


    private void SpawnObj(){
        pos = new Vector2(Random.Range(-45, 45), Random.Range(-25,25));
        GameObject obj = Instantiate(bPrefab,pos,Quaternion.identity);
        obj.transform.parent = transform;
    }

    // Update is called once per frame


}
