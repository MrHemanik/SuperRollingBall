using ManageObjectScripts;
using UnityEngine;

namespace ScreenScripts
{
    public class PauseScreenScript : MonoBehaviour
    {
        private void Awake()
        {
            TogglePauseScreen("");
            GameManager.StartListening("TogglePauseScreen", TogglePauseScreen);
        }

        private void OnDestroy()
        {
            GameManager.StopListening("TogglePauseScreen");
        }

        private void TogglePauseScreen(string coins)
        {
            if (gameObject.activeSelf) gameObject.SetActive(false);
            else gameObject.SetActive(true);
        }
    }
}