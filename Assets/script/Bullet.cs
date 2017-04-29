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
        if (collision.gameObject.tag == "Asteroid")
        {
            //Debug.Log(collision.gameObject.name);
            GameObject xplosion = Instantiate(explosion, transform.position, Quaternion.identity);
            

            Destroy(xplosion,1.5f);
            Destroy(this.gameObject);
        }
    }
}
