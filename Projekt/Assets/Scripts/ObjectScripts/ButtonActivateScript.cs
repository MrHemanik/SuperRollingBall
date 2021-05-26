using UnityEngine;

namespace ObjectScripts
{
    public class ButtonActivateScript : MonoBehaviour
    {
        public string colliderEntity = "Player";
        private bool _buttonPressed;
        private Animator _buttonAnimator;
        private ButtonMoveOnActivation _parentScript;
        
        public bool IsPressed()
        {
            return _buttonPressed;
        }
        
        private void Start()
        {
            _buttonAnimator = GetComponentInChildren<Animator>();
            _parentScript = GetComponentInParent<ButtonMoveOnActivation>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag(colliderEntity))
            {
                _buttonAnimator.SetBool("Pressed",true);
                _buttonPressed = true;
                _parentScript.TestIfPuzzleIsSolved();
                
                
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag(colliderEntity))
            {
                _buttonAnimator.SetBool("Pressed",false);
                _buttonPressed = false;
            }
        }
    }
}