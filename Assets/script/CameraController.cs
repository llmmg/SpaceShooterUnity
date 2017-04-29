using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    private Player player;
    private Vector3 offset;

	// Use this for initialization
	void Start () {
        offset = this.transform.position - player.transform.position;	
	}
	
	// Update is called once per frame
	void LateUpdate () {
     
    }
    private void Awake()
    {
        player = FindObjectOfType<Player>();
    }
}
