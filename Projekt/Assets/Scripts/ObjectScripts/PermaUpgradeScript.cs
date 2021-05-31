using UnityEngine;

namespace ObjectScripts
{
    public class PermaUpgradeScript : MonoBehaviour
    {
        public int id;
        public GameObject collectedPopup;
        private GameObject _parent;
        private void Start()
        {
            _parent = transform.parent.gameObject;
            _parent.SetActive(!GameManager.GetIsPermaUpgradeCollected(id));
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            _parent.SetActive(false);
            GameManager.TriggerEvent("PermaUpgradeCollected", id.ToString());
            Instantiate(collectedPopup, new Vector3(), new Quaternion()); //Pop-Up Screen
            
        }
    }
}
