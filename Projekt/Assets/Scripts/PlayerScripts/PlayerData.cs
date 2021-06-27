// Die Daten zum speichern müssen 1 Objekt sein, weshalb ich dieses Objekt angelegt habe (Vorher nur int[])

[System.Serializable]
public class PlayerData //Sobald ich es in NameSpace PlayerScripts packe, funktioniert es nicht mehr
{
    public int maxUnlockedLevel;
    public int collectedCoinsTotal;
    public float[] timeHighscore;
    public bool[] permaUpgrades;

    public PlayerData(int u, int c, float[] h, bool[] p)
    {
        maxUnlockedLevel = u;
        collectedCoinsTotal = c;
        timeHighscore = h;
        permaUpgrades = p;
    }
}