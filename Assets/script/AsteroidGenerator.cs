using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AsteroidGenerator : NetworkBehaviour {

    public GameObject AsteroidPrefab;

    /* Const variables */ 
    public int NUMBER_OF_ASTEROIDS = 500;
    public int FIELDLIMIT = 500;
    public const int MAX_ASTEROID_SIZE = 60;
    public const int MIN_ASTEROID_SIZE = 5;


    // Use this for initialization
    void Start () {
        for(int i=0;i<NUMBER_OF_ASTEROIDS; i++)
        {
            GameObject clone = (GameObject)Instantiate(AsteroidPrefab, new Vector3(Random.Range(-FIELDLIMIT, FIELDLIMIT), Random.Range(-FIELDLIMIT, FIELDLIMIT), Random.Range(-FIELDLIMIT, FIELDLIMIT)), Quaternion.Euler(new Vector3(Random.Range(0, 90), Random.Range(0, 90), Random.Range(0, 90))));
            clone.transform.localScale= new Vector3(Random.Range(MIN_ASTEROID_SIZE, MAX_ASTEROID_SIZE), Random.Range(MIN_ASTEROID_SIZE, MAX_ASTEROID_SIZE), Random.Range(MIN_ASTEROID_SIZE, MAX_ASTEROID_SIZE));
            NetworkServer.Spawn(clone);
        }

    }
	// Update is called once per frame
	void Update () {
		
	}
}