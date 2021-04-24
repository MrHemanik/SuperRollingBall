using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlattformScript : MonoBehaviour
{
    private Vector3 startPoint;
    private Vector3 endPoint;
    public Vector3 moveDirection;
    public float movingTime = 4.0f;
    public float waitOnPoint = 1.0f; //Zeit, die er am Punkt wartet.

    void Start()
    {
        startPoint = transform.position;
        endPoint = startPoint + moveDirection;
    }

    void Update()
    {
        Vector3 currentPosition = transform.position;
        //Ich habe dafür über 4 Stunden meines Lebens verschwendet :) Hab vieles Verkompliziert.
        //Wenn eine Koordinate über oder unter Start und Endpunkt ist, muss Richtung gewechselt werden.
        if (currentPosition.x > Mathf.Max(startPoint.x,endPoint.x) || 
            currentPosition.x < Mathf.Min(startPoint.x,endPoint.x) || //MathMinMax geht hier auch
            currentPosition.y > startPoint.y && currentPosition.y > endPoint.y || 
            currentPosition.y < startPoint.y && currentPosition.y < endPoint.y ||
            currentPosition.z > startPoint.z && currentPosition.z > endPoint.z || 
            currentPosition.z < startPoint.z && currentPosition.z < endPoint.z)
        {
            //Debug.Log("Umdrehen" + moveDirection + startPoint + endPoint);
            //Kann zu Problemen führen, falls es zu größeren Lags kommt
            moveDirection = moveDirection * -1;
            //TODO: Timer aktivieren, aber der hält nicht in Mitte, was nervig ist.
        }
        transform.position = currentPosition+(moveDirection/movingTime*Time.deltaTime);
        
        
    }
}
