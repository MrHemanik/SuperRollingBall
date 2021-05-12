using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int currentLevel = 0;
    public int maxUnlockedLevel = 0;
    public int collectedCoinsTotal = 0; // Generell aufgesammelte Münzen, auch nach Neustart des Spiels.
    public int collectedCoinsInLevel = 0;
    public int maxLivePoints = 3;
    public int livePoints = 0;
    // Start is called before the first frame update

    public void Start()
    {
        GameObject.Find("GameOver").SetActive(false);
        GameObject.Find("Victory").SetActive(false);

    }

    public void CoinCollected()
    {
        collectedCoinsTotal++;
        collectedCoinsInLevel++;
        GameObject.Find("HUD").GetComponent<HudScript>().UpdateDisplay();
        Debug.Log(collectedCoinsTotal);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
