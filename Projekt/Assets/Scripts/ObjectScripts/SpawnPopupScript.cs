using UnityEngine;

namespace ObjectScripts
{
    public class SpawnPopupScript : MonoBehaviour
    {
        public GameObject popup;
        public bool destroyTrigger = true; //Ob der Text nur 1-Mal auftauchen soll

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Instantiate(popup, new Vector3(), new Quaternion());
                if (destroyTrigger) Destroy(gameObject);
            }
        }
    }
}