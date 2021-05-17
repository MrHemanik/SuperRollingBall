using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


public class GameManager : MonoBehaviour
{
    /*Fremdcode Anfang
    Vom UnityTutorial:
    https://learn.unity.com/tutorial/create-a-simple-messaging-system-with-events#5cf5960fedbc2a281acd21fa
    Mit StackOverflow Änderungen für Parameterübergabe (UnityEvent zu  Action):
    https://stackoverflow.com/questions/42177820/pass-argument-to-unityevent
    Und eigenen Anpassungen
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
                    
                    Debug.LogError ("Kein aktiver GameManager gefunden!");
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
    //Viel umgeschrieben in den Listenern ... verstehen tu ich sie trotzdem nicht wirklich
    public static void StartListening (string eventName, Action<string> listener)
    {
        if (!Instance._eventDictionary.TryGetValue (eventName, out _))
        {
            Instance._eventDictionary.Add (eventName, listener);
        }
    }

    public static void StopListening (string eventName)
    {
        if (_gameManager == null) return;
        if (Instance._eventDictionary.TryGetValue (eventName, out _))
        {
            //Eigenes Codestück, welches den Listener auch wirklich entfernt
            Instance._eventDictionary.Remove(eventName);
        }
    }
    public static void TriggerEvent (string eventName,string f = "") //Eigenanpassung mit Übergabefloat
    {
        if (Instance._eventDictionary.TryGetValue (eventName, out var thisEvent))
        {
            thisEvent.Invoke(f);
        }
    }
    /*Fremdcode zu Message-System ENDE*/
    /*Fremdcode fürs Savingsystem in BinaryFormat, Fremdcode aus der Quelle: https://www.youtube.com/watch?v=XOjd_qU2Ido&ab_channel=Brackeys*/

    private static void LoadDataFromFile()
    {
        string path = Application.persistentDataPath + "/gameData.binary";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            stream.Close();
            if (data != null)
            {
                _maxUnlockedLevel = data.maxUnlockedLevel;
                _maxLivePoints = data.maxLivePoints;
                _collectedCoinsTotal = data.collectedCoinsTotal;
                _timeHighscore = data.timeHighscore;
            }
        }
        else
        {
            Debug.Log("Keine Daten zum Laden gefunden!");
        }
    }

    private static void SaveDataToFile()
    {
        string path = Application.persistentDataPath + "/gameData.binary";
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        PlayerData data = new PlayerData(_maxUnlockedLevel,_maxLivePoints, _collectedCoinsTotal,_timeHighscore);
        formatter.Serialize(stream, data);
        stream.Close();
    }
    /*Ende Fremdcode zum Savingsystem*/
    //Variablen
    /* Global */ /* Muss noch Funktionalität hinzugefügt werden! */
    
    private static readonly int[] LevelList = {1001, 1002, 99999}; //MAINTAIN! Liste der Level im Spiel (99999 = VictoryScene)
    private readonly string[] _skyboxColor = {"BFFFFD", "A995A5", "FFFFFF"}; //Jedes level hat auch eine Skybox
    private static int _maxUnlockedLevel; //Speichert die Arraystelle aus _levelList für das höchstfreigeschaltende Level 
    private int _curLevel; //Das momentane Level als Levelzahl (1001,1002,..)
    private static int _maxLivePoints = 3;
    private static int _collectedCoinsTotal; // Generell aufgesammelte Münzen, auch nach Neustart des Spiels.
    private static float[] _timeHighscore = new float[LevelList.Length];//new Time[_levelList.Length];
    /* Lokal */
    private int _collectedCoinsInLevel;
    private int _livePoints;
    private float _levelStartTime;
    

    private void Awake()
    {
        StartListening("CoinCollected", CoinCollected);
        StartListening("HeartCollected", HeartCollected);
        StartListening("Death", Death);
        StartListening("Victory", Victory);
        StartListening("LevelTimerStart", StartLevelTimer);
        StartListening("FetchDisplayData", UpdateHud);
        StartListening("FetchMainMenuData", UpdateMainMenu);
        StartListening("LoadScene", LoadScene);
        StartListening("FetchCurrentLevel", GiveCurrentLevel);
        StartListening("LoadNextLevel", LoadNextLevel);
    }

    private void Start()
    {
        // Sorgt dafür, dass es nicht mehrere GameManager in der Scene gibt
        if (this == _gameManager)
        {
            DontDestroyOnLoad(gameObject);
            LoadDataFromFile();
            LoadScene("StartScene");
        }
        else Destroy(gameObject);
    }
    
    private void Reset()
    {
        _livePoints = _maxLivePoints;
        _collectedCoinsInLevel = 0;
    }
    /* Eventfunktionen */
    private void CoinCollected(string s)
    {
        _collectedCoinsTotal++;
        _collectedCoinsInLevel++;
        TriggerEvent("UpdateCoinDisplay", _collectedCoinsInLevel.ToString());
    }private void HeartCollected(string s)
    {
        _livePoints++;
        TriggerEvent("UpdateLiveDisplay", _livePoints.ToString());
    }
    
    private void Death(string s)
    {
        _livePoints--;
        if (_livePoints <= 0)
        {
            LoadScene("GameOverScene");
        }
        else
        {
            TriggerEvent("OpenDeathScreen");
            TriggerEvent("UpdateLiveDisplay", _livePoints.ToString());
        }
    }
    private void Victory(string s)
    {
        TriggerEvent("OpenVictoryScreen",_collectedCoinsInLevel+" / "+GameObject.Find("AllCoins").gameObject.transform.childCount);
        TestForLevelHighscore();
        UnlockNextLevel();
    }

    private void TestForLevelHighscore()
    {
        int curLevelIndex = Array.IndexOf(LevelList, _curLevel);
        float timeHighscore = _timeHighscore[curLevelIndex];
        float newTime = Time.time - _levelStartTime;
        Debug.Log("High:" + timeHighscore+"; Neu: "+newTime);
        if (timeHighscore.Equals(0) || timeHighscore > newTime)
        {
            _timeHighscore[curLevelIndex] = newTime;
        }

        Debug.Log(_timeHighscore[curLevelIndex]);
    }
    private void UnlockNextLevel()
    {
        //Erhöht das _maxUnlockedLevel, falls das Level _levelList[_maxUnlockedLevel] geschafft wurde
        Debug.Log("Level: "+_curLevel+"; MaxLevel: "+LevelList[_maxUnlockedLevel]);
        if (_curLevel == LevelList[_maxUnlockedLevel])//Falls das momentane Level das höchstfreigeschaltende ist
        {
            //Victory darf nicht im VictoryScene ausgelöst werden, da er sonst outOufBounds geht.
            _maxUnlockedLevel++;
            _curLevel = LevelList[_maxUnlockedLevel];
            Debug.Log("Neues Level freigeschalten");
        }
    }
    private void StartLevelTimer(string s)
    {
        _levelStartTime = Time.time;
    }
    private void UpdateHud(string s)
    {
        TriggerEvent("UpdateLiveDisplay", _livePoints.ToString());
        TriggerEvent("UpdateCoinDisplay", _collectedCoinsInLevel.ToString());
    }

    private void UpdateMainMenu(string s)
    {
        TriggerEvent("UpdateCollectedCoinsTotal",_collectedCoinsTotal.ToString());
    }
    
    private void LoadScene(string sceneName)
    {
        /* Lädt die Szene sceneName und setzt die die lokalen Variablen zurück*/
        SaveDataToFile();
        if (sceneName == "99999_Level")
        {
            SceneManager.LoadScene("VictoryScene");
        }
        else
        {
            try
            {
                _curLevel = int.Parse(sceneName.Split('_')[0]); //Zahl vor _; 1002_Level = 1;;
            }
            catch
            {
                // ignored
            }

            SceneManager.LoadScene(sceneName);
        }
        Reset();
    }
    private void GiveCurrentLevel(string s)
    {
        TriggerEvent("StartCameraAnimation",_curLevel.ToString());
        TriggerEvent("SkyboxColor",_skyboxColor[Array.IndexOf(LevelList,_curLevel)]);
    }

    private void LoadNextLevel(string s)
    {
        //Sucht das Level in _levelList und lädt das darauf folgende
        Debug.Log(_curLevel);
        LoadScene(LevelList[Array.IndexOf(LevelList, _curLevel)+1]+"_Level");
    }

    
    // * Input System Methoden* //
    public void OnPause()
    {
        if (Time.timeScale != 0) Time.timeScale = 0;
        else Time.timeScale = 1;
        TriggerEvent("TogglePauseScreen");
    }
}
