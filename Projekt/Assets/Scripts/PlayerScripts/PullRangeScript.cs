using System;
using System.Linq;
using UnityEngine;

namespace PlayerScripts
{
    public class PullRangeScript : MonoBehaviour
    {
        //Skript welches Objekte mit Tag die in im pullableObjectTags array stehen zur Liste hinzufügt, sobald eins in der Range ist.
        public string[] pullableObjectTags;
        private PlayerController pc;

        private void Start()
        {
            pc = transform.parent.GetComponent<PlayerController>();
        }

        private void OnTriggerEnter(Collider other)
        {
            foreach (var pullTag in pullableObjectTags)
            {
                if (other.gameObject.CompareTag(pullTag))
                {
                    AddToObjects(other.gameObject);
                }
            }
        }private void OnTriggerExit(Collider other)
        {
            foreach (var pullTag in pullableObjectTags)
            {
                if (other.gameObject.CompareTag(pullTag))
                {
                    RemoveFromObjects(other.gameObject);
                }
            }
        }

        private void AddToObjects(GameObject pullObject)
        {
            pc.pullableObjectsInRange.Add(pullObject);
        }

        private void RemoveFromObjects(GameObject pullObject)
        {
            pc.pullableObjectsInRange.Remove(pullObject);
        }
    }
}