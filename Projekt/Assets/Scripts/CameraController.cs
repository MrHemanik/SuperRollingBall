using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class CameraController : MonoBehaviour
{
    public GameObject player;
    public Vector2 zoomRange = new Vector2(4.0f, 12.0f); //Vector, der angibt, dass von Distanz 4(x) bis 12(y) gezoomt sein kann.
    public int currentZoom = 8; //Startwert 8;
    public int defaultFOV = 60;
    private bool _cutScene = true;
    private float _cutSceneDuration = 4.0f;
    private Vector3 _offset;
    private Rigidbody _playerRigidbody;
    private Animator _animator;

    private Camera _camera;
    // Start is called before the first frame update
    private void Start()
    {
        _offset = new Vector3(0.0f, currentZoom, -currentZoom);
        _playerRigidbody = player.GetComponent<Rigidbody>();
        _camera = GetComponent<Camera>();
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void OnZoom(InputValue movementValue)
    {
        Vector2 zoomVector = movementValue.Get<Vector2>();
        if (zoomVector.y < 0)
        {
            //Reinzoomen
            if (currentZoom <= zoomRange.y)
            {
                currentZoom++;
            }
        }
        else
        {
            //Rauszoomen
            if (currentZoom >= zoomRange.x)
            {
                currentZoom--;
            }
        }
        //Neuberechnung des Offsets
        _offset = new Vector3(0.0f, currentZoom, -currentZoom);
        //Debug.Log(player.transform.position);
    }

    private void LateUpdate() //LateUpdate für Updates die was anzeigen sollen, da die als letztes berechnet werden
    {
        if (_cutScene)
        {
            //Kameramovement - wird von Animation geregelt
            _cutSceneDuration -= Time.deltaTime;
            if (_cutSceneDuration < 0)
            {
                _cutScene = false;
                _playerRigidbody.isKinematic = false;
                _animator.enabled = false; //SUPER unschöne Lösung, aber ich lebe erstmal damit.
            }
        }
        else
        {
            //Position der Kamera
            transform.position = player.transform.position + _offset;

            //Field Of View (To Be Removed - sieht nicht so schön aus)
            Vector3 ballVelocity = _playerRigidbody.velocity;
            //Da ich speed aus der RigidBody info nicht auslesen konnte, habe ich aus der Velocity die Speed berechnet (Vektorbetrag = Länge = Speed)
            float ballSpeed = Mathf.Sqrt(Mathf.Pow(ballVelocity.x, 2.0f) + Mathf.Pow(ballVelocity.y, 2.0f) +
                                         Mathf.Pow(ballVelocity.z, 2.0f));
            _camera.fieldOfView =
                defaultFOV + ballSpeed / 2;
        }
    }
}
