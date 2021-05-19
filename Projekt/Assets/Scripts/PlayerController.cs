using JetBrains.Annotations;
using ObjectScripts;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	/* Movement */
	
    public float speedModifier = 1;
    public float jumpModifier = 1;
    private Vector2 _movementVector;
    private const float Speed = 20; //Geschwindigkeit, die jede DeltaTime hinzugefügt wird
    private const float MINJumpSpeed = 300; //Geschwindigkeit, die nur 1-Mal hinzugefügt wird
    private const float MAXJumpSpeed = 600;
    private const float TimeTilMaxJump = 2.0f; //Sekunden, bis der volle Sprung ausgeführt wird
    private float _currentJumpCharge;
    private bool _jumpAllowed; // Gibt an, ob ein Sprung erlaubt ist (Bodenberührung)
    private bool _chargeJump;
    private bool _wallJumpAllowed;
    private Vector3 _wallJumpDirection; //Richtung, in der die Wand liegt - von der man weggeschleudert wird

    /* General */
    
    private Rigidbody _rb;
    public Vector3 lastCheckPoint; //Respawn Position für den Respawn
    
    /*Prefabs*/
    public GameObject dustCloud;
    public GameObject confetti;

    private void Awake()
    {
	    transform.position = GameObject.Find("SpawnpointTrigger").GetComponent<SpawnpointScript>().transform.position;
	    GameManager.StartListening("Respawn", Respawn);
    }
    private void OnDestroy()
    {
	    GameManager.StopListening("Respawn");
    }

    private void Start()
    {
	    _rb = GetComponent<Rigidbody>();
        _rb.isKinematic = true; //Bewegung wird deaktiviert, wird durch Kameraskript wieder aktiviert.
        lastCheckPoint = transform.position;
		_currentJumpCharge = MINJumpSpeed;
    }
    private void FixedUpdate() //Updated 1-Mal pro Frame
    {
	    //Jump
	    if (_chargeJump)
	    {
		    if (_currentJumpCharge < MAXJumpSpeed) 
			    _currentJumpCharge += ((MAXJumpSpeed - MINJumpSpeed) / TimeTilMaxJump) * Time.fixedDeltaTime;
		    else _currentJumpCharge = MAXJumpSpeed; //Damit aus sowas wie 4,000001 eine 4 wird - im generellen aber redundant, da es keinen Unterschied macht.
	    }

		//Movement 
		if (_movementVector.sqrMagnitude < 0.01) 
			return;
		Vector3 movement = new Vector3(_movementVector.x, 0.0f,_movementVector.y);
        _rb.AddForce(movement * (Speed * speedModifier));
        //Entfernt die Physikelemente des RigidBodys
        //rb.velocity = new Vector3(0, 0, 0);
        //rb.angularVelocity = new Vector3(0, 0, 0);
    }

    /* Getter und Setter ---------------------------------------------------------------------------------------------*/
    public void MultiplyJumpModifier(float modifier)
    {
	    jumpModifier *= modifier;
    }
    public void MultiplySpeedModifier(float modifier)
    {
	    speedModifier *= modifier;
    }
    /* Collider und Trigger ------------------------------------------------------------------------------------------*/
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
			GameManager.TriggerEvent("CoinCollected");
		}else if(other.gameObject.CompareTag("Heart"))
        {
	        other.gameObject.SetActive(false);
			GameManager.TriggerEvent("HeartCollected");

        }else if(other.gameObject.CompareTag("Death"))
        {
	        if (!_rb.isKinematic) //Beugt doppelten "death" trigger vor
	        {
		        _rb.GetComponent<SphereCollider>().enabled = false;
		        _rb.isKinematic = true;
		        GameManager.TriggerEvent("Death");
	        }
        }
		if(other.gameObject.CompareTag("Goal"))
		{
			_rb.GetComponent<SphereCollider>().enabled = false;
			_rb.isKinematic = true;
			GameManager.TriggerEvent("Victory");
			Instantiate(confetti, transform.position, dustCloud.transform.rotation);
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

    /* Methoden ------------------------------------------------------------------------------------------------------*/
    private void Respawn(string s){ //Trigger der durch den Wiederbeleben Knopf getriggered wird.
	    _rb.GetComponent<SphereCollider>().enabled = true;
	    transform.position = lastCheckPoint;
	    _rb.isKinematic = false;
		_rb.velocity = new Vector3(0, 0, 0);
		_rb.angularVelocity = new Vector3(0, 0, 0);
    }
    /* Input System Methoden -----------------------------------------------------------------------------------------*/
    [UsedImplicitly]
    public void OnMovement(InputAction.CallbackContext context) //Beim Drücken der Move Tasten
    {
	    _movementVector = context.ReadValue<Vector2>(); //Holt Vector2 Daten aus Movement
    }

    [UsedImplicitly]
    public void OnJump(InputAction.CallbackContext context)
    {
	    if (_wallJumpAllowed)
	    {
		    _jumpAllowed = false; //Damit man sich nicht gegen eine Wand stellen kann, springt und dann noch einmal in der Luft springen kann
		    Instantiate (dustCloud, transform.position, dustCloud.transform.rotation); //Partikelwolke
		    //Entfernt die Fallkraft, damit jeder Sprung gleichhoch ist, egal, wie lange man fällt.
		    var velocity = _rb.velocity;
		    _rb.velocity = new Vector3(velocity.x, 0, velocity.z);
		    //Fügt in die umgedrehte Wandrichtigung Force hinzu (+ Force nach oben);
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
				    //Debug.Log("Jump losgelassen, speedModifier:"+_currentJumpCharge*20);
				    _jumpAllowed = false;
				    _chargeJump = false;
				    Instantiate(dustCloud, transform.position, dustCloud.transform.rotation);
				    //Entfernt die Fallkraft, damit jeder Sprung gleichhoch ist, selbst, wenn man fällt.
				    var velocity = _rb.velocity;
				    _rb.velocity = new Vector3(velocity.x, 0, velocity.z);
				    _rb.AddForce(new Vector3(0.0f, jumpModifier*_currentJumpCharge, 0.0f));
				    _currentJumpCharge = MINJumpSpeed;
				    break;
		    }
	    }
    }

}
