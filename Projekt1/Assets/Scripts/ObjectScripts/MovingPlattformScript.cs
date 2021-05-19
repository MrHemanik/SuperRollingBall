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

        private void Start()
        {
            _startPoint = transform.position;
            _endPoint = _startPoint + moveDirection;
            //Debug.Log(_startPoint +""+ _endPoint);
        }

        private void Update()
        {
            transform.position = GetPosition(Time.time);
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
    }
}