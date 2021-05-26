using System;
using UnityEngine;

namespace ObjectScripts.Buttons
{
    public class ButtonResetScript : MonoBehaviour
    {
        public GameObject[] _resetObjects;
        private Animator _buttonAnimator;
        

        private void Start()
        {
            _buttonAnimator = GetComponentInChildren<Animator>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                _buttonAnimator.SetBool("Pressed",true);
                foreach (var respawnObject in _resetObjects)
                {
                    respawnObject.GetComponent<RespawnObjectScript>().Respawn();
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                _buttonAnimator.SetBool("Pressed",false);
            }
        }
    }
}
