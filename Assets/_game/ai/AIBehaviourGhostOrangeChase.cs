using UnityEngine;
using UnityEngine.Tilemaps;
using Wakaman.Entities;

namespace Wakaman.AI
{
    public class AIBehaviourGhostOrangeChase : StateMachineBehaviour
    {
        private Player player;
        private Tilemap tilemap;
        private Pathfinding pathfinding;
        private GhostAI ghost = null;
        [SerializeField] private float updateTargetPosTime = 1.0f;
        [SerializeField] private int fleeDistance = 8;
        private float currUpdateTime;
        private float currChaseTime;
        private bool isFleeing;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            player = !player ? Player.instance : player;
            tilemap = !tilemap ? MapInfo.CollisionMap : tilemap;
            pathfinding = !pathfinding ? Pathfinding.instance : pathfinding;
            ghost = !ghost ? animator.GetComponent<GhostAI>() : ghost;

            ghost.MoveTo(player.transform.position);
            currUpdateTime = 0.0f;
            currChaseTime = 0.0f;
            isFleeing = false;
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            currChaseTime += Time.deltaTime;
            currUpdateTime += Time.deltaTime;
            if (currChaseTime > ghost.ChaseTime && !Game.IsNearLevelEnd())
            {
                animator.SetBool("is_scattering", true);
            }
            else
            {
                var playerCellPos = tilemap.WorldToCell(player.transform.position);
                var ghostCellPos = tilemap.WorldToCell(ghost.transform.position);
                if (!ghost.HasStopped && pathfinding.GetTileDistance(ghostCellPos, playerCellPos) < fleeDistance && !isFleeing)
                {
                    isFleeing = true;
                    currUpdateTime = 0f;
                    ghost.MoveTo(ghost.CornerPositions[0].position);
                }
                else if (ghost.HasStopped || currUpdateTime > updateTargetPosTime)
                {
                    isFleeing = false;
                    currUpdateTime = 0f;
                    ghost.MoveTo(player.transform.position);
                }
            }
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool("is_chasing", false);
        }
    }
}
