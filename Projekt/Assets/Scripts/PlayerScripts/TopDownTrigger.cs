using ManageObjectScripts;
using UnityEngine;

namespace PlayerScripts
{
    public class TopDownTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                GameManager.TriggerEvent("CameraModeTopDown");
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                GameManager.TriggerEvent("CameraModeNormal");
            }
        }
    }
}