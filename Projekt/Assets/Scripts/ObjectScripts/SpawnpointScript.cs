using PlayerScripts;
using UnityEngine;

namespace ObjectScripts
{
    public class SpawnpointScript : MonoBehaviour
    {

        public GameObject missionPopup;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.gameObject.GetComponent<PlayerController>().lastCheckPoint = gameObject.transform.position;
                Instantiate(missionPopup, new Vector3(), new Quaternion());
                Destroy(gameObject.transform.parent.gameObject,1);
            }
        }
        
    }
}
