using UnityEngine;

namespace PlayerScripts
{
    public class PullRangeScript : MonoBehaviour
    {
        //Skript welches Objekte mit Tag die in im pullableObjectTags array stehen zur Liste hinzufügt, sobald eins in der Range ist.
        public string[] pullableObjectTags;
        private PlayerController _pc;

        private void Start()
        {
            _pc = transform.parent.GetComponent<PlayerController>();
        }

        private void OnTriggerEnter(Collider other)
        {
            foreach (var pullTag in pullableObjectTags)
            {
                if (other.gameObject.CompareTag(pullTag))
                {
                    AddToObjects(other.gameObject.GetComponent<Rigidbody>());
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            foreach (var pullTag in pullableObjectTags)
            {
                if (other.gameObject.CompareTag(pullTag))
                {
                    RemoveFromObjects(other.gameObject.GetComponent<Rigidbody>());
                }
            }
        }

        private void AddToObjects(Rigidbody pullObject)
        {
            _pc.pullableObjectsInRange.Add(pullObject);
        }

        private void RemoveFromObjects(Rigidbody pullObject)
        {
            _pc.pullableObjectsInRange.Remove(pullObject);
        }
    }
}