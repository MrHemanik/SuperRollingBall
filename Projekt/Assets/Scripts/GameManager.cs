using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using JetBrains.Annotations;

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

    private static string _path;
    private static void LoadDataFromFile()
    {
        if (File.Exists(_path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(_path, FileMode.Open);
            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            stream.Close();
            if (data != null)
            {
                _maxUnlockedLevel = data.maxUnlockedLevel;
                _collectedCoinsTotal = data.collectedCoinsTotal;
                _timeHighscore = data.timeHighscore;
                _collectedPermaUpgrades = data.permaUpgrades;
                LoadUpgrades();
            }
        }
        else
        {
            Debug.Log("Keine Daten zum Laden gefunden!");
        }
    }

    private static void SaveDataToFile()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(_path, FileMode.Create);
        PlayerData data = new PlayerData(_maxUnlockedLevel,_maxLivePoints, _collectedCoinsTotal,_timeHighscore, _collectedPermaUpgrades);
        formatter.Serialize(stream, data);
        stream.Close();
    }
    /*Ende Fremdcode zum Savingsystem*/
    //Variablen
    /* Global */ /* Muss noch Funktionalität hinzugefügt werden! */
    private void DeleteSaveFile(string s)
    {
        _maxUnlockedLevel = 0;
        _collectedCoinsTotal = 0;
        _timeHighscore=null;
        _curLevel = 0;
        _collectedPermaUpgrades = new bool[100];
        File.Delete(_path);
        LoadScene("StartScene");
    }
    
    private static readonly int[] LevelList = {1001, 1002, 1003, 99999}; //MAINTAIN! Liste der Level im Spiel (99999 = VictoryScene)
    private static readonly string[] SkyboxColor = {"BFFFFD", "A995A5","000000", "FFFFFF"}; //Jedes level hat auch eine Skybox
    private static readonly string[] PermaUpgradeList = {"Live","Live","Hit"};
    private static int _maxUnlockedLevel; //Speichert die Arraystelle aus _levelList für das höchstfreigeschaltende Level 
    private static int _maxLivePoints = 3;
    private static int _maxHitPoints = 3;
    private static bool[] _collectedPermaUpgrades = new bool[100]; //Index von permaUpgradeList, was für ein Upgrade es ist
    private static int _collectedCoinsTotal; // Generell aufgesammelte Münzen, auch nach Neustart des Spiels.
    private static float[] _timeHighscore = new float[LevelList.Length];//new Time[_levelList.Length];
    /* Lokal */
    private int _curLevel; //Der index für das Level aus LevelList
    private int _collectedCoinsInLevel;
    private int _coinCountInLevel;
    private float _levelStartTime;
    private static int _livePoints;
    private static int _hitPoints;
    private bool _isInvincible;
    /*Prefabs*/
    public GameObject damageTakenScreenPrefab;
    


    private void Awake()
    {
        _path = Application.persistentDataPath + "/gameData.binary";
        StartListening("CoinCollected", CoinCollected);
        StartListening("HeartCollected", HeartCollected);
        StartListening("HitPointsCollected", HitPointsCollected);
        StartListening("Death", Death);
        StartListening("Victory", Victory);
        StartListening("DamageTaken", DamageTaken);
        StartListening("MakeVincible", MakeVincible);
        StartListening("LevelTimerStart", StartLevelTimer);
        StartListening("FetchDisplayData", UpdateHud);
        StartListening("FetchMainMenuData", UpdateMainMenu);
        StartListening("LoadScene", LoadScene);
        StartListening("FetchCurrentLevel", GiveCurrentLevelInfo);
        StartListening("LoadNextLevel", LoadNextLevel);
        StartListening("LoadHighestLevel", LoadHighestLevel);
        StartListening("ReloadLevel", ReloadLevel);
        StartListening("DeleteSaveFile", DeleteSaveFile);
        StartListening("PermaUpgradeCollected", PermaUpgradeCollected);
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
    /* Getter und Setter ---------------------------------------------------------------------------------------------*/
    
    //Getter und Setter für _timeHighscore, für den Fall, dass außerhalb der Arraygröße abgefragt wird (z.B durch alte Spieldaten)
    public static float GetHighscoreFromIndex(int i) //Ich musste es public machen, da sonst die LevelSelectionHighscores keinen guten Weg hätten, das abzufragen
    {
        //Gibt den Highscore des Indexes aus
        if (_timeHighscore == null) return 0;
        if (_timeHighscore.Length <= i) return 0;
        return _timeHighscore[i];
    }
    private static void SetHighscoreToIndex(int index, float highscore)
    {
        if (_timeHighscore == null || _timeHighscore.Length <= index)
        {
            float[] newTimeHighscore = new float[LevelList.Length];
            if (_timeHighscore != null)
            {
                for (int i = 0; i < _timeHighscore.Length; i++)
                {
                    newTimeHighscore[i] = _timeHighscore[0];
                }
            }
            _timeHighscore = newTimeHighscore;
        }
        _timeHighscore[index] = highscore;
    }

    public static int GetMaxUnlockedLevel()
    {
        return LevelList[_maxUnlockedLevel];
    }

    public static bool GetIsPermaUpgradeCollected(int id)
    {
        return _collectedPermaUpgrades[id];
    }
    /* Eventmethoden -------------------------------------------------------------------------------------------------*/
    private void CoinCollected(string s) // Wird beim Münzaufsammeln ausgelöst
    {
        _collectedCoinsTotal++;
        _collectedCoinsInLevel++;
        TriggerEvent("UpdateCoinDisplay", _collectedCoinsInLevel+" / "+_coinCountInLevel);
    }
    private void HeartCollected(string s) // Wird beim Herzaufsammeln ausgelöst
    {
        _livePoints++;
        TriggerEvent("UpdateLiveDisplay", _livePoints.ToString());
    }
    private void HitPointsCollected(string s) // Wird beim Herzaufsammeln ausgelöst
    {
        ChangeHitPoints(_maxHitPoints); // Regeneriert voll
    }
    
    private void Death(string s) // Wird beim Tod ausgelöst (Runterfallen oder keine Hitpoints mehr
    {
        _livePoints--;
        ChangeHitPoints(_maxHitPoints); //Wird visuell beim Respawnknopf aktualisiert
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
    private void Victory(string s) // Wird beim Erfüllen der Zielbedingung ausgelöst
    {
        TriggerEvent("OpenVictoryScreen",_collectedCoinsInLevel+" / "+_coinCountInLevel);
        TestForLevelHighscore();
        UnlockNextLevel();
    }
    

    private void DamageTaken(string s) //Beim Trigger mit Damage Objekt
    {
        if (!_isInvincible)
        {
            _isInvincible = true;
            Instantiate(damageTakenScreenPrefab, transform.position, new Quaternion()); // TODO: Sollte eigentlich mit in DamageTaken sein
            TimerManagerScript.StartTimer("MakeVincible", 2f); //Wird so gelöst anstelle des EffectSystems, da das nicht als Effect aufgelistet werden soll
            ChangeHitPoints(_hitPoints-1);

        }
    }
    //Callback Methoden
    private void StartLevelTimer(string s) //Wird ausgeführt nachdem die Startkamerafahrt des Levels fertig ist
    {
        _levelStartTime = Time.time;
    }

    private void MakeVincible(string s)
    {
        _isInvincible = false;
    }
    //Start of Scene Methoden
    private void UpdateHud(string s) //Beim ersten Laden des Huds werden die Daten geholt
    {
        _coinCountInLevel=GameObject.Find("AllCoins").gameObject.transform.childCount; //Muss nur beim Start eines neuen Levels geupdated werden
        TriggerEvent("UpdateLiveDisplay", _livePoints.ToString());
        TriggerEvent("UpdateCoinDisplay", _collectedCoinsInLevel+" / "+_coinCountInLevel);
    }

    private void UpdateMainMenu(string s) //Trigger: Laden des Hauptmenüs
    {
        //Gibt dem Hauptmenü die Informationen über die gesammelten Münzen insgesamt
        TriggerEvent("UpdateCollectedCoinsTotal",_collectedCoinsTotal.ToString());
    }
    private void GiveCurrentLevelInfo(string s)
    {
        //Zum Start des Levels wird die Skybox angepasst und die Startanimation angespielt
        TriggerEvent("StartCameraAnimation",LevelList[_curLevel].ToString());
        TriggerEvent("SkyboxColor",SkyboxColor[_curLevel]);
    }
    //SceneManagement
    private void LoadScene(string sceneName) //Lädt eine Scene
    {
        Debug.Log("Scene wird geladen: "+sceneName);
        /* Lädt die Szene sceneName und setzt die lokalen Variablen zurück*/
        SaveDataToFile(); //Speichert die Spieldaten
        if (sceneName == "99999_Level")
        {
            //Falls das Level das "letzte" Level ist (99999), also der letzte eintrag in LevelList, dann soll VictoryScene geöffnet werden
            SceneManager.LoadScene("VictoryScene");
        }
        else
        {
            try//Falls es ein Level ist, soll _curLevel angepasst werden
            {
                _curLevel = Array.IndexOf(LevelList,int.Parse(sceneName.Split('_')[0])); //Zahl vor _; 1002_Level = 1;;
            }
            catch
            {
                // ignored
            }

            SceneManager.LoadScene(sceneName);
        }
        Reset(); //setzt die lokalen Variablen zurück
    }

    private void LoadNextLevel(string s)
    {
        //Sucht das Level in _levelList und lädt das darauf folgende
        LoadScene(LevelList[_curLevel+1]+"_Level");
    }

    private void LoadHighestLevel(string s)
    {
        //Lädt das höchste freigeschaltende Level
        LoadScene(LevelList[_maxUnlockedLevel]+"_Level");
    }

    private void ReloadLevel(string s)
    {
        LoadScene(LevelList[_curLevel]+"_Level");
    }

    private void PermaUpgradeCollected(string id)
    {
        _collectedPermaUpgrades[int.Parse(id)] = true;
        CollectUpgrade(int.Parse(id));
    }

    /* Methoden ------------------------------------------------------------------------------------------------------*/
    private void Reset() //Setzt lokale Variablen zurück
    {
        
        _livePoints = _maxLivePoints;
        _hitPoints = _maxHitPoints;
        _collectedCoinsInLevel = 0;
        _coinCountInLevel = 0;
    }
    
    private void TestForLevelHighscore() //Testet, ob die Zeit kürzer als Highscore ist und agiert dementsprechend
    {
        float timeHighscore = GetHighscoreFromIndex(_curLevel);
        float newTime = Time.time - _levelStartTime;
        Debug.Log("High:" + timeHighscore+"; Neu: "+newTime);
        if (timeHighscore.Equals(0) || timeHighscore > newTime)
        {
            SetHighscoreToIndex(_curLevel,newTime);
            TriggerEvent("NewHighscore");
        }
        TriggerEvent("SetLevelTime", newTime.ToString("0.00"));
    }
    private void UnlockNextLevel() //Testet, ob das geschaffte Level das höchst-freigeschaltende ist und agiert dementsprechend
    {
        //Erhöht das _maxUnlockedLevel, falls das Level _levelList[_maxUnlockedLevel] geschafft wurde
        Debug.Log("Level: "+LevelList[_curLevel]+"; MaxLevel: "+LevelList[_maxUnlockedLevel]);
        if (_curLevel >= _maxUnlockedLevel)//Falls das momentane Level das höchstfreigeschaltende ist
        {
            //Victory darf nicht im VictoryScene ausgelöst werden, da er sonst outOufBounds geht.
            _maxUnlockedLevel = _curLevel + 1;
            Debug.Log("Neues Level freigeschalten");
        }
    }

    private static void ChangeHitPoints(int newHitpoints)
    {
        _hitPoints = newHitpoints;
        TriggerEvent("ResizeCore", Math.Sqrt(1.0*_hitPoints / _maxHitPoints).ToString("0.00")); //Neue Coregröße
        TriggerEvent("UpdateHitPointDisplay", (1.0 * _hitPoints / _maxHitPoints).ToString("0.00"));
        if(_hitPoints <= 0) TriggerEvent("BallDeath"); //Ruft BallDeath in PlayerCon auf, welcher Death hier auslöst
    }

    private static void LoadUpgrades() //Lädt die Upgrades 
    {
        if (_collectedPermaUpgrades == null) //Falls es null ist kann es leer instanziert werden
        {
            _collectedPermaUpgrades = new bool[100];
            return;
        }

        for (var i=0;i<_collectedPermaUpgrades.Length;i++)
        {
            if (_collectedPermaUpgrades[i]) CollectUpgrade(i);
            
        }
    }

    private static void CollectUpgrade(int id)
    {
        Debug.Log("Aktives Permaupgrade gefunden: "+id);
        switch (PermaUpgradeList[id])
            {
                case "Live":
                    _maxLivePoints++;
                    _livePoints = _maxLivePoints; //Wird vollgeheilt
                    break;
                case "Hit":
                    _maxHitPoints++;
                    ChangeHitPoints(_maxHitPoints); //Wird vollgeheilt
                    break; 
            }
    }
    /* Input System Methoden -----------------------------------------------------------------------------------------*/
    [UsedImplicitly]
    public void OnPause() //Pausiert das Spiel (Auslöser: p)
    {
        if (Time.timeScale != 0) Time.timeScale = 0;
        else Time.timeScale = 1;
        TriggerEvent("TogglePauseScreen");
    }
}
