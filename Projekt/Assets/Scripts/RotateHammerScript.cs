using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateHammerScript : MonoBehaviour
{
    private void Update()
    {

    }

    private void OnCollisionEnter(Collision other) //Bei Berührung eines Objektes (Kollision)
    {
        if(other.gameObject.CompareTag("Player")) //Berührung mit Boden
        {
            other.gameObject.GetComponent<Rigidbody>().AddForce(10000* (transform.GetChild(0).position - transform.position));
        }
    }
}