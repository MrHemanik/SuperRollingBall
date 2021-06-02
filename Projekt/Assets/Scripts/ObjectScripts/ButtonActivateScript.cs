using UnityEngine;

namespace ObjectScripts
{
    public class ButtonActivateScript : MonoBehaviour
    {
        public string colliderEntity = "Player";
        private bool _buttonPressed;
        private Animator _buttonAnimator;
        private ButtonMoveOnActivation _parentScript;
        private Material _defaultMaterial;
        private Color _defaultColor;
        private Color _darkenedColor;
        private const float ColorDarkenFactor = 0.80f;
        private static readonly int Pressed = Animator.StringToHash("Pressed");

        public bool IsPressed()
        {
            return _buttonPressed;
        }
        
        private void Start()
        {
            _buttonAnimator = GetComponentInChildren<Animator>();
            _parentScript = GetComponentInParent<ButtonMoveOnActivation>();
            _defaultMaterial = transform.GetChild(0).gameObject.GetComponent<Renderer>().material;
            _defaultColor = _defaultMaterial.color;
            _darkenedColor = new Color(
                _defaultColor.r * ColorDarkenFactor, 
                _defaultColor.g * ColorDarkenFactor,
                _defaultColor.b * ColorDarkenFactor);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag(colliderEntity))
            {
                _buttonAnimator.SetBool(Pressed,true);
                _buttonPressed = true;
                _defaultMaterial.color = _darkenedColor;
                _parentScript.TestIfPuzzleIsSolved();
                
                
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag(colliderEntity))
            {
                _buttonAnimator.SetBool(Pressed,false);
                _defaultMaterial.color = _defaultColor;
                _buttonPressed = false;
            }
        }
    }
}