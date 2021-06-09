
using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class CameraController : MonoBehaviour
{
    private GameObject _player;
    public Vector2 zoomRange = new Vector2(2.0f, 12.0f); //Vector, der angibt, dass von Distanz 2(x) bis 12(y) gezoomt sein kann.
    public int currentZoom = 8; //Startwert 8;
    public int defaultFOV = 60;
    private bool _cutScene = true;
    private bool _levelStartCutScene = true;
    private float _cutSceneDuration = 100.0f; // Wird in SetAnimation auf die richtige Zeit gesetzt
    
    private Rigidbody _playerRigidbody;
    private PlayerInput _playerInput;
    private Animator _animator;
    private int _cameraMode; //0 Normal; 1 TopDown
    private Camera _camera;
    //CameraMovement: Rotation
    private const float RotateSpeed = 5;            // RotSpeed für Maus
    private const float ButtonRotateSensitivity = 4;// RotSpeed für Buttons
    private const float CameraSmoothness = 0.5f;
        //Die Kamera-Höhen sind relativ zu einer Kugel mit R = Wurzel(2), so ist Wurzel(2) die Spitze der Kugel und -Wurzel(2) der tiefste Punkt.
    private const float MINCameraHeight = 0.1f;     // Minimal Y Höhe des Kugelorbits
    private const float MAXCameraHeight = 1.4f;     // Maximal Y Höhe des Kugelorbits, muss < Wurzel(2) bzw radius sein
    //CameraMovementInputVariablen
    private Vector2 _mRotation;         // MouseRotation
    private float _buttonXRotate;       // Button XZ-Plane-Rotation Input
    private float _buttonYRotate;       // Button Y-Rotation Input
    private bool _fixedCamera = true;   // Kamera ist fest; Kamera ist per Maus bewegbar.
    private bool _xRotation = true;     // X-Rotation ist fest
    private bool _yRotation;            // Y-Rotation ist fest
    
    private Vector3 _fixedOffset;                                // fixed Offset bestimmt vom currentZoom, z.B: [0,8,-8]
    private Vector3 _offsetRotation = new Vector3(0,1,-1);  // KameraOffset, eigentliche Kameraposition relativ zum Player
    
    private readonly Quaternion _newQuaternion = Quaternion.Euler(0,0,0); //Quaternion mit keiner Rotation
    private readonly bool[] _cameraUpDownMovement = new bool[2]; //Das erste für Up, das zweite für Down
    


    // Start is called before the first frame update
    private void Awake()
    {
        GameManager.StartListening("StartCameraAnimation", SetAnimation);
        GameManager.StartListening("EndCameraAnimation", CutSceneEnd);
        GameManager.StartListening("SkyboxColor", SetSkyboxColor);
        GameManager.StartListening("ResetCamera", ResetCamera);
        GameManager.StartListening("CameraModeNormal", NormalCameraMode);
        GameManager.StartListening("CameraModeTopDown", TopDownCameraMode);
        GameManager.StartListening("Puzzle1Solved", StartPuzzle1SolvedCameraAnimation);
    }
    private void OnDestroy()
    {
        GameManager.StopListening("StartCameraAnimation");
        GameManager.StopListening("EndCameraAnimation");
        GameManager.StopListening("SkyboxColor");
        GameManager.StopListening("ResetCamera");
        GameManager.StopListening("CameraModeNormal");
        GameManager.StopListening("CameraModeTopDown");
        GameManager.StopListening("Puzzle1Solved");
    }
    
    private void Start()
    {
        _camera = GetComponent<Camera>();
        _fixedOffset = new Vector3(0.0f, currentZoom, -currentZoom);
        _player = transform.parent.gameObject;
        _playerRigidbody = _player.GetComponent<Rigidbody>();
        _camera = GetComponent<Camera>();
        _playerInput = GetComponent<PlayerInput>();
        _animator = gameObject.GetComponent<Animator>();
        transform.parent = null; //Detached sich, da es sonst zu komischen transforms kommt (bei Kugelbewegung kommt er mit der rotation nicht klar und wackelt trotz gegenrotation)
        GameManager.TriggerEvent("FetchCurrentLevel"); //Aktiviert SetAnimation mit CurLevel & SetSkyboxColor mit Skybox[CurLevel]
    }
    
    private void FixedUpdate() //Eigentlich LateUpdate für Updates die was anzeigen sollen, da die als letztes berechnet werden, jedoch wird die Camera Transformed, was in Fixed gehört (sonst entsteht shutter)
    {
        
        if (!_cutScene)
        {
            //Position der Kamera
            var playerTransform = _player.transform;
            var playerPosition = playerTransform.position;
            if (!_fixedCamera)
            {
                //https://forum.unity.com/threads/free-look-with-new-input-system.676873/
                //Ich habe viel durch StackOverflow gelernt, jedoch habe ich dort nur die Befehle wie AngleAxis oder Slerp gelernt und habe daraus die Kamerabewegung erstellt
                
                //input einlesen
                float x = _mRotation.x;
                float y = _mRotation.y;
                _mRotation = Vector2.zero; //Setzt den Input wieder zurück
                
                //Berechnung ob Input akzeptiert wird;
                if (_cameraUpDownMovement[0] && y > 0) y = 0; //Falls man nicht nach oben bewegen darf, der Input aber nach oben geht, wird der Input gelöscht
                if (_cameraUpDownMovement[1] && y < 0) y = 0; // -||-
                if (_cameraUpDownMovement[0] && _buttonYRotate > 0) _buttonYRotate = 0;
                if (_cameraUpDownMovement[1] && _buttonYRotate < 0) _buttonYRotate = 0;

                //Berechnung der Rotation
                Quaternion xMovement = _newQuaternion;
                if(_xRotation) //Falls X-Rotation aktiviert ist
                    xMovement = Quaternion.AngleAxis(x+ _buttonXRotate* RotateSpeed/10, Vector3.up);
                Quaternion yMovement = _newQuaternion;
                if (_yRotation)
                    //Falls Y-Rotation aktiviert ist
                    yMovement = Quaternion.AngleAxis(y + _buttonYRotate * RotateSpeed / 10, 
                        transform.right // Orthogonaler Vektor zu forward, was die lookat direction zum Ball ist.
                        );
                Quaternion movement = xMovement * yMovement;
                
                _offsetRotation = movement* _offsetRotation;
                //Erkärung zum Orbitsbereich: https://i.imgur.com/uytrurW.png *Outdated*
                _cameraUpDownMovement[0] = (_offsetRotation.y >= MAXCameraHeight); //Muss false sein, bei true darf nicht bewegt werden
                _cameraUpDownMovement[1] = (_offsetRotation.y <= MINCameraHeight);
                Vector3 offset = currentZoom *(_offsetRotation);

                Vector3 newPosition = playerPosition + offset;
                transform.position = Vector3.Slerp(transform.position, newPosition, CameraSmoothness);
                
            }
            else
            {
                transform.position = playerPosition + _fixedOffset;
            }
            //Rotation der Kamera
            gameObject.transform.LookAt(playerTransform);
            //Field of View
            Vector3 ballVelocity = _playerRigidbody.velocity;
            //Da ich speed aus der RigidBody info nicht auslesen konnte, habe ich aus der Velocity die Speed berechnet (Vektorbetrag = Länge = Speed)
            float ballSpeed = Mathf.Sqrt(Mathf.Pow(ballVelocity.x, 2.0f) + Mathf.Pow(ballVelocity.y, 2.0f) +
                                         Mathf.Pow(ballVelocity.z, 2.0f));
            _camera.fieldOfView = defaultFOV + ballSpeed / 2;
        }
    }

    /* Event Methoden ------------------------------------------------------------------------------------------------*/
    #region EventMethoden
    private void NormalCameraMode(string f)
    {
        _cameraMode = 0;
        _fixedOffset = new Vector3(0.0f, currentZoom, -currentZoom);
        Debug.Log("CameraMode:Normal");
        
    }

    private void TopDownCameraMode(string f)
    {
        _cameraMode = 1;
        _fixedOffset = new Vector3(0.0f, currentZoom*1.5f, -0.1f);
        Debug.Log("CameraMode:TopDown");
    }
    private void SetAnimation(string input)
    {
        Animation(true);
        // Reminder: Die Animation muss im Animator hinzugefügt und die transition gesetzt sein!
        _animator.SetInteger(Animator.StringToHash("Level"),int.Parse(input));
        foreach (AnimationClip a in _animator.runtimeAnimatorController.animationClips)
        {
            if (a.name == input)
            {
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
        if (_levelStartCutScene)
        {
            GameManager.TriggerEvent("LevelTimerStart");
            _levelStartCutScene = false;
        }
    }
    
    private void SetSkyboxColor(string color)
    {
        Debug.Log("Skybox-Farbe wird geladen: "+color);
        //Rechnet den Hexadeximal-Farbenwert zum Objekt Color um
        _camera.backgroundColor = new Color32(
            Convert.ToByte(color.Substring(0, 2),16),
            Convert.ToByte(color.Substring(2, 2),16),
            Convert.ToByte(color.Substring(4, 2),16),1
            );
    }

    private void ResetCamera(string s)
    {
        _offsetRotation = new Vector3(1,1,1);
    }

    private void StartPuzzle1SolvedCameraAnimation(string s)
    {
        Animation(true);
        _animator.SetTrigger(Animator.StringToHash("Puzzle1Solved"));
        _cutSceneDuration =3.30f; //Idealerweise wie bei SetAnimation die CutSceneDuration ermitteln, aber das wäre zu viel rechenarbeit, wenn man auch einfach die Fixzahl eingeben kann
        TimerManagerScript.StartTimer("EndCameraAnimation",_cutSceneDuration);
    }
    #endregion
    /* Methoden ------------------------------------------------------------------------------------------------------*/
    #region Methoden
    private void Animation(bool active)
    {
        //Werte, die beim Start/Ende der Animation geändert werden müssen
        _animator.enabled = active;
        _cutScene = active;
        _playerRigidbody.isKinematic = active;
        _playerInput.enabled = !active;
        var transform1 = transform;
        transform1.position = new Vector3();
        transform1.rotation = new Quaternion();
        Debug.Log("Kamera Reset");
    }
    
    
    #endregion
    /* Input Methoden -------------------------------------------------------------------------------------------------*/
    #region InputSystem
    [UsedImplicitly]
    public void OnZoom(InputAction.CallbackContext movementValue)
    {
        if (!movementValue.started) return; //soll nur beim "Start" trigger ausgelöst werden, sonst wird pro scroll 3-Mal gescrollt (start,perform,canceled)
        Vector2 zoomVector = movementValue.ReadValue<Vector2>();
        if (zoomVector.y < 0)
        {
            //Reinzoomen
            if (currentZoom < zoomRange.y)
            {
                currentZoom++;
            }
        }
        else
        {
            //Rauszoomen
            if (currentZoom > zoomRange.x)
            {
                currentZoom--;
            }
        }
        //Neuberechnung des Offsets
        if(_cameraMode == 0) _fixedOffset = new Vector3(0.0f, currentZoom, -currentZoom);
        else if (_cameraMode == 1) _fixedOffset = new Vector3(0.0f, currentZoom*1.5f, -0.1f);

    }
    public void OnLook(InputAction.CallbackContext rotateValue) // Bei Mausbewegung
    {
        if (!rotateValue.started) return;
        _mRotation = rotateValue.ReadValue<Vector2>();
    }

    public void RotateXButton(InputAction.CallbackContext context)
    {
        _buttonXRotate = ButtonRotateSensitivity*context.ReadValue<float>();
    }
    public void RotateYButton(InputAction.CallbackContext context)
    {
        _buttonYRotate = ButtonRotateSensitivity*context.ReadValue<float>();
    }
    public void ToggleFixedCamera(InputAction.CallbackContext context) //Beim Drücken von C
    {
        _fixedCamera = !_fixedCamera;
        _offsetRotation = new Vector3(0,1,-1);
        GameManager.TriggerEvent("UpdateFixedDisplay", _fixedCamera.ToString());
    }
    public void ToggleXRotation(InputAction.CallbackContext context) //Beim Drücken von V
    {
        _xRotation = !_xRotation;
        GameManager.TriggerEvent("UpdateXAxisDisplay", _xRotation.ToString());
    }public void ToggleYRotation(InputAction.CallbackContext context) //Beim Drücken von B
    {
        _yRotation = !_yRotation;
        GameManager.TriggerEvent("UpdateYAxisDisplay", _yRotation.ToString());
    }
    #endregion
}
