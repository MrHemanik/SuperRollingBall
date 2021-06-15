using System;
using UnityEngine;

namespace ObjectScripts
{
    public class WalkThroughWall : MonoBehaviour
    {
        public Material transparentMaterial;
        public Material _normalMaterial;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            Renderer r = gameObject.GetComponent<Renderer>();
            _normalMaterial = r.material;
            r.material = transparentMaterial;
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            Renderer r = gameObject.GetComponent<Renderer>();
            r.material = _normalMaterial;
        }
    }
}
