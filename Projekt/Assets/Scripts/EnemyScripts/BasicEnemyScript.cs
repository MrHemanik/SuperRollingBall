using System;
using UnityEngine;
using UnityEngine.AI;

namespace EnemyScripts
{
    public class BasicEnemyScript : MonoBehaviour
    {
        private NavMeshAgent nav;
        // Start is called before the first frame update
        void Start()
        {
            nav = gameObject.GetComponent<NavMeshAgent>();
        }


        private void OnTriggerStay(Collider other)
        {
            if(other.CompareTag("Player")){
                nav.SetDestination(other.transform.position);
            }
        }
    }
}
