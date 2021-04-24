using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed = 10;
    public float jumpSpeed = 10;
    private Rigidbody rb;
	public bool movementAllowed = true;
    public bool jumpAllowed = false; // Gibt an, ob ein Sprung erlaubt ist (Bodenberührung)
    public bool wallJumpAllowed = false;
    private Vector3 wallJumpDirection; //Richtung, in der die Wand liegt - von der man weggeschleudert wird
	private Vector3 lastCheckPoint; //Respawn Position für den Respawn
    public Vector2 movementVector;
    private float movementZ;
	public GameObject GameOver;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start");
        rb = GetComponent<Rigidbody>();
		lastCheckPoint = transform.position;
		GameOver.SetActive(false); //Fühlt sich wie die falsche Stelle an um das zu machen, maybe GameManagerObject?
    }
    
    void OnMovement(InputValue movementValue) //Beim Drücken der Move Tasten
    {
        movementVector = movementValue.Get<Vector2>(); //Holt Vector2 Daten aus Movement
    }

    void OnJump()
    {
        if (wallJumpAllowed)
        {
            //Fügt in die umgedrehte Ballrichtigung
            Debug.Log("SPRUNGGG");
            rb.AddForce(new Vector3(-1000.0f*wallJumpDirection.x, 1000.0f, -1000.0f*wallJumpDirection.z));
        }
        else if (jumpAllowed)
        {
            jumpAllowed = false;
            rb.AddForce(new Vector3(0.0f, 20*jumpSpeed, 0.0f));
        }
    }
    void FixedUpdate() //Updated 1-Mal pro Frame
    {
        if (movementVector.sqrMagnitude < 0.01 || !movementAllowed)
            return;

        Vector3 movement = new Vector3(movementVector.x, 0.0f,movementVector.y);
        rb.AddForce(movement * speed);
        //Entfernt die Physikelemente des RigidBodys
        //rb.velocity = new Vector3(0, 0, 0);
        //rb.angularVelocity = new Vector3(0, 0, 0);
    }
    void OnCollisionEnter(Collision other) //Bei Berührung eines Objektes (Kollision)
    {
        if(other.gameObject.CompareTag("Ground")) //Berührung mit Boden
        {
            jumpAllowed = true;
        }
        else if (other.gameObject.CompareTag("Walljump")) //Berührung mit Abspringbarer Wand
        {
            rb.velocity = new Vector3(0, 0, 0);
            wallJumpAllowed = true;
            wallJumpDirection = other.contacts[0].point - transform.position;
			//TODO wallJumpDirection werte MathSignen, wo aber 0 auch 0 bleibt;
            print("Wand in Richtung: " + wallJumpDirection);
        }
    }
    

    void OnCollisionExit(Collision other)
    {
        if(other.gameObject.CompareTag("Walljump")) //Berührung mit Boden
        {
            wallJumpAllowed = false;
        }

        //transform.SetParent(null);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Moving"))
        {
            //Wird mit der Moving Plattform mitbewegt (Moving Plattform muss Scale (1,1,1) haben!
            //Debug.Log("Trigger");
            transform.parent.parent = other.transform;
        }else if(other.gameObject.CompareTag("Death")){
			GameOver.SetActive(true);
		}

    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Moving"))
        {
            //Debug.Log("Release");
            //Player wird von der Plattform gelöst
            transform.parent.SetParent(null);
        }
    }
    void OnRespawn(){ //Trigger der durch den Wiederbeleben Knopf getriggered wird.
		transform.position = lastCheckPoint;
		rb.velocity = new Vector3(0, 0, 0);
		rb.angularVelocity = new Vector3(0, 0, 0);
		GameOver.SetActive(false);
		movementAllowed = true;
	}
}
