using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectScript : MonoBehaviour
{
    public string statusEffect;
    public float duration = 10;
    public float multiplier = 1.25f;
    public PlayerController player;
    public GameObject pickUp;
    private void Start()
    {
        Destroy(gameObject, duration);
        ActivateStatusEffect(multiplier);
        Debug.Log("StatusEffect: "+statusEffect+" für "+duration+ " sekunden um "+multiplier+" multipliziert");
    }

    private void OnDestroy()
    {
        ActivateStatusEffect(1/multiplier);
        Debug.Log("StatusEffect: "+statusEffect+" ausgelaufen");
        pickUp.SetActive(true); //Sobald der Effekt vorbei ist erscheint das Pickup wieder
    }

    private void ActivateStatusEffect(float multiplierInput)
    {
        switch (statusEffect)
        {
            case "JumpBoost":
                player.MultiplyJumpModifier(multiplierInput);
                break;
            case "SpeedBoost":
                player.MultiplySpeedModifier(multiplierInput);
                break;
        }
    }
}
