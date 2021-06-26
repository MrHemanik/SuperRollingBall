using ManageObjectScripts;
using UnityEngine;
using UnityEngine.UI;

namespace ScreenScripts
{
    public class IsLevelLockedScript : MonoBehaviour
    {
        public int levelNumber;

        private void Start()
        {
            if (GameManager.GetMaxUnlockedLevel() < levelNumber) return;
            gameObject.transform.Find("Locked").gameObject.SetActive(false); //Macht das Overlay aus
            gameObject.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => //Setzt Knopflogik
            {
                GameManager.TriggerEvent("LoadScene", levelNumber + "_Level");
            });
        }
    }
}