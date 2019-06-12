using UnityEngine;

namespace Wakaman.AI
{
    public class AIBehaviourGhostLeaveCage : StateMachineBehaviour
    {
        private Pathfinding pathfinding;
        private GhostAI ghost = null;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            ghost = !ghost ? animator.GetComponent<GhostAI>() : ghost;
            pathfinding = !pathfinding ? Pathfinding.instance : pathfinding;
            ghost.MoveIgnoreCollision(new Vector3[] { pathfinding.CageCenter, pathfinding.CageExit });
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (ghost.HasStopped)
                animator.SetBool("is_chasing", true);
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool("is_leaving_cage", false);
        }
    }
}
