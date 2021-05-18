using UnityEngine;

namespace ObjectScripts
{
    public class CheckpointScript : MonoBehaviour
    {
        private ParticleSystem.EmissionModule _particleRing;

        public GameObject checkpointPopup;
        // Start is called before the first frame update
        void Start()
        {
            _particleRing = gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().emission;
        }

        // Update is called once per frame
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.gameObject.GetComponent<PlayerController>().lastCheckPoint = gameObject.transform.position;
                _particleRing.enabled = false;
                Instantiate(checkpointPopup, new Vector3(), new Quaternion());
                Destroy(gameObject.transform.parent.gameObject,3);
            }
        }
    }
}
