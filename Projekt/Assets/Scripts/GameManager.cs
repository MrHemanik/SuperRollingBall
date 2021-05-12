using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    /*Fremdcode Anfang vom UnityTutorial https://learn.unity.com/tutorial/create-a-simple-messaging-system-with-events#5cf5960fedbc2a281acd21fa*/
    private Dictionary <string, Action<float>> _eventDictionary;
    private static GameManager _gameManager;

    private static GameManager Instance
    {
        get
        {
            if (!_gameManager)
            {
                _gameManager = FindObjectOfType (typeof (GameManager)) as GameManager;

                if (!_gameManager)
                {
                    Debug.LogError ("There needs to be one active GameManger script on a GameObject in your scene.");
                }
                else
                {
                    _gameManager.Init (); 
                }
            }

            return _gameManager;
        }
    }
    void Init ()
    {
        if (_eventDictionary == null)
        {
            _eventDictionary = new Dictionary<string, Action<float>>();
        }
    }

    public static void StartListening (string eventName, Action<float> listener)
    {
        Action<float> thisEvent = null;
        if (Instance._eventDictionary.TryGetValue (eventName, out thisEvent))
        {
            thisEvent+=listener;
        } 
        else
        {
            thisEvent+=listener;
            Instance._eventDictionary.Add (eventName, thisEvent);
        }
    }

    public static void StopListening (string eventName, Action<float> listener)
    {
        if (_gameManager == null) return;
        Action<float> thisEvent = null;
        if (Instance._eventDictionary.TryGetValue (eventName, out thisEvent))
        {
            thisEvent -= listener;
        }
    }

    public static void TriggerEvent (string eventName,float f)
    {
        Action<float> thisEvent = null;
        if (Instance._eventDictionary.TryGetValue (eventName, out thisEvent))
        {
            thisEvent.Invoke(f);
        }
    }
    /*Fremdcode ENDE*/
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
        TriggerEvent("UpdateLiveDisplay", livePoints);
        TriggerEvent("UpdateCoinDisplay", collectedCoinsInLevel);
    }

    /* Player Trigger */
    public void CoinCollected()
    {
        collectedCoinsTotal++;
        collectedCoinsInLevel++;
        TriggerEvent("UpdateCoinDisplay", collectedCoinsInLevel);
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
            TriggerEvent("UpdateLiveDisplay", livePoints);
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
