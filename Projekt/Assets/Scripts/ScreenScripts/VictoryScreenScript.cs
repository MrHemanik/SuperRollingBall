using System;
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
        }

        private void Start()
        {
            Debug.Log("Lol");
            _highscoreNumber = GameObject.Find("HighscoreNumber");
            _highscoreText = GameObject.Find("HighscoreText");
            _highscoreNumber.SetActive(false);
            _highscoreText.SetActive(false);
            CloseVictoryScreen("");
        }

        private void OnDestroy()
        {
            GameManager.StopListening("OpenVictoryScreen");
            GameManager.StopListening ("CloseVictoryScreen");
            GameManager.StopListening ("NewHighscore");
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
        private void NewHighscore(string highscore)
        {
            _highscoreNumber.SetActive(true);
            _highscoreText.SetActive(true);
            _highscoreNumber.GetComponent<TextMeshProUGUI>().text= highscore+" Sekunden";
        }
    }
}
