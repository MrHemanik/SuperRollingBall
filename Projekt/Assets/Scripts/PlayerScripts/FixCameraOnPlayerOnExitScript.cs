using ManageObjectScripts;
using UnityEngine;

namespace PlayerScripts
{
    public class FixCameraOnPlayerOnExitScript : StateMachineBehaviour
    {
        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            GameManager.TriggerEvent("EndCameraAnimation");
        }
    }
}