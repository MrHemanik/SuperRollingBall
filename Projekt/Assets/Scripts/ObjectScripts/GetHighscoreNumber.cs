using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GetHighscoreNumber : MonoBehaviour
{
    public int gameIndex;
    void Start()
    {
        float highscore = GameManager.GetHighscoreFromIndex(gameIndex);
        if(highscore > 0) gameObject.GetComponent<TextMeshProUGUI>().text = highscore.ToString("0.00");
    }
}
