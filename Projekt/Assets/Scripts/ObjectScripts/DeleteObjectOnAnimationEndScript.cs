using UnityEngine;

namespace ObjectScripts
{
    public class DeleteObjectOnAnimationEndScript : StateMachineBehaviour
    {
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Destroy(animator.gameObject.transform.parent.gameObject);
        }
    }
}