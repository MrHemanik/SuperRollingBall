using UnityEngine;

namespace EnemyScripts
{
    public class SniperScript : MonoBehaviour
    {
        public float timeBetweenBullets; //Zeit zwischen schüssen
        public float projectileSpeed; //Kugelgeschwindigkeit
        public float range;
        public GameObject bulletPrefab;
        private Transform _scope;
        private float _timeTilNextShot;

        private void Start()
        {
            _scope = gameObject.transform.GetChild(0).gameObject.transform;
            GetComponent<SphereCollider>().radius = range;

        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _timeTilNextShot = timeBetweenBullets;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log(transform.position-other.gameObject.transform.position);
                _scope.LookAt(other.transform);
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
