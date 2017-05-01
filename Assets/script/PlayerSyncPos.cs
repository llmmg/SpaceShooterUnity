using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerSyncPos : NetworkBehaviour {

    [SyncVar]
    private Vector3 syncPos;
    [SyncVar]
    private Quaternion syncRotation;

    
    [SerializeField]
    Transform myTransform; //...player transform

    [SerializeField]
    float lerpRate=15;
	

	// Update is called once per frame
	void FixedUpdate () {
        TransmitPos();
        TransmitRotation();
        LerpPosition();
    }

    void LerpPosition()
    {
        //other player online
        if(!isLocalPlayer)
        {
            myTransform.position = Vector3.Lerp(myTransform.position, syncPos, Time.deltaTime * lerpRate);
            myTransform.rotation = Quaternion.Lerp(myTransform.rotation, syncRotation, Time.deltaTime * lerpRate);
        }
    }

    //run only on serv but call on client
    [Command]
    void CmdProvidePosToServer(Vector3 pos)
    {
        syncPos = pos;
    }

    //rotations
    [Command]
    void CmdProvideRotationToServer(Quaternion playerRot)
    {
        syncRotation = playerRot;
    }

    [ClientCallback]
    void TransmitPos()
    {
        if (isLocalPlayer)
        {
            CmdProvidePosToServer(myTransform.position);
        }
    }

    [Client]
    void TransmitRotation()
    {
        if(isLocalPlayer)
        {
            CmdProvideRotationToServer(myTransform.rotation);
        }
    }

}
