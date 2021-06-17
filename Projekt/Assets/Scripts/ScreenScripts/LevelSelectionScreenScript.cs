using ManageObjectScripts;
using UnityEngine;

namespace ScreenScripts
{
    public class LevelSelectionScreenScript : MonoBehaviour
    {
        // Start is called before the first frame update
        void Awake()
        {
            GameManager.StartListening("ToggleLevelSelectionScreen", ToggleLevelSelectionScreen);
            ToggleLevelSelectionScreen("");
            if (GameManager.GetMaxUnlockedLevel().Equals(99999))
            {
                gameObject.transform.Find("NewestLevelButton").gameObject.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            GameManager.StopListening("ToggleLevelSelectionScreen");
        }

        private void ToggleLevelSelectionScreen(string maxUnlockedLevel)
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }
    }
}
