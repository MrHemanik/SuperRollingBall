using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    public Vector2 zoomRange = new Vector2(4.0f, 12.0f); //Vector, der angibt, dass von Distanz 4(x) bis 12(y) gezoomt sein kann.
    public int currentZoom = 8; //Startwert 8;
    public int defaultFOV = 60;
    private bool _cutScene = true;
    private float _cutSceneDuration = 100.0f; // Wird in SetAnimation auf die richtige Zeit gesetzt
    private Vector3 _offset;
    private Rigidbody _playerRigidbody;
    private PlayerInput _playerInput;
    private Animator _animator;
    //private string[] _cameraModeList ={"Normal","TopDown"};
    private int _cameraMode;

    private Camera _camera;
    // Start is called before the first frame update
    private void Awake()
    {
        GameManager.StartListening("StartCameraAnimation", SetAnimation);
        GameManager.StartListening("EndCameraAnimation", CutSceneEnd);
        GameManager.StartListening("SkyboxColor", SetSkyboxColor);
        GameManager.StartListening("CameraModeNormal", NormalCameraMode);
        GameManager.StartListening("CameraModeTopDown", TopDownCameraMode);
    }
    private void OnDestroy()
    {
        GameManager.StopListening("StartCameraAnimation");
        GameManager.StopListening("EndCameraAnimation");
        GameManager.StopListening("SkyboxColor");
        GameManager.StopListening("CameraModeNormal");
        GameManager.StopListening("CameraModeTopDown");
    }
    
    private void Start()
    {
        _camera = GetComponent<Camera>();
        _offset = new Vector3(0.0f, currentZoom, -currentZoom);
        _playerRigidbody = player.GetComponent<Rigidbody>();
        _camera = GetComponent<Camera>();
        _playerInput = GetComponent<PlayerInput>();
        _animator = gameObject.transform.parent.GetComponent<Animator>();
        GameManager.TriggerEvent("FetchCurrentLevel"); //Aktiviert SetAnimation mit CurLevel & SetSkyboxColor mit Skybox[CurLevel]
    }
    
    private void LateUpdate() //LateUpdate für Updates die was anzeigen sollen, da die als letztes berechnet werden
    {
        if (!_cutScene)
        {
            //Position der Kamera
            transform.position = player.transform.position + _offset;
            //Field of View
            Vector3 ballVelocity = _playerRigidbody.velocity;
            //Da ich speed aus der RigidBody info nicht auslesen konnte, habe ich aus der Velocity die Speed berechnet (Vektorbetrag = Länge = Speed)
            float ballSpeed = Mathf.Sqrt(Mathf.Pow(ballVelocity.x, 2.0f) + Mathf.Pow(ballVelocity.y, 2.0f) +
                                         Mathf.Pow(ballVelocity.z, 2.0f));
            _camera.fieldOfView = defaultFOV + ballSpeed / 2;
        }
    }

    private void NormalCameraMode(string f)
    {
        _cameraMode = 0;
        _offset = new Vector3(0.0f, currentZoom, -currentZoom);
        gameObject.transform.rotation = Quaternion.Euler(45,0,0);
        Debug.Log("CameraMode:Normal");
        
    }

    private void TopDownCameraMode(string f)
    {

        _cameraMode = 1;
        _offset = new Vector3(0.0f, currentZoom*1.5f, 0.0f);
        gameObject.transform.rotation = Quaternion.Euler(90,0,0);
        Debug.Log("CameraMode:TopDown");
        
    }
    
    
    /* Event Methoden ------------------------------------------------------------------------------------------------*/
    private void SetAnimation(string input)
    {
        // Reminder: Die Animation muss im Animator hinzugefügt und die transition gesetzt sein!
        _animator.SetInteger(Animator.StringToHash("Level"),int.Parse(input));
        foreach (AnimationClip a in _animator.runtimeAnimatorController.animationClips)
        {
            if (a.name == input)
            {
                Animation(true);
                _cutSceneDuration = a.length;
                Debug.Log("Cutscene Duration:"+_cutSceneDuration);
                TimerManagerScript.StartTimer("EndCameraAnimation",_cutSceneDuration);
                return;
            }
        }
    }
    private void CutSceneEnd(string s)
    {
        Animation(false);
        GameManager.TriggerEvent("LevelTimerStart");
    }
    
    private void SetSkyboxColor(string color)
    {
        Debug.Log("Skybox-Farbe wird geladen: "+color);
        //Rechnet den Hexadeximal-Farbenwert zum Objekt Color um
        _camera.backgroundColor = new Color32(
            System.Convert.ToByte(color.Substring(0, 2),16),
            System.Convert.ToByte(color.Substring(2, 2),16),
            System.Convert.ToByte(color.Substring(4, 2),16),1
            );
    }

    /* Methoden ------------------------------------------------------------------------------------------------------*/
    private void Animation(bool active)
    {
        //Werte, die beim Start/Ende der Animation geändert werden müssen
        _cutScene = active;
        _playerRigidbody.isKinematic = active;
        _playerInput.enabled = !active;
    }
    /* Input Methoden -------------------------------------------------------------------------------------------------*/
    [UsedImplicitly]
    private void OnZoom(InputValue movementValue)
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
        if(_cameraMode == 0) _offset = new Vector3(0.0f, currentZoom, -currentZoom);
        else if (_cameraMode == 1) _offset = new Vector3(0.0f, currentZoom*1.5f, 0.0f);
        //Debug.Log(player.transform.position);
    }
}
