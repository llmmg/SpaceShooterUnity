﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidGenerator : MonoBehaviour {
    public GameObject AsteroidPrefab;
    public const int NUMBER_OF_ASTEROIDS = 1000;
    // Use this for initialization
    void Start () {
        //GameObject asteroid = (GameObject)Instantiate(AsteroidPrefab, transform.position, Quaternion.identity);
        GameObject[] asteroids;
        asteroids = new GameObject[NUMBER_OF_ASTEROIDS];
        for(int i=0;i<NUMBER_OF_ASTEROIDS; i++)
        {
            GameObject clone = (GameObject)Instantiate(AsteroidPrefab, new Vector3(Random.Range(-250, 250), Random.Range(-250, 250), Random.Range(-250, 250)), Quaternion.Euler(new Vector3(Random.Range(0, 90), Random.Range(0, 90), Random.Range(0, 90))));
            asteroids[i] = clone;
        }

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}