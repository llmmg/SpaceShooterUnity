using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SetupLocalPlayer : NetworkBehaviour {

	// Use this for initialization
	void Start () {
        if (isLocalPlayer)
        {
            GetComponent<Player>().enabled = true;
            //Camera.main.transform.position = this.transform.position - this.transform.up * 10;
            float size = this.transform.localScale.magnitude;
            Camera.main.transform.position = this.transform.position - this.transform.forward *5 + this.transform.up*2;
            //Camera.main.transform.position = this.transform.position - this.transform.forward* this.transform.localScale.x*6 + this.transform.up*this.transform.localScale.y*4;
            Camera.main.transform.LookAt(this.transform.position);
            Camera.main.transform.parent = this.transform;
        }
            
        //Camera.main.GetComponent().target=transform;
    }
}
