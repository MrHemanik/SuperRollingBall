using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HudScript : MonoBehaviour
{
    /* GameManager */
    private GameManager _gm;
    private TextMeshProUGUI liveText;
    private TextMeshProUGUI coinText;
    // Start is called before the first frame update
    private void Start()
    {
        _gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        liveText = GameObject.Find("LiveNumber").GetComponent<TextMeshProUGUI>();
        coinText = GameObject.Find("CoinNumber").GetComponent<TextMeshProUGUI>();
        liveText.text = _gm.livePoints.ToString();
        coinText.text = _gm.collectedCoinsInLevel.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateDisplay()
    {
        liveText.text = _gm.livePoints.ToString();
        coinText.text = _gm.collectedCoinsInLevel.ToString();
    }
}
