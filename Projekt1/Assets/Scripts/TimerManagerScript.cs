using UnityEngine;

public static class TimerManagerScript 
{
    // Start is called before the first frame update
    
    public static void StartTimer(string eventName, float duration)
    {
        //Input: "eventName, duration", so z.B. "Banane",6.5
        GameObject statusObject = new GameObject("Timer for "+eventName, typeof(TimerScript));
        statusObject.transform.parent = GameObject.Find("Timer").transform; //Macht den Timer zum Child
        TimerScript timerScript = statusObject.GetComponent<TimerScript>();
        timerScript.SetValues(eventName, duration);
        Debug.Log("Timer gestartet");
    }
}
