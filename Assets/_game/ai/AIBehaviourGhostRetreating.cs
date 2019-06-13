using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wakaman.AI
{
    public class AIBehaviourGhostRetreating : StateMachineBehaviour
    {
        private GhostAI ghost = null;

        private bool movingToCageExit;
        private bool movingToCagePos;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            ghost = !ghost ? animator.GetComponent<GhostAI>() : ghost;
            ghost.isRetreating = true;
            ghost.MoveTo(MapInfo.CageExit);
            movingToCageExit = true;
            movingToCagePos = false;

            ghost.SpriteAnimator.SetBool("is_retreating", true);
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (movingToCageExit && ghost.HasStopped)
            {
                movingToCageExit = false;
                movingToCagePos = true;
                ghost.MoveIgnoreCollision(new Vector3[] { MapInfo.CageCenter, ghost.CagePosition });
            }
            else if (movingToCagePos && ghost.HasStopped)
            {
                animator.SetBool("is_frightened", false);
                animator.SetBool("is_retreating", false);
                animator.SetBool("is_in_cage", true);
                animator.SetBool("is_leaving_cage", false);
            }
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool("is_retreating", false);
            animator.SetBool("is_in_cage", true);
            animator.SetBool("is_leaving_cage", false);

            ghost.SpriteAnimator.SetBool("is_frightened", false);
            ghost.SpriteAnimator.SetBool("is_retreating", false);

            ghost.isRetreating = false;
            ghost.isFrightened = false;

            GameEvents.GhostEndRetreat();
        }
    }
}
