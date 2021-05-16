using System;
using TMPro;
using UnityEngine;

namespace ObjectScripts
{
    public class WalljumpScript : MonoBehaviour
    {
        public GameObject walljumpPopup;
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Instantiate(walljumpPopup, new Vector3(), new Quaternion());
                Destroy(gameObject,1);
            }
        }
    }
}
