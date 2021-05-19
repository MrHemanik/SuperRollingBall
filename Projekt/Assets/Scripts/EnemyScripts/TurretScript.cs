using System;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

namespace EnemyScripts
{
    public class TurretScript : MonoBehaviour
    {
        public float timeBetweenBullets; //Zeit zwischen schüssen
        public float projectileSpeed; //Kugelgeschwindigkeit
        public float range;
        public float rotationSpeed;
        public GameObject bulletPrefab;
        private Transform _scope;
        private float _timeTilNextShot;
        public bool resetAim = false;
        private Quaternion defaultRotation;

        private void Start()
        {
            _scope = gameObject.transform.GetChild(0).gameObject.transform;
            defaultRotation = _scope.rotation;
            GetComponent<SphereCollider>().radius = range;

        }

        private void Update()
        {
            if (resetAim)
            {
                _scope.rotation = Quaternion.Slerp(_scope.rotation, defaultRotation, rotationSpeed * Time.deltaTime);
                if (Quaternion.Angle(_scope.rotation,defaultRotation) < 0.01f) resetAim = false;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _timeTilNextShot = timeBetweenBullets;
                resetAim = false;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                // Fremdcode: Berechnung eines langsamen LookAt: https://answers.unity.com/questions/166666/slow-lookat.html
                Vector3 direction =  other.transform.position-_scope.position;
                Quaternion toRotation = Quaternion.LookRotation(direction);
                _scope.rotation = Quaternion.Slerp(_scope.rotation, toRotation, rotationSpeed * Time.deltaTime);
                //Fremdcode ENDE
                
                _timeTilNextShot -= Time.deltaTime;
                if (_timeTilNextShot <= 0)
                {
                    ShootBullet();
                    _timeTilNextShot = timeBetweenBullets;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                resetAim = true;
            }
        }

        private void ShootBullet()
        {
            BulletScript bulletScript = Instantiate(bulletPrefab, _scope.transform.GetChild(0).position, _scope.rotation, gameObject.transform).GetComponent<BulletScript>();
            bulletScript.projectileSpeed = projectileSpeed;
            bulletScript.sniperRange = range-1.5f; // Minus 1,5, da das Scope so lang ist

        }
    }
}
