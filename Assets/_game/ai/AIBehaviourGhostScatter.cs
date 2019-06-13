using UnityEngine;
using Wakaman.Entities;

namespace Wakaman.AI
{
    public class AIBehaviourGhostScatter : StateMachineBehaviour
    {
        private GhostAI ghost = null;
        private float currScatterTime;
        private int currPosIndex = 0;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            ghost = !ghost ? animator.GetComponent<GhostAI>() : ghost;
            currScatterTime = 0.0f;
            currPosIndex = 0;
            ghost.MoveTo(ghost.CornerPositions[0].position);
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            currScatterTime += Time.deltaTime;
            if(currScatterTime > ghost.ScatterTime)
            {
                animator.SetBool("is_chasing", true);
            }
            else
            {
                if (ghost.HasStopped)
                {
                    currPosIndex++;
                    if (currPosIndex >= ghost.CornerPositions.Length)
                        currPosIndex = 0;
                    ghost.MoveTo(ghost.CornerPositions[currPosIndex].position);
                }
            }
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool("is_scattering", false);
        }
    }
}
