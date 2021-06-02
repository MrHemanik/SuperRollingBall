using UnityEngine;
using UnityEngine.AI;

namespace EnemyScripts
{
    public class BasicEnemyScript : MonoBehaviour
    {
        private NavMeshAgent _nav;

        public bool alive = true;
        // Start is called before the first frame update
        private void Start()
        {
            _nav = gameObject.GetComponent<NavMeshAgent>();
        }


        private void OnTriggerStay(Collider other)
        {
            if(other.CompareTag("Player")&&alive){
                _nav.SetDestination(other.transform.position); //Gibt immer einen Error wenn der Enemy zerstört wird, da SetDestination länger zum verarbeiten braucht.
            }
        }
    }
}
