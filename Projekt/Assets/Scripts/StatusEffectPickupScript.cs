using System;
using UnityEngine;

public class StatusEffectPickupScript : MonoBehaviour
{
    public string statusEffect;
    public float duration;
    public float multiplier;


    // Update is called once per frame
    private void CreateStatusEffect(GameObject player)
    {
        //Kein Instantiate, da so sonst 2 GameObjects entstehen
        GameObject statusObject = new GameObject("StatusEffect", typeof(StatusEffectScript));
        statusObject.transform.parent = player.transform.parent.Find("StatusEffects").transform; //Macht den Statuseffekt zum Child
        StatusEffectScript statusScript = statusObject.GetComponent<StatusEffectScript>();
        statusScript.player = player.GetComponent<PlayerController>();
        statusScript.pickUp = gameObject;
        statusScript.statusEffect = statusEffect;
        statusScript.duration = duration;
        statusScript.multiplier = multiplier;
        Debug.Log("Statuseffekt initialisiert");
    }

    private void OnTriggerEnter(Collider other)
    {
        CreateStatusEffect(other.gameObject);
        gameObject.SetActive(false);
    }
}
