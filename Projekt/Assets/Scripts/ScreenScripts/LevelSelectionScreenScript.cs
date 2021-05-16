using System;
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
            GameManager.StartListening("ToggleLevelSelectionScreen", ToggleLevelSelectionScreen);
        }

        private void ToggleLevelSelectionScreen(string f ="")
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }
    }
}
