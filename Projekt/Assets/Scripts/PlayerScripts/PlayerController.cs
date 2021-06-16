using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ObjectScripts;
using ScreenScripts;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerScripts
{
	public class PlayerController : MonoBehaviour
	{
		#region Variables
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
		private float _standardMass;
		private float _standardDrag;
		private HudScript _hudScript;
		private Transform _cameraTransform;
    
		private bool _pullObjects;
		public List<GameObject> pullableObjectsInRange;
    
    

		/* General */
    
		private Rigidbody _rb;
		public Vector3 lastCheckPoint; //Respawn Position für den Respawn
    
		/*Prefabs*/
		public GameObject dustCloud;
		public GameObject confetti;
		public GameObject statusPrefab;
		#endregion
		/* Standard Methoden ---------------------------------------------------------------------------------------------*/
		#region StandardMethods
		private void Awake()
		{
			transform.position = GameObject.Find("SpawnpointTrigger").GetComponent<SpawnpointScript>().transform.position;
			_cameraTransform = transform.Find("Camera");
			GameManager.StartListening("BallRespawn", BallRespawn);
			GameManager.StartListening("ResizeCore", ResizeCore);
			GameManager.StartListening("BallDeath", BallDeath);
		}
		private void OnDestroy()
		{
			GameManager.StopListening("BallRespawn");
			GameManager.StopListening("ResizeCore");
			GameManager.StopListening("BallDeath");
		}

		private void Start()
		{
			_rb = GetComponent<Rigidbody>();
			Transform transform1 = transform;
			_hudScript = transform1.Find("UI Screens").Find("HUD").GetComponent<HudScript>();
			_rb.isKinematic = true; //Bewegung wird deaktiviert, wird durch Kameraskript wieder aktiviert.
			lastCheckPoint = transform1.position;
			_currentJumpCharge = MINJumpSpeed;
			_standardDrag = _rb.drag;
			_standardMass = _rb.mass;
		}
		private void FixedUpdate() //Updated 1-Mal pro Frame
		{
			//Jump
			if (_chargeJump)
			{
				if (_currentJumpCharge < MAXJumpSpeed)
				{
					UpdateCurrentJumpCharge(_currentJumpCharge + ((MAXJumpSpeed - MINJumpSpeed) / TimeTilMaxJump) * Time.fixedDeltaTime);
			    
				}
				else
				{
					UpdateCurrentJumpCharge(MAXJumpSpeed); //Damit aus sowas wie 4,000001 eine 4 wird - im generellen aber redundant, da es keinen Unterschied macht.
				}
			} 
	    
			//Movement 
			if (_movementVector.sqrMagnitude > 0.01){
				Vector3 forward = _cameraTransform.forward;
				forward = new Vector3(forward.x, 0.0f, forward.z);
				forward.Normalize();
				float angle = (int) (Mathf.Atan2(_movementVector.x, _movementVector.y)*180/Math.PI); //https://answers.unity.com/questions/914088/get-angle-of-vector2.html
				Vector3 movement = Quaternion.AngleAxis(angle, Vector3.up) * forward;

				_rb.AddForce(movement * (Speed * speedModifier));
			}
			//Pull Objects
			if (_pullObjects)
			{
				Vector3 playerPos = transform.position;
				foreach (var pullObject in pullableObjectsInRange)
				{
					Vector3 pullPos = pullObject.transform.position;

					pullObject.GetComponent<Rigidbody>().AddForce(playerPos - pullPos);
				}
			}
		}
		#endregion
		/* Getter und Setter ---------------------------------------------------------------------------------------------*/
		#region GetterSetter
		public void MultiplyJumpModifier(float modifier)
		{
			jumpModifier *= modifier;
		}
		public void MultiplySpeedModifier(float modifier)
		{
			speedModifier *= modifier;
		}

		public void PlayerWetness(float modifier)
		{
			_rb.drag *= modifier;
			_rb.mass *= modifier;
			speedModifier *= 1 / modifier;
		}
		#endregion
		/* Collider und Trigger ------------------------------------------------------------------------------------------*/
		#region Collider&Trigger
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
			else if(other.gameObject.CompareTag("Spike"))
			{
				GameManager.TriggerEvent("DamageTaken");
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
			if (other.gameObject.CompareTag("PullRange")) return; //Falls die PullRange versucht das hier zu triggern wirds blockiert
			if(other.gameObject.CompareTag("Water"))
			{
				//Im wasser ist man langsam, aber springt höher
				_rb.drag = 4;
				_rb.mass = 1;
			}else if(other.gameObject.CompareTag("Spike"))
			{
				Debug.Log("Damage");
				GameManager.TriggerEvent("DamageTaken");
			}else if(other.gameObject.CompareTag("Coin"))
			{
				other.gameObject.SetActive(false);
				GameManager.TriggerEvent("CoinCollected");
			}else if(other.gameObject.CompareTag("Heart"))
			{
				other.gameObject.SetActive(false);
				GameManager.TriggerEvent("HeartCollected");

			}else if(other.gameObject.CompareTag("HitPoints"))
			{
				other.gameObject.SetActive(false);
				GameManager.TriggerEvent("HitPointsCollected");

			}else if(other.gameObject.CompareTag("Death"))
			{
				BallDeath();
			}
			else if(other.gameObject.CompareTag("Goal"))
			{
				_rb.GetComponent<SphereCollider>().enabled = false;
				_rb.isKinematic = true;
				GameManager.TriggerEvent("Victory");
				Instantiate(confetti, transform.position, dustCloud.transform.rotation);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.gameObject.CompareTag("PullRange")) return; //Falls die PullRange versucht das hier zu triggern wirds blockiert
			if(other.gameObject.CompareTag("Water"))
			{
				_rb.drag = _standardDrag;
				_rb.mass = _standardMass;
				GameObject statusObject = Instantiate(statusPrefab, transform.position, new Quaternion(),
					transform.Find("StatusEffects").transform);
				statusObject.GetComponent<StatusEffectScript>().Initialize(2,2, GetComponent<PlayerController>(), gameObject);
			}
		}
		#endregion
		/* Event Methoden ------------------------------------------------------------------------------------------------*/
		#region EventMethods
		private void BallRespawn(string s){ //Trigger der durch den Wiederbeleben Knopf getriggered wird.
			_rb.GetComponent<SphereCollider>().enabled = true;
			transform.position = lastCheckPoint;
			_rb.isKinematic = false;
			_rb.velocity = new Vector3(0, 0, 0);
			_rb.angularVelocity = new Vector3(0, 0, 0);
			_rb.drag = _standardDrag;
			_rb.mass = _standardMass;
			transform.SetParent(null);
			GameManager.TriggerEvent("HitPointsCollected"); //Setzt das Leben wieder voll und ruft danach ResizeCore auf
		}

		private void ResizeCore(string size)
		{
			float newSize = float.Parse(size);
			transform.GetChild(0).localScale = new Vector3(newSize, newSize, newSize);
		}
		private void BallDeath(string s ="")
		{
			if (!_rb.isKinematic) //Beugt doppelten "death" trigger vor
			{
				_rb.GetComponent<SphereCollider>().enabled = false;
				_rb.isKinematic = true;
				GameManager.TriggerEvent("Death");
			}
		}

		private void UpdateCurrentJumpCharge(float jumpCharge)
		{
			_currentJumpCharge = jumpCharge;
			_hudScript.UpdateJumpChargeDisplay((_currentJumpCharge-MINJumpSpeed)/(MAXJumpSpeed-MINJumpSpeed));
		}
		#endregion
		/* Input System Methoden -----------------------------------------------------------------------------------------*/
		#region InputSystem
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
						UpdateCurrentJumpCharge(MINJumpSpeed);
						break;
				}
			}
		}

		[UsedImplicitly]
		public void OnBoxPull(InputAction.CallbackContext context)
		{
			if (context.started) _pullObjects = true;
			if (context.canceled) _pullObjects = false;
		}
		#endregion

	}
}
