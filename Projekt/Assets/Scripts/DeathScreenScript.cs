using UnityEngine;

public class DeathScreenScript : MonoBehaviour
{
    private void Awake()
    {
        GameManager.StartListening("OpenDeathScreen", OpenDeathScreen);
        GameManager.StartListening ("CloseDeathScreen", CloseDeathScreen);
    }

    // Update is called once per frame
    private void OpenDeathScreen(float f)
    {
        gameObject.SetActive(true);
    }
    private void CloseDeathScreen(float f)
    {
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }
}
