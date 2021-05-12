using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HudScript : MonoBehaviour
{
    /* GameManager */
    private TextMeshProUGUI liveText;
    private TextMeshProUGUI coinText;
    // Start is called before the first frame update
    private void Awake()
    {
        GameManager.StartListening ("UpdateLiveDisplay", UpdateLiveDisplay);
        GameManager.StartListening ("UpdateCoinDisplay", UpdateCoinDisplay);
    }

    private void Start()
    {
        liveText = GameObject.Find("LiveNumber").GetComponent<TextMeshProUGUI>();
        coinText = GameObject.Find("CoinNumber").GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateLiveDisplay(float liveCount)
    {
        liveText.text = liveCount.ToString();
    }public void UpdateCoinDisplay(float coinCount)
    {
        coinText.text = coinCount.ToString();
    }
}
