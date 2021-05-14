using System;
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

    // Update is called once per frame
    private void OpenVictoryScreen(string s)
    {
        gameObject.SetActive(true);
    }
    private void CloseVictoryScreen(string s)
    {
        gameObject.SetActive(false);
    }
}
