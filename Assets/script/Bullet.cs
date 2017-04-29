using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bullet : NetworkBehaviour {
    public GameObject explosion;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.gameObject.name);
        GameObject xplosion = (GameObject)Instantiate(explosion, transform.position, Quaternion.identity);
        
        Destroy(this.gameObject);
        Destroy(xplosion,3.0f);

        //print("COLLISION!!!");
        
    }
}
