using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Asteroid : NetworkBehaviour
{
    public GameObject explosion;
    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnCollisionEnter(Collision collision)
    {
        explosion.transform.localScale = new Vector3(100, 100, 100);
        Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(this.gameObject,1);
    }

}
