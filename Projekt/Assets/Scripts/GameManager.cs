using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    /*Fremdcode Anfang
    Vom UnityTutorial:
    https://learn.unity.com/tutorial/create-a-simple-messaging-system-with-events#5cf5960fedbc2a281acd21fa
    Mit StackOverflow Änderungen für Parameterübergabe (UnityEvent zu  Action):
    https://stackoverflow.com/questions/42177820/pass-argument-to-unityevent
    */
    private Dictionary <string, Action<string>> _eventDictionary;
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
            _eventDictionary = new Dictionary<string, Action<string>>();
        }
    }

    public static void StartListening (string eventName, Action<string> listener)
    {
        Action<string> thisEvent = null;
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

    public static void StopListening (string eventName, Action<string> listener)
    {
        if (_gameManager == null) return;
        Action<string> thisEvent = null;
        if (Instance._eventDictionary.TryGetValue (eventName, out thisEvent))
        {
            //Eigenes Codestück, welches den Listener auch wirklich entfernt
            Instance._eventDictionary.Remove(eventName);
            //
            thisEvent -= listener;
        }
    }

    
    public static void TriggerEvent (string eventName,string f)//Eigenanpassung mit Übergabefloat
    {
        Action<string> thisEvent = null;
        if (Instance._eventDictionary.TryGetValue (eventName, out thisEvent))
        {
            thisEvent.Invoke(f);
        }
    }
    public static void TriggerEvent (string eventName) //TriggerEvent wenn man keine Floatübergabe braucht
    {
        TriggerEvent(eventName,"");
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
    

    private void Awake()
    {
        // Sorgt dafür, dass es nicht mehrere GameManager im Objekt gibt
        if(this == _gameManager) DontDestroyOnLoad(gameObject);
        else Destroy(gameObject);
        
        StartListening("CoinCollected", CoinCollected);
        StartListening("Death", Death);
        StartListening("Victory", Victory);
        StartListening("FetchDisplayData", UpdateHud);
    }

    private void Start()
    {
        Time.timeScale = 1;
        livePoints = maxLivePoints;
        collectedCoinsInLevel = 0;
    }

    /* Eventfunktionen */
    private void CoinCollected(string s)
    {
        collectedCoinsTotal++;
        collectedCoinsInLevel++;
        TriggerEvent("UpdateCoinDisplay", collectedCoinsInLevel.ToString());
    }
    
    private void Death(string s)
    {
        livePoints--;
        if (livePoints <= 0)
        {
            LoadScene("GameOverScene");
        }
        else
        {
            Time.timeScale = 0;
            TriggerEvent("OpenDeathScreen");
            TriggerEvent("UpdateLiveDisplay", livePoints.ToString());
        }
    }
    private void Victory(string s)
    {
        Time.timeScale = 0;
        TriggerEvent("OpenVictoryScreen");
    }

    private void UpdateHud(string s)
    {
        TriggerEvent("UpdateLiveDisplay", livePoints.ToString());
        TriggerEvent("UpdateCoinDisplay", collectedCoinsInLevel.ToString());
    }
    /* UI Screen Aufrufe */
    public void OnRespawnButtonPressed()
    {
        Time.timeScale = 1;
        TriggerEvent("CloseDeathScreen");
        TriggerEvent("CloseVictoryScreen");
        TriggerEvent("Respawn");
    }
    public void OnNextLevelButtonPressed()
    {
        //TODO: Speicher shit und geht zum nächsten Level
        LoadScene("DemoLevel");
        
    }
    public void OnMainMenuButtonPressed()
    {
        //TODO: Speicher shit
        LoadScene("StartScene");
    }
    public void OnExitGameButton()
    {
        Application.Quit();
    }

    /** Lädt die Szene sceneName und setzt die die lokalen Variablen zurück**/
    private void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        Start();
    }
}
