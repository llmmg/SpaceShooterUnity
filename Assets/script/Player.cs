using System.Timers;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {

    private Rigidbody rb;
    private Mesh theMesh;

    public float speed;
    public float horTurnSpeed;
    public float verTurnSpeed;

    public GameObject bulletPrefab;

    public Transform bulletSpawn;
    public Transform bulletSpawn2;

    //player
    public Transform myObject;
    private Vector3[] originalVerts;
    private Vector3[] rotatedVerts;
    private float rotateAngleY;

    private float nextActionTime;
    public float period;

    private int life=10;
    private int score=0;

    /* Const variables */
    private const int FIELDLIMIT = 500;

    
    

    // Use this for initialization
    void Start () {
        rotateAngleY = 0;
        speed = 10f;
        rb.maxAngularVelocity = 2;

        if (!myObject)
        {
            myObject = this.transform;
           
        }
        
        theMesh = myObject.GetComponent<MeshFilter>().mesh as Mesh;
        originalVerts = new Vector3[theMesh.vertices.Length];
        originalVerts = theMesh.vertices;

        rotatedVerts = new Vector3[originalVerts.Length];

        //Re-orient object
        //RotateMesh();
	}
    void RotateMesh()
    {
        Quaternion qAngle = Quaternion.AngleAxis(rotateAngleY, Vector3.up);
        for (int vert=0; vert<originalVerts.Length; vert++)
        {
            rotatedVerts[vert] = qAngle * originalVerts[vert];
        }

        theMesh.vertices = rotatedVerts;
    }
	public void incScore(int value)
    {
        score += value;
    }
    void LostLife()
    {
        life = life - 1;
    }
    void rePos(Rigidbody rb)
    {
        if (transform.position.x < -FIELDLIMIT)
            rb.position = new Vector3(-FIELDLIMIT+10, transform.position.y, transform.position.z);
        if (transform.position.x > FIELDLIMIT)
            rb.position = new Vector3(FIELDLIMIT-10, transform.position.y, transform.position.z);
        if (transform.position.y < -FIELDLIMIT)
            rb.position = new Vector3(transform.position.x,-FIELDLIMIT+ 10, transform.position.z);
        if (transform.position.y > FIELDLIMIT)
            rb.position = new Vector3(transform.position.x, FIELDLIMIT - 10, transform.position.z);
        if (transform.position.z < -FIELDLIMIT)
            rb.position = new Vector3(transform.position.x, transform.position.y, -FIELDLIMIT+10);
        if (transform.position.z > FIELDLIMIT)
            rb.position = new Vector3(transform.position.x, transform.position.y, FIELDLIMIT - 10);

    }

    // Update is called once per frame
    void LateUpdate() {
        //rePos(rb);

        rb.velocity = transform.forward * speed;
        float x = Input.GetAxis("Vertical");
        float y = Input.GetAxis("Horizontal");
  
        //up/Down
        rb.AddRelativeTorque(x * verTurnSpeed * Time.deltaTime, y * horTurnSpeed * Time.deltaTime, 0);
        //barel roll
        rb.AddRelativeTorque(y * (-1) * verTurnSpeed * Vector3.forward * Time.deltaTime);
        //rb.AddRelativeTorque(x * (-1) * verTurnSpeed * Vector3.forward * Time.deltaTime);


        //shooting 
        if (Input.GetKey(KeyCode.Space))
        {          
            CmdFire();
        }
        //shooting 
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (this.speed < 40)
                speed += 0.3f;
        }
        //shooting 
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (this.speed >2)
                speed -= 0.3f;
        }
    }

    [Command]
    void CmdFire()
    {

        if (Time.time > nextActionTime)
        {
            nextActionTime = Time.time+period;
            
            // Create the Bullet from the Bullet Prefab
            GameObject bullet = (GameObject)Instantiate(
                bulletPrefab,
                bulletSpawn.position,
                bulletSpawn.rotation);

            GameObject bullet2 = (GameObject)Instantiate(
                bulletPrefab,
                bulletSpawn2.position,
                bulletSpawn2.rotation);


            // Add velocity to the bullet
            bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 60+ myObject.transform.forward;
            bullet2.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 60 + myObject.transform.forward;

            NetworkServer.Spawn(bullet);
            NetworkServer.Spawn(bullet2);

            // Destroy the bullet after 5 seconds
            Destroy(bullet, 5.0f);
            Destroy(bullet2, 5.0f);

        }
           

    }
    private void OnCollisionEnter(Collision collision)
    {
		LostLife();
        if (life<0)
        {
            //CmdRespawn();
        }
    }

    [Command]
    void CmdRespawn()
    {
        GameObject player = Instantiate<GameObject>(NetworkManager.singleton.playerPrefab);
        //NetworkServer.UnSpawn(this.gameObject);
        //NetworkServer.Spawn(player);
        NetworkManager.Destroy(this.gameObject);
        NetworkServer.DestroyPlayersForConnection(this.connectionToClient);
        NetworkServer.AddPlayerForConnection(this.connectionToClient,player,this.playerControllerId);
        //NetworkServer.ReplacePlayerForConnection(this.connectionToClient, player, this.playerControllerId);
        //var spawn = NetworkManager.singleton.GetStartPosition();
        //var newPlayer = (GameObject)Instantiate(NetworkManager.singleton.playerPrefab, Vector3.zero, Quaternion.identity);
        //Destroy(this.gameObject);
        //NetworkServer.Destroy(this.gameObject);
        //NetworkServer.ReplacePlayerForConnection(this.connectionToClient, newPlayer, this.playerControllerId);
    }


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
}
