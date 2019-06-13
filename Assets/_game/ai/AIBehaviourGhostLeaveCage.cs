using UnityEngine;

namespace Wakaman.AI
{
    public class AIBehaviourGhostLeaveCage : StateMachineBehaviour
    {
        private GhostAI ghost = null;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            ghost = !ghost ? animator.GetComponent<GhostAI>() : ghost;
            ghost.MoveIgnoreCollision(new Vector3[] { MapInfo.CageCenter, MapInfo.CageExit });
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (ghost.HasStopped)
                animator.SetBool("is_scattering", true);
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool("is_leaving_cage", false);
            animator.SetBool("is_frightened", false);
            animator.SetBool("is_retreating", false);
        }
    }
}
