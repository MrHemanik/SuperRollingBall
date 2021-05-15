using TMPro;
using UnityEngine;

namespace ScreenScripts
{
    public class VictoryScreenScript : MonoBehaviour
    {
        private void Awake()
        {
            Debug.Log("Load "+gameObject.name);
            CloseVictoryScreen("");
            GameManager.StartListening("OpenVictoryScreen", OpenVictoryScreen);
            GameManager.StartListening ("CloseVictoryScreen", CloseVictoryScreen);
        }

        private void OnDestroy()
        {
            GameManager.StopListening("OpenVictoryScreen", OpenVictoryScreen);
            GameManager.StopListening ("CloseVictoryScreen", CloseVictoryScreen);
            Debug.Log("Dest "+gameObject.name);
            
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
    }
}
