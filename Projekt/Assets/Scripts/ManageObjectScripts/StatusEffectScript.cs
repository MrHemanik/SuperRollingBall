using System;
using System.Collections;
using System.Collections.Generic;
using PlayerScripts;
using UnityEngine;

public class StatusEffectScript : MonoBehaviour
{
    public string statusEffect;
    private float _duration = 10;
    private float _multiplier = 1.25f;
    private PlayerController _player;
    private GameObject _pickUp;

    public void Initialize(float d, float m,PlayerController p, GameObject pu)
    {
        _duration = d;
        _multiplier = m;
        _player = p;
        _pickUp = pu;
    }

    private void Start()
    {
        Destroy(gameObject, _duration);
        ActivateStatusEffect(_multiplier);
        Debug.Log("StatusEffect: "+statusEffect+" für "+_duration+ " sekunden um "+_multiplier+" multipliziert");
    }

    private void FixedUpdate()
    {
        transform.rotation = Quaternion.Euler(90,0,0);
    }

    private void OnDestroy()
    {
        ActivateStatusEffect(1/_multiplier);
        Debug.Log("StatusEffect: "+statusEffect+" ausgelaufen");
        _pickUp.SetActive(true); //Sobald der Effekt vorbei ist erscheint das Pickup wieder
    }

    private void ActivateStatusEffect(float multiplierInput)
    {
        switch (statusEffect)
        {
            case "JumpBoost":
                _player.MultiplyJumpModifier(multiplierInput);
                break;
            case "SpeedBoost":
                _player.MultiplySpeedModifier(multiplierInput);
                break;
            case "Wet":
                _player.PlayerWetness(multiplierInput);
                break;
        }
    }
}
