using PlayerScripts;
using UnityEngine;

namespace ManageObjectScripts
{
    public class StatusEffectPickupScript : MonoBehaviour
    {
        public float duration;
        public float multiplier;
        public GameObject statusPrefab;


        // Update is called once per frame
        private void CreateStatusEffect(GameObject player)
        {
            //Kein Instantiate, da so sonst 2 GameObjects entstehen
            GameObject statusObject = Instantiate(this.statusPrefab, player.transform.position, new Quaternion(),
                player.transform.Find("StatusEffects").transform);
            statusObject.GetComponent<StatusEffectScript>().Initialize(duration, multiplier,
                player.GetComponent<PlayerController>(), gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                CreateStatusEffect(other.gameObject);
                gameObject.SetActive(false);
            }
        }
    }
}