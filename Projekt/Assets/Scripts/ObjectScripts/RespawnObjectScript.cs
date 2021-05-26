using System;
using UnityEngine;

namespace ObjectScripts
{
    public class RespawnObjectScript : MonoBehaviour
    {
        private Vector3 _respawnPosition;
        private Quaternion _respawnRotation;
        private Rigidbody _rb;
        private Transform _tf;
        public GameObject respawnEffect;
        void Start()
        {
            _tf = transform;
            _respawnPosition = _tf.position;
            _respawnRotation = _tf.rotation;
            _rb = gameObject.GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Death"))
            {
                Respawn();
            }
        }

        public void Respawn()
        {
            _tf.position = _respawnPosition;
            _tf.rotation = _respawnRotation;
            _rb.velocity = new Vector3(0, 0, 0);
            _rb.angularVelocity = new Vector3(0, 0, 0);
            Instantiate (respawnEffect, transform.position, new Quaternion()); //Partikelwolke
        }
    }
}