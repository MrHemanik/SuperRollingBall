using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlattformScript : MonoBehaviour
{
    private Vector3 _startPoint;
    private Vector3 _endPoint;
    public Vector3 moveDirection;
    public float movingTime = 4.0f; //Zeit, die bis zum Punkt gebraucht wird (exklusive pauseTime)
    public float pauseTime = 1.0f; //Zeit, die er am Punkt wartet.
    private float _waitTilNextCheckTime; //Zum Schutz gegen Festhängen durch Lags
    private float _timer = 0.0f;

    void Start()
    {
        _startPoint = transform.position;
        _endPoint = _startPoint + moveDirection;
        _waitTilNextCheckTime = movingTime/2 + pauseTime;
    }

    void Update()
    {
        if (_timer > pauseTime) //Sobald der Timer die wartezeit bis zum nächsten Check überläuft, dann darf wieder gechecked werden
        {
            Vector3 currentPosition = transform.position;
            if(_timer > _waitTilNextCheckTime){ //Erst wenn der Timer die Wartezeit bis zum Check durchlaufen hat, darf er wieder checken
                //Ich habe dafür über 4 Stunden meines Lebens verschwendet :) Hab vieles Verkompliziert.
                //Wenn eine Koordinate über oder unter Start und Endpunkt ist, muss Richtung gewechselt werden.
                if (currentPosition.x > Mathf.Max(_startPoint.x, _endPoint.x) ||
                    currentPosition.x < Mathf.Min(_startPoint.x, _endPoint.x) || //MathMinMax geht hier auch
                    currentPosition.y > _startPoint.y && currentPosition.y > _endPoint.y ||
                    currentPosition.y < _startPoint.y && currentPosition.y < _endPoint.y ||
                    currentPosition.z > _startPoint.z && currentPosition.z > _endPoint.z ||
                    currentPosition.z < _startPoint.z && currentPosition.z < _endPoint.z)
                {
                    //Debug.Log("Umdrehen" + moveDirection + startPoint + endPoint);
                    //Kann zu Problemen führen, falls es zu größeren Lags kommt
                    moveDirection = moveDirection * -1; //Umdrehen
                    _timer = 0.0f;
                }
            }
            transform.position = currentPosition + (moveDirection / movingTime * Time.deltaTime); //Bewegen
        }
        _timer += Time.deltaTime;

        


    }
}
