using System.Numerics;
using JetBrains.Annotations;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class CameraController : MonoBehaviour
{
    private GameObject _player;
    public Vector2 zoomRange = new Vector2(4.0f, 12.0f); //Vector, der angibt, dass von Distanz 4(x) bis 12(y) gezoomt sein kann.
    public int currentZoom = 8; //Startwert 8;
    public int defaultFOV = 60;
    public bool _cutScene = true; //TODO: Wieder private machen, debug
    private bool _levelStartCutScene = true;
    private float _cutSceneDuration = 100.0f; // Wird in SetAnimation auf die richtige Zeit gesetzt
    public Vector3 _offset;
    private Rigidbody _playerRigidbody;
    private PlayerInput _playerInput;
    private Animator _animator;
    //private string[] _cameraModeList ={"Normal","TopDown"};
    private int _cameraMode;
    private Camera _camera;
    //CameraMovement: Rotation
    public float rotateSpeed = 5;
    private Vector2 _mRotation;
    private bool _fixedCamera = false; // Kamera ist fest; Kamera ist per Maus bewegbar.
    private float _cameraSmoothness = 0.5f;
    public Vector3 _curOffset;
    
    



    // Start is called before the first frame update
    private void Awake()
    {
        GameManager.StartListening("StartCameraAnimation", SetAnimation);
        GameManager.StartListening("EndCameraAnimation", CutSceneEnd);
        GameManager.StartListening("SkyboxColor", SetSkyboxColor);
        GameManager.StartListening("CameraModeNormal", NormalCameraMode);
        GameManager.StartListening("CameraModeTopDown", TopDownCameraMode);
        GameManager.StartListening("Puzzle1Solved", StartPuzzle1SolvedCameraAnimation);
    }
    private void OnDestroy()
    {
        GameManager.StopListening("StartCameraAnimation");
        GameManager.StopListening("EndCameraAnimation");
        GameManager.StopListening("SkyboxColor");
        GameManager.StopListening("CameraModeNormal");
        GameManager.StopListening("CameraModeTopDown");
        GameManager.StopListening("Puzzle1Solved");
    }
    
    private void Start()
    {
        _camera = GetComponent<Camera>();
        _offset = new Vector3(0.0f, currentZoom, -currentZoom);
        _curOffset = _offset;
        _player = transform.parent.gameObject;
        _playerRigidbody = _player.GetComponent<Rigidbody>();
        _camera = GetComponent<Camera>();
        _playerInput = GetComponent<PlayerInput>();
        _animator = gameObject.GetComponent<Animator>();
        transform.parent = null; //Detached sich, da es sonst zu komischen transforms kommt (bei Kugelbewegung kommt er mit der rotation nicht klar und wackelt trotz gegenrotation)
        GameManager.TriggerEvent("FetchCurrentLevel"); //Aktiviert SetAnimation mit CurLevel & SetSkyboxColor mit Skybox[CurLevel]
    }
    
    private void LateUpdate() //LateUpdate für Updates die was anzeigen sollen, da die als letztes berechnet werden
    {
        //https://forum.unity.com/threads/free-look-with-new-input-system.676873/
        if (!_cutScene)
        {
            //Position der Kamera
            var playerTransform = _player.transform;
            var playerPosition = playerTransform.position;
            if (!_fixedCamera)
            {
                
                Quaternion camTurnAngle = Quaternion.AngleAxis(_mRotation.x * rotateSpeed/10, Vector3.up);
                _mRotation.x = 0;
                _curOffset = camTurnAngle * _curOffset;
                //_curOffset.Normalize();
                //_curOffset = _curOffset * (1 / 8);
                //Debug.Log("OffsetLength: "+_curOffset.magnitude);
            }
            Vector3 newPosition = playerPosition + _curOffset;
            transform.position = Vector3.Slerp(transform.position, newPosition, _cameraSmoothness);
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
        _offset = new Vector3(0.0f, currentZoom, -currentZoom);
        Debug.Log("CameraMode:Normal");
        
    }

    private void TopDownCameraMode(string f)
    {
        _cameraMode = 1;
        _offset = new Vector3(0.0f, currentZoom*1.5f, 0.0f);
        Debug.Log("CameraMode:TopDown");
    }
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
            System.Convert.ToByte(color.Substring(0, 2),16),
            System.Convert.ToByte(color.Substring(2, 2),16),
            System.Convert.ToByte(color.Substring(4, 2),16),1
            );
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
        var _oldOffset = _offset;
        if(_cameraMode == 0) _offset = new Vector3(0.0f, currentZoom, -currentZoom);
        else if (_cameraMode == 1) _offset = new Vector3(0.0f, currentZoom*1.5f, 0.0f);
        //_curOffset += _offset;
        //Debug.Log(player.transform.position);
    }
    public void OnLook(InputAction.CallbackContext rotateValue)
    {
        if (!rotateValue.started) return;
        _mRotation = rotateValue.ReadValue<Vector2>();
    }
    #endregion
}
