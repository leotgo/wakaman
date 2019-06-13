using UnityEngine;
using UnityEngine.Tilemaps;

namespace Wakaman.AI
{
    public class AIBehaviourGhostCage : StateMachineBehaviour
    {
        private Tilemap tilemap = null;
        private GhostAI ghost = null;
        private bool isMovingDown = true;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            ghost = !ghost ? animator.GetComponent<GhostAI>() : ghost;
            tilemap = !tilemap ? MapInfo.CollisionMap : tilemap;
            // The following line is weird, but is done because of lazyness to make it work 
            // with the line in OnStateUpdate: "isMovingDown = !isMovingDown"
            isMovingDown = !ghost.StartMovingDownInCage;
            animator.SetBool("is_retreating", false);
            if (!ghost.StartInCage)
            {
                animator.SetBool("is_chasing", true);
                animator.SetBool("is_in_cage", false);
            }
            ghost.CurrentSpeed = ghost.ScaredSpeed;
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!ghost.IsReady)
                return;

            if (ghost.HasStopped && ghost.StartInCage)
            {
                if (ghost.CanLeaveCage)
                {
                    animator.SetBool("is_leaving_cage", true);
                }
                else
                {
                    isMovingDown = !isMovingDown;
                    Vector3 targetOffset = (isMovingDown ? Vector3.down : Vector3.up) * tilemap.cellSize.y / 2f;
                    Vector3 target = ghost.CagePosition + targetOffset;
                    ghost.MoveIgnoreCollision(new Vector3[] { target, ghost.CagePosition });
                }
            }
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool("is_in_cage", false);
            ghost.CurrentSpeed = ghost.DefaultSpeed;
        }
    }
}
