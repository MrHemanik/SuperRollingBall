using System.Collections;
using System.Collections.Generic;
using ManageObjectScripts;
using UnityEngine;

public class StartTimerOnExitScript : StateMachineBehaviour
{


    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameManager.TriggerEvent("LevelTimerStart");
    }
}
