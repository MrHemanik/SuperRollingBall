using UnityEngine;

namespace ObjectScripts
{
    public class MovePlayerWithPlattformScript : MonoBehaviour
    {
        //Bewegt den Player mit dem Objekt mit, solange Kollision herrscht.
        private GameObject _player;
        private bool _movePlayer;
        private Vector3 _speed;
        private Vector3 _lastPosition;

        private void FixedUpdate()
        {
            if (!_movePlayer) return;
            var position = transform.position;
            _speed = position - _lastPosition;
            var playerPosition = _player.transform.position;
            _player.transform.position = Vector3.Slerp(playerPosition, playerPosition + _speed, 0.5f);
            _lastPosition = position;
            ;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!other.gameObject.CompareTag("Player")) return;
            _player = other.gameObject;
            _lastPosition = transform.position; //Damit der lastPosition akkurat ist.
            _movePlayer = true;
        }

        private void OnCollisionExit(Collision other)
        {
            if (!other.gameObject.CompareTag("Player")) return;
            _player = null;
            _movePlayer = false;
        }
    }
}