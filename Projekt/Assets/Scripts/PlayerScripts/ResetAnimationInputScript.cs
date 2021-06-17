using UnityEngine;

namespace PlayerScripts
{
    public class ResetAnimationInputScript : StateMachineBehaviour
    {
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state


        //Der State von PuzzleSolved wird wieder auf 0 gesetzt, damit es nicht öfters getriggered wird!
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetInteger(Animator.StringToHash("PuzzleSolved"), 0);
        }
    }
}