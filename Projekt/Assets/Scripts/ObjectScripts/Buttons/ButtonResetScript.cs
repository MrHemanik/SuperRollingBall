using System;
using UnityEngine;

namespace ObjectScripts.Buttons
{
    public class ButtonResetScript : MonoBehaviour
    {
        public GameObject[] resetObjects;
        private Animator _buttonAnimator;
        private Material _defaultMaterial;
        private Color _defaultColor;
        private Color _darkenedColor;
        private const float ColorDarkenFactor = 0.80f;
        private static readonly int Pressed = Animator.StringToHash("Pressed");


        private void Start()
        {
            _buttonAnimator = GetComponentInChildren<Animator>();
            _defaultMaterial = transform.GetChild(0).gameObject.GetComponent<Renderer>().material;
            _defaultColor = _defaultMaterial.color;
            _darkenedColor = new Color(
                _defaultColor.r * ColorDarkenFactor, 
                _defaultColor.g * ColorDarkenFactor,
                _defaultColor.b * ColorDarkenFactor);

        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                _buttonAnimator.SetBool(Pressed,true);
                _defaultMaterial.color = _darkenedColor;
                foreach (var respawnObject in resetObjects)
                {
                    respawnObject.GetComponent<RespawnObjectScript>().Respawn();
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                _buttonAnimator.SetBool(Pressed,false);
                _defaultMaterial.color = _defaultColor;
            }
        }
    }
}
