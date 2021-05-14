using System;
using TMPro;
using UnityEngine;

public class VictoryScreenScript : MonoBehaviour
{
    private void Awake()
    {
        CloseVictoryScreen("");
        GameManager.StartListening("OpenVictoryScreen", OpenVictoryScreen);
        GameManager.StartListening ("CloseVictoryScreen", CloseVictoryScreen);
    }

    private void OnDestroy()
    {
        GameManager.StopListening("OpenVictoryScreen", OpenVictoryScreen);
        GameManager.StopListening ("CloseVictoryScreen", CloseVictoryScreen);
    }

    private void OpenVictoryScreen(string coins)
    {
        gameObject.SetActive(true);
        GameObject.Find("CollectedCoinsNumber").GetComponent<TextMeshProUGUI>().text = coins;
    }
    private void CloseVictoryScreen(string s)
    {
        gameObject.SetActive(false);
    }
}
