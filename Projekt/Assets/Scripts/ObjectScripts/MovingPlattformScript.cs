using System;
using UnityEngine;

namespace ObjectScripts
{
    public class MovingPlattformScript : MonoBehaviour
    {
        public Vector3 moveDirection;
        public float movingTime = 4.0f;
        public float pauseTimeStartEnd = 1.0f; //Zeit, die er am Punkt wartet.
        public float pauseTimeMid = 1.0f; //Zeit, die er am Punkt wartet.
        private Vector3 _endPoint;
        private Vector3 _startPoint;
        public GameObject _player;
        private bool _movePlayer;
        private Vector3 _speed; 
        private void Start()
        {
            _startPoint = transform.position;
            _endPoint = _startPoint + moveDirection;
            //Debug.Log(_startPoint +""+ _endPoint);
        }

        private void Update()
        {
            var position = transform.position;
            
            _speed = position;
            position = GetPosition(Time.time);
            transform.position = position; 
            _speed = position - _speed; //Speed = Neue Postion - Alte Postion 
            if(_movePlayer) _player.transform.position += _speed;
        }
    
        //Das hat ja mal so viel Zeit gekostet, das so umzusetzen.
        private Vector3 GetPosition(float time)
        {
            var intervalTime = 2 * (movingTime + pauseTimeStartEnd + pauseTimeMid); //Intervalllänge
            var timer = time % intervalTime; //Timer von 0 bis Intervalllänge
            Vector3 position;
            if (timer < movingTime / 2) //Hinweg erste Hälfte
                position = _startPoint + moveDirection / movingTime * timer;
            else if (timer < movingTime / 2 + pauseTimeMid) //Pause Hinweg
                position = _startPoint + moveDirection / 2;
            else if (timer < movingTime + pauseTimeMid) //Hin zweite Hälfte
                position = _startPoint + moveDirection / movingTime * (timer - pauseTimeMid);
            else if (timer < movingTime + pauseTimeStartEnd + pauseTimeMid) //Pause Ende
                position = _endPoint;
            else //Die zweite Hälfte ist die an _endPoint gespiegelte erste Hälfte: f(_intervalTime) = _endPoint +_startPoint - f(0)
                position = _endPoint + _startPoint - GetPosition(timer - intervalTime / 2);
            return position;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!other.gameObject.CompareTag("Player")) return;
            _player = other.gameObject;
            _movePlayer = true;
        }private void OnCollisionExit(Collision other)
        {
            if (!other.gameObject.CompareTag("Player")) return;
            _player = null;
            _movePlayer = false;
        }
    }
}