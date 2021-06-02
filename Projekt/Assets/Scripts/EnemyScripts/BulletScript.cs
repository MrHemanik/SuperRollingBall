using UnityEngine;

namespace EnemyScripts
{
    public class BulletScript : MonoBehaviour
    {
        public float projectileSpeed;
        public float sniperRange;
        private Transform _ownTransform;

        private void Start()
        {
            _ownTransform = transform;
            Destroy(gameObject, sniperRange / projectileSpeed); //Zerstört die Kugel sobald sie außer Reichweite ist
       
        }

        // Update is called once per frame
        private void Update()
        {
            _ownTransform.position += _ownTransform.forward * (Time.deltaTime * projectileSpeed); //Schießt in richtung die er guckt
        }

        private void OnCollisionEnter(Collision other)
        {
            //Debug.Log("COLLISION mit: " +other.gameObject.name);
            Destroy(gameObject);
        }
    }
}
