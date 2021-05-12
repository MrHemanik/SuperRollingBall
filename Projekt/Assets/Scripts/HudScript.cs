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
    private void Start()
    {
        liveText = GameObject.Find("LiveNumber").GetComponent<TextMeshProUGUI>();
        coinText = GameObject.Find("CoinNumber").GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateDisplay(int liveCount, int coinCount)
    {
        liveText.text = liveCount.ToString();
        coinText.text = coinCount.ToString();
    }
}
