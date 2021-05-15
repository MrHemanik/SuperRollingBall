using System;
using UnityEngine;

namespace ScreenScripts
{
    public class MissionScreenScript : MonoBehaviour
    {

        void Awake()
        {
            GameManager.StartListening("DestroyMissionGoalScreen", DestroySelf);
        }

        private void OnDestroy()
        {
            GameManager.StopListening("DestroyMissionGoalScreen", DestroySelf);
        }

        private void DestroySelf(string s)
        {
            Destroy(gameObject);
        }
    }
}
