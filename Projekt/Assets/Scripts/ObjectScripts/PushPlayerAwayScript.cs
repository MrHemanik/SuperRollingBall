using UnityEngine;

namespace ObjectScripts
{
    public class PushPlayerAwayScript : MonoBehaviour
    {
        private Vector3 _tp; //Da sich die Zone nicht bewegen kann, kanns als variable 1-Mal zugewiesen werden
        private Vector3 _bounceDirection;

        private void Start()
        {
            _tp = transform.position;
        }

        private void OnTriggerEnter(Collider other)
        {
            _bounceDirection = other.ClosestPoint(_tp) - _tp;
            _bounceDirection.y = 0; //Soll nicht nach oben/unten drücken
        }

        private void OnTriggerStay(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            other.attachedRigidbody.AddForce(_bounceDirection * 100);
        }
    }
}