using UnityEngine;

public class VictoryScreenScript : MonoBehaviour
{
    private void Awake()
    {
        GameManager.StartListening("OpenVictoryScreen", OpenVictoryScreen);
        GameManager.StartListening ("CloseVictoryScreen", CloseVictoryScreen);
    }

    // Update is called once per frame
    private void OpenVictoryScreen(float f)
    {
        gameObject.SetActive(true);
    }
    private void CloseVictoryScreen(float f)
    {
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }
}
