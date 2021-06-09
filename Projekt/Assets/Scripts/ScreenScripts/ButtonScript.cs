
using UnityEngine;


namespace ScreenScripts
{
    public class ButtonScript : MonoBehaviour
    {
        public void OnRespawnButtonPressed()
        {
            Time.timeScale = 1;
            GameManager.TriggerEvent("CloseDeathScreen");
            GameManager.TriggerEvent("BallRespawn");
            GameManager.TriggerEvent("ResetCamera");
        }
        public void OnNextLevelButtonPressed()
        {
            GameManager.TriggerEvent("LoadNextLevel");
        }

        public void OnHighestLevelButtonPressed()
        {
            GameManager.TriggerEvent("LoadHighestLevel");
        }
        public void OnPlayLevelAgainButtonPressed()
        {
            GameManager.TriggerEvent("ReloadLevel");
        }
        
        public void OnMainMenuButtonPressed()
        {
            GameManager.TriggerEvent("LoadScene", "StartScene");
        }
        public void OnExitGameButton()
        {
            Application.Quit();
        }

        public void OnToggleLevelSelectionScreenButtonPressed()
        {
            GameManager.TriggerEvent("ToggleLevelSelectionScreen");
        }
        public void OnLoadLevelButtonPressed(string levelName)
        {
            GameManager.TriggerEvent("LoadScene",levelName);
        }

        public void OnToggleDeleteSaveFileScreenButtonPressed()
        {
            GameManager.TriggerEvent("ToggleDeleteSaveFileScreen");
        }

        public void OnDeleteSaveFileButtonPressed()
        {
            GameManager.TriggerEvent("DeleteSaveFile");
        }
    }
}
