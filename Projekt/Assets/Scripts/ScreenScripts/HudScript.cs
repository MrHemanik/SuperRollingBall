using TMPro;
using UnityEngine;

namespace ScreenScripts
{
    public class HudScript : MonoBehaviour
    {
        private TextMeshProUGUI _liveText;
        private TextMeshProUGUI _coinText;
        private void Awake()
        {
            GameManager.StartListening ("UpdateLiveDisplay", UpdateLiveDisplay);
            GameManager.StartListening ("UpdateCoinDisplay", UpdateCoinDisplay);
        }
        private void OnDestroy()
        {
            GameManager.StopListening ("UpdateLiveDisplay", UpdateLiveDisplay);
            GameManager.StopListening ("UpdateCoinDisplay", UpdateCoinDisplay);
        }
        private void Start()
        {
            _liveText = GameObject.Find("LiveNumber").GetComponent<TextMeshProUGUI>();
            _coinText = GameObject.Find("CoinNumber").GetComponent<TextMeshProUGUI>();
            GameManager.TriggerEvent("FetchDisplayData");
        }
        private void UpdateLiveDisplay(string liveCount)
        {
            _liveText.text = liveCount;
        }
        private void UpdateCoinDisplay(string coinCount)
        {
            _coinText.text = coinCount;
        }
    }
}
