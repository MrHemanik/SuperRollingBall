
// Die Daten zum speichern müssen 1 Objekt sein, weshalb ich dieses Objekt angelegt habe (Vorher nur int[])
[System.Serializable]
public class PlayerData
{
    public int maxUnlockedLevel;
    public  int maxLivePoints;
    public int collectedCoinsTotal;
    public float[] timeHighscore;
    public bool[] permaUpgrades;
    
    public PlayerData(int u, int l, int c, float[] h, bool[] p)
    {
        maxUnlockedLevel = u;
        maxLivePoints = l;
        collectedCoinsTotal = c;
        timeHighscore = h;
        permaUpgrades = p;
    }
}
