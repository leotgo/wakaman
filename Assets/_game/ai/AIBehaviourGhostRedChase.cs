using UnityEngine;
using Wakaman.Entities;

namespace Wakaman.AI
{
    public class AIBehaviourGhostRedChase : StateMachineBehaviour
    {
        private GhostAI ghost = null;
        [SerializeField] private float updateTargetPosTime = 1.0f;
        private float currUpdateTime;
        private float currChaseTime;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            ghost = !ghost ? animator.GetComponent<GhostAI>() : ghost;
            ghost.MoveTo(Player.instance.transform.position);
            currUpdateTime = 0.0f;
            currChaseTime = 0.0f;
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            currChaseTime += Time.deltaTime;
            currUpdateTime += Time.deltaTime;
            if(currChaseTime > ghost.ChaseTime && !Game.IsNearLevelEnd())
            {
                animator.SetBool("is_scattering", true);
            }
            else
            {
                if (ghost.needsTargetRecalc || ghost.HasStopped || currUpdateTime > updateTargetPosTime)
                {
                    ghost.needsTargetRecalc = false;
                    currUpdateTime = 0f;
                    ghost.MoveTo(Player.instance.transform.position);
                }
            }
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool("is_chasing", false);
        }
    }
}
