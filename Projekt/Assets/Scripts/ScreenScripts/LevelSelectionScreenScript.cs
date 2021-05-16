using UnityEngine;

namespace ScreenScripts
{
    public class LevelSelectionScreenScript : MonoBehaviour
    {
        // Start is called before the first frame update
        void Awake()
        {
            GameManager.StartListening("ToggleLevelSelectionScreen", ToggleLevelSelectionScreen);
            ToggleLevelSelectionScreen();
        }

        private void OnDestroy()
        {
            GameManager.StopListening("ToggleLevelSelectionScreen");
        }

        private void ToggleLevelSelectionScreen(string f ="")
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }
    }
}
