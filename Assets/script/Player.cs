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

    // Use this for initialization
    void Start () {
        rotateAngleY = 0;
        speed = 10f;
        rb.maxAngularVelocity = 2;

        /*this.transform.eulerAngles = new Vector3(
            this.transform.eulerAngles.x,
            this.transform.eulerAngles.y,
            this.transform.eulerAngles.z+180
            );*/


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
	
    void rePos(Rigidbody rb)
    {
        if (transform.position.x < 0)
            rb.position = new Vector3(10, transform.position.y, transform.position.z);
        if (transform.position.x > 500)
            rb.position = new Vector3(490, transform.position.y, transform.position.z);
        if (transform.position.y < 0)
            rb.position = new Vector3(transform.position.x, 10, transform.position.z);
        if (transform.position.y > 500)
            rb.position = new Vector3(transform.position.x, 490, transform.position.z);
        if (transform.position.z < 0)
            rb.position = new Vector3(transform.position.x, transform.position.y, 10);
        if (transform.position.z > 500)
            rb.position = new Vector3(transform.position.x, transform.position.y, 490);

    }

    // Update is called once per frame
    void LateUpdate() {
        //change position of player if off map
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
                speed += 1f;
        }
        //shooting 
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (this.speed >2)
                speed -= 1f;
        }
    }

    [Command]
    void CmdFire()
    {
         if(Time.time > nextActionTime)
        {
            nextActionTime += period;
            
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
    

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
}
