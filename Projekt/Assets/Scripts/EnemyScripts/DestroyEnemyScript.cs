using UnityEngine;

namespace EnemyScripts
{
    public class DestroyEnemyScript : MonoBehaviour
    {
        public bool alive = true;
        public GameObject deathCloudPrefab;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Destroy(gameObject.transform.parent.gameObject, 0.2f);
                Instantiate(deathCloudPrefab, transform.position, new Quaternion());
                var rb = other.GetComponent<Rigidbody>();
                var rbVelocity = rb.velocity;
                rb.velocity =
                    new Vector3(rbVelocity.x, -rbVelocity.y,
                        rbVelocity.x); //"Bounce" vom Objekt, invertiert die Geschwindigkeit der Y-Achse
                BasicEnemyScript bes = transform.parent.GetComponent<BasicEnemyScript>();
                if (bes != null) bes.alive = false;
            }
        }
    }
}