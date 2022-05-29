using ManageObjectScripts;
using TMPro;
using UnityEngine;

namespace ScreenScripts
{
    public class HudScript : MonoBehaviour
    {
        //Variablen
        private TextMeshProUGUI _liveText;
        private TextMeshProUGUI _coinText;
        private RectTransform _hitPointBar;

        private RectTransform _jumpChargeBar;

        //Kamera
        private GameObject _xOverlay;
        private GameObject _yOverlay;
        private GameObject _fixedOverlay;

        private void Awake()
        {
            GameManager.StartListening("UpdateLiveDisplay", UpdateLiveDisplay);
            GameManager.StartListening("UpdateCoinDisplay", UpdateCoinDisplay);
            GameManager.StartListening("UpdateHitPointDisplay", UpdateHitPointDisplay);
            GameManager.StartListening("UpdateXAxisDisplay", UpdateXAxisDisplay);
            GameManager.StartListening("UpdateYAxisDisplay", UpdateYAxisDisplay);
            GameManager.StartListening("UpdateFixedDisplay", UpdateFixedDisplay);
        }

        private void OnDestroy()
        {
            GameManager.StopListening("UpdateLiveDisplay");
            GameManager.StopListening("UpdateCoinDisplay");
            GameManager.StopListening("UpdateHitPointDisplay");
            GameManager.StopListening("UpdateXAxisDisplay");
            GameManager.StopListening("UpdateYAxisDisplay");
            GameManager.StopListening("UpdateFixedDisplay");
        }

        private void Start()
        {
            _liveText = GameObject.Find("LiveNumber").GetComponent<TextMeshProUGUI>();
            _coinText = GameObject.Find("CoinNumber").GetComponent<TextMeshProUGUI>();
            _hitPointBar = GameObject.Find("HitPointBar").GetComponent<RectTransform>();
            _jumpChargeBar = GameObject.Find("JumpChargeBar").GetComponent<RectTransform>();
            _xOverlay = GameObject.Find("XOverlay").GetComponent<RectTransform>().gameObject;
            _yOverlay = GameObject.Find("YOverlay").GetComponent<RectTransform>().gameObject;
            _fixedOverlay = GameObject.Find("FixedOverlay").GetComponent<RectTransform>().gameObject;
            _yOverlay.SetActive(false);
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
            _hitPointBar.sizeDelta = new Vector2(float.Parse(hitPoints) * 400, 50);
        }

        public void
            UpdateJumpChargeDisplay(
                float jumpCharge) //Hier auf Public gemacht, um zu vergleichen, ob Event oder Direktaufruf besser ist.
        {
            _jumpChargeBar.sizeDelta = new Vector2(jumpCharge * 400, 50);
        }

        public void UpdateXAxisDisplay(string value)
        {
            _xOverlay.SetActive(bool.Parse(value));
        }

        public void UpdateYAxisDisplay(string value)
        {
            _yOverlay.SetActive(bool.Parse(value));
        }

        public void UpdateFixedDisplay(string value)
        {
            _fixedOverlay.SetActive(bool.Parse(value));
        }
    }
}