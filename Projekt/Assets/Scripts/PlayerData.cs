using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int maxUnlockedLevel;
    public  int maxLivePoints;
    public int collectedCoinsTotal;
    public float[] timeHighscore;
    
    public PlayerData(int u, int l, int c, float[] h)
    {
        maxUnlockedLevel = u;
        maxLivePoints = l;
        collectedCoinsTotal = c;
        timeHighscore = h;
    }
}
