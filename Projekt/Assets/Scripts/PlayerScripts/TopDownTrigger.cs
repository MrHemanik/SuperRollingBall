using System;
using System.Collections;
using System.Collections.Generic;
using ManageObjectScripts;
using UnityEngine;

public class TopDownTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.TriggerEvent("CameraModeTopDown");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.TriggerEvent("CameraModeNormal");
        }
    }
}
