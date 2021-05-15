using UnityEngine;

namespace ScreenScripts
{
    public class ButtonScript : MonoBehaviour
    {
        public void OnRespawnButtonPressed()
        {
            Time.timeScale = 1;
            GameManager.TriggerEvent("CloseDeathScreen");
            GameManager.TriggerEvent("CloseVictoryScreen");
            GameManager.TriggerEvent("Respawn");
        }
        public void OnNextLevelButtonPressed()
        {
            GameManager.TriggerEvent("LoadScene", "DemoLevel");

        }
        public void OnMainMenuButtonPressed()
        {
            GameManager.TriggerEvent("LoadScene", "StartScene");
        }
        public void OnExitGameButton()
        {
            Application.Quit();
        }
    }
}
