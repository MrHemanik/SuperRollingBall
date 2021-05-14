using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatisticsMainMenuScript : MonoBehaviour
{
    void Awake()
    {
        //Alte Variante: gameObject.GetComponent<TextMeshProUGUI>().text = GameManager.collectedCoinsTotal.ToString();
        GameManager.StartListening("UpdateCollectedCoinsTotal", UpdateCoinCount);
        GameManager.TriggerEvent("FetchMainMenuData");
    }

    private void OnDestroy()
    {
        GameManager.StopListening("UpdateCollectedCoinsTotal", UpdateCoinCount);
    }
    private void UpdateCoinCount(string count)
    {
        gameObject.GetComponent<TextMeshProUGUI>().text = count;
    }
}
