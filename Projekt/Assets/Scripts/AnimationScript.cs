using UnityEngine;

public class AnimationScript : MonoBehaviour
{
    private void Awake()
    {
        GameManager.StartListening("StartCameraAnimation", SetAnimation);
        GameManager.TriggerEvent("FetchCurrentLevel");
    }

    private void OnDestroy()
    {
        GameManager.StopListening("StartCameraAnimation");
    }

    private void SetAnimation(string input)
    {
        // Reminder: Die Animation muss im Animator hinzugefügt und die transition gesetzt sein!
        gameObject.GetComponent<Animator>().SetInteger(Animator.StringToHash("Level"),int.Parse(input));
    }
}
