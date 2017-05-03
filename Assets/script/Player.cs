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

    //public Text FTW;
    private int i=0; //dirty index


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
            if (life <= 0)
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
        //speedup
        if (Input.GetKey(KeyCode.P))
        {
            if (this.speed < 35)
                speed += 0.3f;
        }
        //slowdown
        if (Input.GetKey(KeyCode.L))
        {
            if (this.speed >2)
                speed -= 0.3f;
        }

        //SUPERSPEED
        if (Input.GetKey(KeyCode.O))
        {
            if (this.speed < 100)
                speed += 2f;
        }
        //shooting 
        if (Input.GetKey(KeyCode.K))
        {
            if (this.speed > 2)
                speed -= 2f;
        }

        //tracking --test
        GameObject rectTrans = GameObject.Find("mire");
        Text mire = rectTrans.GetComponent<Text>();

        float mindist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        GameObject[] pos = GameObject.FindGameObjectsWithTag("Player");
        GameObject closest = this.gameObject;
        foreach(GameObject obj in pos)
        {
            Vector3 diff = obj.transform.position - currentPos;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < mindist && curDistance !=0)
            {
                closest = obj;
                mindist = curDistance;
            }
        }

        Vector3 viewPos = Camera.main.WorldToScreenPoint(closest.transform.position);
        correctedPos(ref viewPos);

        mire.transform.position = (viewPos);
    }

    void displayARound()
    {

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        //get one player from players at each loop of update
        i = (i + 1) % players.Length;
        GameObject currentPlayer = players[i];

        GameObject rectTrans = GameObject.Find("mire");
        Text myText = rectTrans.GetComponent<Text>();

        Vector3 viewPosTmp = Camera.main.WorldToScreenPoint(currentPlayer.transform.position);
        correctedPos(ref viewPosTmp);

        myText.transform.position = viewPosTmp;


        //foreach (GameObject player in players)
        //{
        //    //Text myText = FTW.GetComponent<Text>();

        //    //Position
        //    //Vector3 viewPosTmp = Camera.main.WorldToScreenPoint(player.transform.position);
        //    //correctedPos(ref viewPosTmp);
        //    ////myText.transform.SetParent(GameObject.Find("Canvas").transform);

        //    //myText.transform.position = viewPosTmp;
        //}

        //GameObject rectTrans= GameObject.Find("mire");
        //Text mire = rectTrans.GetComponent<Text>();
        //Vector3 viewPos = Camera.main.WorldToScreenPoint(GameObject.FindWithTag("Player").transform.position);       
        //print(viewPos);

        //print(Screen.height + " width: " + Screen.width);
        //729 1440
        //correctedPos(ref viewPos);

        //mire.transform.position = (viewPos);   
    }
    void correctedPos(ref Vector3 viewedPos)
    {
        if (viewedPos.x < 0)
            viewedPos.x = 0;
        if (viewedPos.x > Screen.width)
            viewedPos.x = Screen.width;
        if (viewedPos.y < 0)
        {
            viewedPos.y = 0;
        }
        if (viewedPos.y > Screen.height)
        {
            viewedPos.y = Screen.height;
        }

        if (viewedPos.z < 0)
        {
            viewedPos.x += Screen.width - viewedPos.x;
            viewedPos.y += Screen.height - viewedPos.y;
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
            bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 80;
            bullet2.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 80;

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
        var newPlayer = (GameObject)Instantiate(NetworkManager.singleton.playerPrefab, new Vector3(Random.Range(-FIELDLIMIT, FIELDLIMIT), Random.Range(-FIELDLIMIT, FIELDLIMIT), Random.Range(-FIELDLIMIT, FIELDLIMIT)), Quaternion.identity);
        Destroy(this.gameObject);
        //NetworkServer.Destroy(this.gameObject);
        NetworkServer.ReplacePlayerForConnection(this.connectionToClient, newPlayer, this.playerControllerId);


       
    }


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

    }
}
