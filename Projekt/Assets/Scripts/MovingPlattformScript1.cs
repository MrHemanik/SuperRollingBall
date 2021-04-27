
using UnityEngine;

public class MovingPlattformScript1 : MonoBehaviour
{
    //Die 'who toucha my spaghet' - Version von MovingPlattformScript, in der ich mit Hilfe von Aufteilung die Bewegung in eine Mathematische Funktion gewandelt habe.
    private Vector3 _startPoint;
    private Vector3 _endPoint;
    public Vector3 moveDirection;
    public float movingTime = 4.0f;
    public float pauseTime = 1.0f; //Zeit, die er am Punkt wartet.
    public bool pauseInMid = true;
    private float _timer = 0.0f;
    private float _intervalTime;

    void Start()
    {
        _startPoint = transform.position;
        _endPoint = _startPoint + moveDirection;
        
    }

    void Update()
    {
        _intervalTime = pauseInMid ? 2 * (movingTime + 2 * pauseTime) : 2 * (movingTime + pauseTime); //Kann eigentlich in Start, aber falls man die checkbox während des Levels ändert, dann gibt es Probleme
        transform.position = pauseInMid ? GetPositionWithMidPause() : GetPosition();
    }

    //Grafische Darstellung: https://i.imgur.com/RbPpvj6.jpg
    private Vector3 GetPosition() //Gibt die Position zurück. Wartet am Start und Ende.
    {
        _timer = Time.time % _intervalTime; //Intervalllänge
        Vector3 position;
        if(_timer <= movingTime) //Hinweg
        {
            position = _startPoint + moveDirection / movingTime * _timer;
        }else if (_timer <= movingTime + pauseTime) //Pause am Ende
        {
            position = _endPoint;
        }else if (_timer <= movingTime * 2 + pauseTime) //Rückweg
        {
            position = _endPoint - moveDirection / movingTime * (_timer-_intervalTime/2);
        }
        else //Pause am Anfang
        {
            position = _startPoint;
        }
        return position;
    }

    private Vector3 GetPositionWithMidPause()
    {
        _timer = Time.time % _intervalTime; //Intervalllänge
        Vector3 position;
        if (_timer < movingTime / 2) //Hin1
        {
            position = _startPoint + moveDirection / movingTime * _timer;
        } else if (_timer < movingTime / 2 + pauseTime)//PauseHin
        {
            position = _startPoint + moveDirection / 2;
        }else if (_timer < movingTime + pauseTime)//Hin2
        {
            position = _startPoint + moveDirection / movingTime * (_timer-pauseTime);
        }else if (_timer < movingTime + 2*pauseTime)//PauseEnd
        {
            position = _endPoint;
        }else if (_timer < 1.5*movingTime + 2*pauseTime)//Zurück1
        {
            position = _endPoint - moveDirection / movingTime * (_timer-_intervalTime/2);
        }else if (_timer < 1.5*movingTime + 3*pauseTime)//PauseZurück
        {
            position = _startPoint + moveDirection / 2;
        }
        else if (_timer < 2*movingTime + 3*pauseTime)//Zurück2
        {
            position = (_startPoint + moveDirection / 2) - moveDirection / movingTime * (float) (_timer-(1.5*movingTime + 3*pauseTime));
        }
        else //PauseStart
        {
            position = _startPoint;
        }

        return position;
    }
}
