using UnityEngine;
using UnityEngine.AI;

namespace EnemyScripts
{
    public class BasicEnemyScript : MonoBehaviour
    {
        private NavMeshAgent _nav;

        public bool alive = true;
        private void Start()
        {
            _nav = gameObject.GetComponent<NavMeshAgent>();
        }


        private void OnTriggerStay(Collider other)
        {
            if(other.CompareTag("Player")&&alive){
                //Gibt ohne 'alive' immer einen Error wenn der Enemy zerstört wird, da SetDestination länger zum verarbeiten braucht.
                _nav.SetDestination(other.transform.position);
            }
        }
    }
}
