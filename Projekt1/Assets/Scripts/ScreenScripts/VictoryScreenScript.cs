using TMPro;
using UnityEngine;

namespace ScreenScripts
{
    public class VictoryScreenScript : MonoBehaviour
    {
        private GameObject _highscoreText;
        private GameObject _highscoreNumber;
        private void Awake()
        {
            GameManager.StartListening("OpenVictoryScreen", OpenVictoryScreen);
            GameManager.StartListening ("CloseVictoryScreen", CloseVictoryScreen);
            GameManager.StartListening ("NewHighscore", NewHighscore);
            GameManager.StartListening ("SetLevelTime",SetLevelTime);
        }

        private void Start()
        {
            _highscoreNumber = GameObject.Find("HighscoreNumber");
            _highscoreText = GameObject.Find("NewHighscoreText");
            _highscoreText.SetActive(false);
            CloseVictoryScreen("");
        }

        private void OnDestroy()
        {
            GameManager.StopListening("OpenVictoryScreen");
            GameManager.StopListening ("CloseVictoryScreen");
            GameManager.StopListening ("NewHighscore");
            GameManager.StopListening ("SetLevelTime");
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

        private void SetLevelTime(string time)
        {
            _highscoreNumber.GetComponent<TextMeshProUGUI>().text= "Zeit: "+time+" Sekunden";
        }
        private void NewHighscore(string highscore)
        {
            _highscoreText.SetActive(true);
            
        }
    }
}
