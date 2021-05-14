using UnityEngine;

public class DeathScreenScript : MonoBehaviour
{
    private void Awake()
    {
        
        CloseDeathScreen("");
        GameManager.StartListening("OpenDeathScreen", OpenDeathScreen);
        GameManager.StartListening ("CloseDeathScreen", CloseDeathScreen);
    }
    private void OnDestroy()
    {
        GameManager.StopListening("OpenDeathScreen", OpenDeathScreen);
        GameManager.StopListening ("CloseDeathScreen", CloseDeathScreen);
    }

    // Update is called once per frame
    private void OpenDeathScreen(string s)
    {
        gameObject.SetActive(true);
    }
    private void CloseDeathScreen(string s)
    {
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }
}
