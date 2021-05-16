using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class AnimationScript : MonoBehaviour
{
    private void Awake()
    {
        GameManager.StartListening("StartCameraAnimation", SetAnimation);
        GameManager.TriggerEvent("FetchCurrentWorld");
    }

    private void OnDestroy()
    {
        GameManager.StopListening("StartCameraAnimation", SetAnimation);
    }

    private void SetAnimation(string input)
    {
        // Reminder: Die Animation muss im Animator hinzugefügt und die transition gesetzt sein!
        int world = int.Parse(input.Split(';')[0]);
        int level = int.Parse(input.Split(';')[1]);
        gameObject.GetComponent<Animator>().SetInteger(Animator.StringToHash("World"),world);
        gameObject.GetComponent<Animator>().SetInteger(Animator.StringToHash("Level"),level);
    }
}
