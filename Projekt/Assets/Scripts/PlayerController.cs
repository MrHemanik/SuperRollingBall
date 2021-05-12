using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
public class PlayerController : MonoBehaviour
{
	/* Movement */
	
    public float speed = 20;
    public Vector2 movementVector;
    public float minJumpSpeed = 10;
    public float maxJumpSpeed = 20;
    public float timeTilMaxJump = 3.0f; //Sekunden, bis der volle Sprung ausgeführt wird
    private float _currentJumpCharge;
    private bool _jumpAllowed; // Gibt an, ob ein Sprung erlaubt ist (Bodenberührung)
    private bool _chargeJump;
    private bool _wallJumpAllowed;
    private Vector3 _wallJumpDirection; //Richtung, in der die Wand liegt - von der man weggeschleudert wird
    
    /* General */
    
    private Rigidbody _rb;
    private Vector3 _lastCheckPoint; //Respawn Position für den Respawn
    public GameObject dustCloud;

    private void Awake()
    { 
	    GameManager.StartListening("Respawn", Respawn);
    }

    private void Start()
    {
	    _rb = GetComponent<Rigidbody>();
        _rb.isKinematic = true; //Bewegung wird deaktiviert, wird durch Kameraskript wieder aktiviert.
        _lastCheckPoint = transform.position;
		_currentJumpCharge = minJumpSpeed;
    }

    public void OnMovement(InputAction.CallbackContext context) //Beim Drücken der Move Tasten
    {
	    movementVector = context.ReadValue<Vector2>(); //Holt Vector2 Daten aus Movement
    }

    public void OnJump(InputAction.CallbackContext context)
    {
	    if (_wallJumpAllowed)
        {
	        _jumpAllowed = false; //Damit man sich nicht gegen eine Wand stellen kann, springt und dann noch einmal in der Luft springen kann
	        //Fügt in die umgedrehte Wandrichtigung Force hinzu (+ Force nach oben);
			Instantiate (dustCloud, transform.position, dustCloud.transform.rotation);
            _rb.AddForce(new Vector3(-500.0f*_wallJumpDirection.x, 400.0f, -500.0f*_wallJumpDirection.z));
             
        }
        else if (_jumpAllowed)
	    {
		    switch (context.phase)
		    {
			    case InputActionPhase.Started:
				    _chargeJump = true;
				    break;
			    case InputActionPhase.Canceled:
				    //Debug.Log("Jump losgelassen, speed:"+_currentJumpCharge*20);
				    _jumpAllowed = false;
				    _chargeJump = false;
				    Instantiate(dustCloud, transform.position, dustCloud.transform.rotation);
				    _rb.AddForce(new Vector3(0.0f, _currentJumpCharge*20, 0.0f));
				    _currentJumpCharge = minJumpSpeed;
				    break;
		    }
	    }
    }

    private void FixedUpdate() //Updated 1-Mal pro Frame
    {
	    //Jump
	    if (_chargeJump)
	    {
		    if (_currentJumpCharge < maxJumpSpeed) 
			    _currentJumpCharge += ((maxJumpSpeed - minJumpSpeed) / timeTilMaxJump) * Time.fixedDeltaTime;
		    else _currentJumpCharge = maxJumpSpeed; //Damit aus sowas wie 4,000001 eine 4 wird - im generellen aber redundant, da es keinen Unterschied macht.
	    }

		//Movement
		if (movementVector.sqrMagnitude < 0.01) 
			return;
		Vector3 movement = new Vector3(movementVector.x, 0.0f,movementVector.y);
        _rb.AddForce(movement * speed);
        //Entfernt die Physikelemente des RigidBodys
        //rb.velocity = new Vector3(0, 0, 0);
        //rb.angularVelocity = new Vector3(0, 0, 0);
    }

    private void OnCollisionEnter(Collision other) //Bei Berührung eines Objektes (Kollision)
    {
        if(other.gameObject.CompareTag("Ground")) //Berührung mit Boden
        {
            _jumpAllowed = true;
        }
        else if (other.gameObject.CompareTag("Walljump")) //Berührung mit Abspringbarer Wand
        {
            _rb.velocity = new Vector3(0, 0, 0);
            _wallJumpAllowed = true;
            _wallJumpDirection = other.contacts[0].point - transform.position;
			//TODO wallJumpDirection werte MathSignen, wo aber 0 auch 0 bleibt;
            //print("Wand in Richtung: " + _wallJumpDirection);
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if(other.gameObject.CompareTag("Walljump")) //Berührung mit Boden
        {
            _wallJumpAllowed = false;
        }

        //transform.SetParent(null);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Moving"))
        {
            //Wird mit der Moving Plattform mitbewegt (Moving Plattform muss Scale (1,1,1) haben!
            //Debug.Log("Trigger");
            transform.parent.parent = other.transform;
        }else if(other.gameObject.CompareTag("Coin"))
        {
	        other.gameObject.SetActive(false);
			GameManager.TriggerEvent("CoinCollected",0);
			
		}else if(other.gameObject.CompareTag("Death"))
        {
	        Time.timeScale = 0;
	        Debug.Log("DEATHHH");
	        GameManager.TriggerEvent("Death",0);
        }
		if(other.gameObject.CompareTag("Goal")){
			Time.timeScale = 0;
			GameManager.TriggerEvent("Victory",0);
		}
		

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Moving"))
        {
            //Debug.Log("Release");
            //Player wird von der Plattform gelöst
            transform.parent.SetParent(null);
        }
    }

    private void Respawn(float f){ //Trigger der durch den Wiederbeleben Knopf getriggered wird.
	    transform.position = _lastCheckPoint;
		_rb.velocity = new Vector3(0, 0, 0);
		_rb.angularVelocity = new Vector3(0, 0, 0);
    }
}
