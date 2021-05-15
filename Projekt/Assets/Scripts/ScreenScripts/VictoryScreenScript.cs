using TMPro;
using UnityEngine;

namespace ScreenScripts
{
    public class VictoryScreenScript : MonoBehaviour
    {
        private void Awake()
        {
            CloseVictoryScreen("");
            GameManager.StartListening("OpenVictoryScreen", OpenVictoryScreen1);
            GameManager.StartListening ("CloseVictoryScreen", CloseVictoryScreen);
        }

        private void OnDestroy()
        {
            GameManager.StopListening("OpenVictoryScreen", OpenVictoryScreen1);
            GameManager.StopListening ("CloseVictoryScreen", CloseVictoryScreen);
        }

        private void OpenVictoryScreen1(string coins)
        {
            gameObject.SetActive(true);
            GameObject.Find("CollectedCoinsNumber").GetComponent<TextMeshProUGUI>().text = coins;
        }
        private void CloseVictoryScreen(string s)
        {
            gameObject.SetActive(false);
        }
    }
}
