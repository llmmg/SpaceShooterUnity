using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bullet : NetworkBehaviour {
    public GameObject explosion;
    private Player collidePlayer;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Asteroid" || collision.gameObject.tag == "Player")
        {
            GameObject xplosion = Instantiate(explosion, transform.position, Quaternion.identity);
            Destroy(xplosion,1.5f);
            Destroy(this.gameObject);

        }

        if (collision.gameObject.tag == "Player")
        {
            collidePlayer = this.transform.parent.GetComponent<Player>();
            collidePlayer.incScore(100);
        }

    }
}
