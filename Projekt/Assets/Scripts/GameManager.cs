using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    /* Global */ /* Muss noch Funktionalität hinzugefügt werden! */
    public int maxUnlockedLevel = 0;
    public int maxLivePoints = 3;
    public int collectedCoinsTotal = 0; // Generell aufgesammelte Münzen, auch nach Neustart des Spiels.
    /* Lokal */
    public int currentLevel = 0;
    public int collectedCoinsInLevel = 0;
    public int livePoints = 0;
    // Start is called before the first frame update

    //TODO: Zu Eventsender und Listener umbauen
    public GameObject deathScreen;
    public GameObject victoryScreen;
    public GameObject playerBody;
    public GameObject hud;

    /*
     //Den Teil aktivieren, sobald eventlistener eingebaut sind!
     private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);//Zerstört alle weitere erstellten
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }*/

    private void Start()
    {
        deathScreen.SetActive(false);
        victoryScreen.SetActive(false);
        livePoints = maxLivePoints;
        hud.GetComponent<HudScript>().UpdateDisplay(livePoints,collectedCoinsInLevel);
    }

    /* Player Trigger */
    public void CoinCollected()
    {
        collectedCoinsTotal++;
        collectedCoinsInLevel++;
        hud.GetComponent<HudScript>().UpdateDisplay(livePoints,collectedCoinsInLevel);
        Debug.Log(collectedCoinsTotal);
    }

    public void Death() 
        /*Wird beim Eintritt des Death Triggers von Player aufgerufen*/
    {
        livePoints--;
        if (livePoints <= 0)
        {
            OpenGameOverScene();
        }
        else
        {
            hud.GetComponent<HudScript>().UpdateDisplay(livePoints, collectedCoinsInLevel);
            deathScreen.SetActive(true);
        }
    }

    public void Victory() 
        /*Wird beim Eintritt des Victory Triggers von Player aufgerufen*/
    {
        victoryScreen.SetActive(true);
    }

    private void OpenGameOverScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("GameOverScene");
    }
    /* UI Screen Aufrufe */
    public void OnRespawnButtonPressed()
    {
        deathScreen.SetActive(false);
        victoryScreen.SetActive(false);
        Debug.Log("Respawn");
        playerBody.GetComponent<PlayerController>().Respawn();
    }
    public void OnNextLevelButtonPressed()
    {
        //TODO: Speicher shit und geht zum nächsten Level
        Time.timeScale = 1;
        SceneManager.LoadScene("DemoLevel");
        
    }

    public void OnMainMenuButtonPressed()
    {
        //TODO: Speicher shit und Openscene MainMenu
        Time.timeScale = 1;
        SceneManager.LoadScene("StartScene");
    }
    public void OnExitGameButton()
    {
        Application.Quit();
    }
    
}
