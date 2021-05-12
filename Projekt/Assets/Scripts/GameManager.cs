using System;
using System.Collections;
using System.Collections.Generic;
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
    Dazu Anpassung auf Singleton:
    https://www.youtube.com/watch?v=5p2JlI7PV1w&ab_channel=SpeedTutor
    //TODO: Zum Singleton machen
    */
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

    public static void TriggerEvent (string eventName) //TriggerEvent wenn man keine Floatübergabe braucht
    {
        Action<float> thisEvent = null;
        if (Instance._eventDictionary.TryGetValue (eventName, out thisEvent))
        {
            thisEvent.Invoke(0);
        }
    }public static void TriggerEvent (string eventName,float f)
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

    private void Awake()
    {
        StartListening("CoinCollected", CoinCollected);
        StartListening("Death", Death);
        StartListening("Victory", Victory);
    }

    private void Start()
    {
        TriggerEvent("CloseDeathScreen");
        TriggerEvent("CloseVictoryScreen");
        livePoints = maxLivePoints;
        TriggerEvent("UpdateLiveDisplay", livePoints);
        TriggerEvent("UpdateCoinDisplay", collectedCoinsInLevel);
    }

    /* Eventfunktionen */
    private void CoinCollected(float f)
    {
        collectedCoinsTotal++;
        collectedCoinsInLevel++;
        TriggerEvent("UpdateCoinDisplay", collectedCoinsInLevel);
    }

    private void Death(float f)
    {
        livePoints--;
        if (livePoints <= 0)
        {
            Time.timeScale = 1;
            SceneManager.LoadScene("GameOverScene");
        }
        else
        {
            TriggerEvent("OpenDeathScreen");
            TriggerEvent("UpdateLiveDisplay", livePoints);
        }
    }

    private void Victory(float f)
    {
        TriggerEvent("OpenVictoryScreen");
    }
    /* UI Screen Aufrufe */
    public void OnRespawnButtonPressed()
    {
        TriggerEvent("CloseDeathScreen");
        TriggerEvent("CloseVictoryScreen");
        TriggerEvent("Respawn");
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
