using UnityEngine;
using UnityEngine.AI;

namespace EnemyScripts
{
    public class BasicEnemyScript : MonoBehaviour
    {
        private NavMeshAgent _nav;
        // Start is called before the first frame update
        void Start()
        {
            _nav = gameObject.GetComponent<NavMeshAgent>();
        }


        private void OnTriggerStay(Collider other)
        {
            if(other.CompareTag("Player")){
                _nav.SetDestination(other.transform.position);
            }
        }
    }
}
