using UnityEngine;

namespace ScreenScripts
{
    public class DeleteSaveFileScreen : MonoBehaviour
    {
        // Start is called before the first frame update
        void Awake()
        {
            GameManager.StartListening("ToggleDeleteSaveFileScreen", ToggleDeleteSaveFileScreen);
            ToggleDeleteSaveFileScreen();
        }

        private void OnDestroy()
        {
            GameManager.StopListening("ToggleDeleteSaveFileScreen");
        }

        private void ToggleDeleteSaveFileScreen(string f ="")
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }
    }
}
