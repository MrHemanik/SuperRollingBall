using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ScreenScripts
{
    public class HudScript : MonoBehaviour
    {
        private TextMeshProUGUI _liveText;
        private TextMeshProUGUI _coinText;
        private RectTransform _hitPointBar;
        private RectTransform _jumpChargeBar;
        private void Awake()
        {
            GameManager.StartListening ("UpdateLiveDisplay", UpdateLiveDisplay);
            GameManager.StartListening ("UpdateCoinDisplay", UpdateCoinDisplay);
            GameManager.StartListening ("UpdateHitPointDisplay", UpdateHitPointDisplay);
        }
        private void OnDestroy()
        {
            GameManager.StopListening ("UpdateLiveDisplay");
            GameManager.StopListening ("UpdateCoinDisplay");
            GameManager.StopListening ("UpdateHitPointDisplay");
        }
        private void Start()
        {
            _liveText = GameObject.Find("LiveNumber").GetComponent<TextMeshProUGUI>();
            _coinText = GameObject.Find("CoinNumber").GetComponent<TextMeshProUGUI>();
            _hitPointBar = GameObject.Find("HitPointBar").GetComponent<RectTransform>();
            _jumpChargeBar = GameObject.Find("JumpChargeBar").GetComponent<RectTransform>();
            
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

        private void UpdateHitPointDisplay(string hitPoints)
        {
            _hitPointBar.sizeDelta = new Vector2(float.Parse(hitPoints)*400,50);
        }

        public void UpdateJumpChargeDisplay(float jumpCharge) //Hier auf Public gemacht, um zu vergleichen, ob Event oder Direktaufruf besser ist.
        {
            _jumpChargeBar.sizeDelta = new Vector2(jumpCharge*400,50);
        }
    }
}
