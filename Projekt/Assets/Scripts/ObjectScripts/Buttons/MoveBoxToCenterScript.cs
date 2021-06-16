using System;
using Unity.Mathematics;
using UnityEngine;

namespace ObjectScripts.Buttons
{
    public class MoveBoxToCenterScript : MonoBehaviour
    {
        //Zusatzskript für ButtonActivateScript was bei ButtonAlt Prefabs benutzt wird um die Box in die Mitte zu ziehen
        private string _colliderEntity;
        private GameObject _objectToMove;
        private bool _moveObject;
        private static readonly Quaternion zeroQuaternion = Quaternion.Euler(0,0,0);
        private void Start()
        {
            _colliderEntity = gameObject.GetComponent<ButtonActivateScript>().colliderEntity;
            
        }
        private void Update()
        {
            if (!_moveObject) return;
            _objectToMove.transform.position = Vector3.Lerp(_objectToMove.transform.position, transform.position, 0.005f);
            _objectToMove.transform.rotation = Quaternion.Slerp(_objectToMove.transform.rotation, zeroQuaternion, 0.005f);
            if ((_objectToMove.transform.position - transform.position).sqrMagnitude < 0.001) _moveObject = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag(_colliderEntity))
            {
                _objectToMove = other.gameObject;
                _moveObject = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag(_colliderEntity))
            {
                _moveObject = false;
                _objectToMove = null;
            }
        }
    }
}
