using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wakaman.AI
{
    public class AIBehaviourGhostFrightened : StateMachineBehaviour
    {
        private GhostAI ghost = null;
        private float currScaredTime;
        private int currPosIndex = 0;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            ghost = !ghost ? animator.GetComponent<GhostAI>() : ghost;
            ghost.isFrightened = true;
            currScaredTime = 0.0f;
            currPosIndex = 0;
            ghost.MoveTo(ghost.CornerPositions[0].position);
            ghost.CurrentSpeed = ghost.ScaredSpeed;
            ghost.SpriteAnimator.SetBool("is_frightened", true);
            ghost.SpriteAnimator.SetBool("is_flashing", true);
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (currScaredTime > Game.GetGhostScaredTime())
            {
                ghost.isFrightened = false;
                animator.SetBool("is_frightened", false);
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
            ghost.isFrightened = false;
            ghost.SpriteAnimator.SetBool("is_frightened", false);
            ghost.SpriteAnimator.SetBool("is_flashing", false);
            ghost.CurrentSpeed = ghost.DefaultSpeed;
        }
    }
}
