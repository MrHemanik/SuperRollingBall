using UnityEngine;

namespace ManageObjectScripts
{
    public class TimerScript : MonoBehaviour
    {
        public string eventName;
        public float duration;

        private void Start()
        {
            Destroy(gameObject, duration);
        }

        public void SetValues(string newEventName, float newDuration)
        {
            eventName = newEventName;
            duration = newDuration;
        }

        private void OnDestroy()
        {
            GameManager.TriggerEvent(eventName);
            Debug.Log(eventName + " wurde durch Timer getriggered");
        }
    }
}