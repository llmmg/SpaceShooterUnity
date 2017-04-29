using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerSyncPos : NetworkBehaviour {

    [SyncVar]
    private Vector3 syncPos;
    
    [SerializeField]
    Transform myTransform;
    [SerializeField]
    float lerpRate=15;
	
	// Update is called once per frame
	void FixedUpdate () {
        TransmitPos();
        LerpPosition(); 
	}

    void LerpPosition()
    {
        //other player online
        if(!isLocalPlayer)
        {
            myTransform.position = Vector3.Lerp(myTransform.position, syncPos, Time.deltaTime * lerpRate);
        }
    }

    //run only on serv but call on client
    [Command]
    void CmdProvidePosToServer(Vector3 pos)
    {
        syncPos = pos;
    }

    [ClientCallback]
    void TransmitPos()
    {
        if (isLocalPlayer)
        {
            CmdProvidePosToServer(myTransform.position);
        }
    }

}
