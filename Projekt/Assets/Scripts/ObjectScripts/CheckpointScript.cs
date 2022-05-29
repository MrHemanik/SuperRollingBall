using PlayerScripts;
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
                Destroy(gameObject.GetComponent<MeshCollider>()); //Zerstört den Trigger
                other.gameObject.GetComponent<PlayerController>().lastCheckPoint =
                    gameObject.transform.position; //Setzt checkpoint
                _particleRing.enabled = false; //Deaktiviert die Partikel
                Instantiate(checkpointPopup, new Vector3(), new Quaternion()); //Pop-Up Screen
                Destroy(gameObject.transform.parent.gameObject,
                    3); //Warte 3 Sekunden, bis es ganz gelöscht wird (Dann ist die Partikelanimation vorbei)
            }
        }
    }
}