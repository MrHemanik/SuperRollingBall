using ManageObjectScripts;
using TMPro;
using UnityEngine;

namespace ScreenScripts
{
    public class StatisticsMainMenuScript : MonoBehaviour
    {
        private void Awake()
        {
            //Alte Variante: gameObject.GetComponent<TextMeshProUGUI>().text = GameManager.collectedCoinsTotal.ToString();
            GameManager.StartListening("UpdateCollectedCoinsTotal", UpdateCoinCount);
            GameManager.TriggerEvent("FetchMainMenuData");
        }


        private void OnDestroy()
        {
            GameManager.StopListening("UpdateCollectedCoinsTotal");
        }

        private void UpdateCoinCount(string count)
        {
            gameObject.GetComponent<TextMeshProUGUI>().text = count;
        }
    }
}