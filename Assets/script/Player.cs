using System.Timers;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Player : NetworkBehaviour {

    private Rigidbody rb;
    private Mesh theMesh;

    /* Movement Tools */
    public float speed;
    public float horTurnSpeed;
    public float verTurnSpeed;

    /* Shoot tools */
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public Transform bulletSpawn2;

    /* Rotate tools */
    public Transform myObject;
    private Vector3[] originalVerts;
    private Vector3[] rotatedVerts;
    private float rotateAngleY;
    private float nextActionTime;
    public float period;


    /* Score tools */
    private int score=0;
    Text scoreText;
    public int StartScore;

    /* Life tools */
    Text lifeText;
    private int life;
    public int StartLife;

    /* Field tools */
    public int FIELDLIMIT = 500;


    // Use this for initialization
    void Start () {

        rotateAngleY = 0;
        speed = 10f;
        rb.maxAngularVelocity = 2;

        /* Get transform */
        if (!myObject)
        {
            myObject = this.transform;           
        }

        /* Get Life text */
        GameObject objLife = GameObject.Find("lifeText");
        lifeText = objLife.GetComponent<Text>();

        /* Get Score text */
        GameObject objScore = GameObject.Find("scoreText");
        scoreText = objScore.GetComponent<Text>();


        /* Mesh Initialisation */
        theMesh = myObject.GetComponent<MeshFilter>().mesh as Mesh;
        originalVerts = new Vector3[theMesh.vertices.Length];
        originalVerts = theMesh.vertices;
        rotatedVerts = new Vector3[originalVerts.Length];

        //init life points
        life = StartLife;
        score = StartScore;
        lifeText.text = string.Format("Life: {0}", life);
        scoreText.text = string.Format("Score: {0}", score);
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


    /* Score informations */
    public void incScore(int value)
    {
        if (isLocalPlayer)
        {
            score += value;
            Debug.Log(score);
            scoreText.text = string.Format("Score: {0}", score);
        }

    }

    /* Life informations */
    private void LostLife()
    {
        if (isLocalPlayer)
        {
            life = life - 1;
            lifeText.text = string.Format("Life: {0}", life);
            /* When he got 0 life, respawn */
            if (life < 0)
            {
                CmdRespawn();
                life = StartLife;
                //score = 0;
            }
        }
    }

    /* Repos if out of field */
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
        Camera.main.transform.position = this.transform.position - this.transform.forward * 5 + this.transform.up * 2;
        Camera.main.transform.LookAt(this.transform.position);
        Camera.main.transform.rotation = transform.rotation;
        /* Limit field of server */
        rePos(rb);

        /* Speed adaptation */
        rb.velocity = transform.forward * speed;
        float x = Input.GetAxis("Vertical");
        float y = Input.GetAxis("Horizontal");
  
        //up/Down
        rb.AddRelativeTorque(x * verTurnSpeed * Time.deltaTime, y * horTurnSpeed * Time.deltaTime, 0);
        //barel roll
        rb.AddRelativeTorque(y * (-1) * verTurnSpeed * Vector3.forward * Time.deltaTime);


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

        //tracking --test
        GameObject rectTrans= GameObject.Find("mire");
        Text mire = rectTrans.GetComponent<Text>();

        Vector3 viewPos = Camera.main.WorldToScreenPoint(GameObject.FindWithTag("Player").transform.position);
       
        print(viewPos);
        //print(Screen.height + " width: " + Screen.width);
        //729 1440
        if (viewPos.x < 0)
            viewPos.x = 0;
        if (viewPos.x > Screen.width)
            viewPos.x = Screen.width;
        if(viewPos.y <0)
        {
            viewPos.y = 0;
        }
        if(viewPos.y>Screen.height)
        {
            viewPos.y = Screen.height;
        }
        
        if (viewPos.z < 0)
        {
            viewPos.x += Screen.width - viewPos.x;
            viewPos.y += Screen.height - viewPos.y;
        }

        mire.transform.position = (viewPos);       


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
            bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 60;
            bullet2.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 60;

            NetworkServer.Spawn(bullet);
            NetworkServer.Spawn(bullet2);

            // Destroy the bullet after 5 seconds
            Destroy(bullet, 5.0f);
            Destroy(bullet2, 5.0f);

        }
           

    }
    private void OnCollisionEnter(Collision collision)
    {
        /* Player lost 1 life */
		LostLife();
        print("collision with "+ collision.gameObject.name+" life:"+life.ToString());
    }

    [Command]
    void CmdRespawn()
    {

        print("Respawn");

        //GameObject player = Instantiate<GameObject>(NetworkManager.singleton.playerPrefab);
        //NetworkServer.UnSpawn(this.gameObject);
        //NetworkServer.Spawn(player);
        //NetworkManager.Destroy(this.gameObject);
        //NetworkServer.DestroyPlayersForConnection(this.connectionToClient);
        //NetworkServer.AddPlayerForConnection(this.connectionToClient,player,this.playerControllerId);
        //NetworkServer.Spawn(player);
        //NetworkServer.ReplacePlayerForConnection(this.connectionToClient, player, this.playerControllerId);
        //var spawn = NetworkManager.singleton.GetStartPosition();
        var newPlayer = (GameObject)Instantiate(NetworkManager.singleton.playerPrefab, Vector3.zero, Quaternion.identity);
        Destroy(this.gameObject);
        //NetworkServer.Destroy(this.gameObject);
        NetworkServer.ReplacePlayerForConnection(this.connectionToClient, newPlayer, this.playerControllerId);


       
    }


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

    }
}
