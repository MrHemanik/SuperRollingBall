using UnityEngine;

namespace ObjectScripts
{
    public class RotateHammerScript : MonoBehaviour
    {
        private void OnCollisionEnter(Collision other) //Bei Berührung eines Objektes (Kollision)
        {
            if (other.gameObject.CompareTag("Player")) //Berührung mit Boden
            {
                var tf = transform;
                other.gameObject.GetComponent<Rigidbody>()
                    .AddForce(10000 * (tf.GetChild(0).position - tf.position));
            }
        }
    }
}