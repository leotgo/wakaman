using UnityEngine;
using UnityEngine.Tilemaps;
using Wakaman.Entities;

namespace Wakaman.AI
{
    public class AIBehaviourGhostPinkChase : StateMachineBehaviour
    {
        private Player player;
        private Tilemap tilemap;
        private Pathfinding pathfinding;
        private GhostAI ghost = null;
        [SerializeField] private float updateTargetPosTime = 1.0f;
        private float currUpdateTime;
        private float currChaseTime;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            player = !player ? Player.instance : player;
            tilemap = !tilemap ? MapInfo.CollisionMap : tilemap;
            pathfinding = !pathfinding ? Pathfinding.instance : pathfinding;
            ghost = !ghost ? animator.GetComponent<GhostAI>() : ghost;

            ghost.MoveTo(player.transform.position);
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
                if (ghost.HasStopped || currUpdateTime > updateTargetPosTime)
                {
                    currUpdateTime = 0f;
                    var playerCellPos = tilemap.WorldToCell(player.transform.position);
                    var ghostCellPos = tilemap.WorldToCell(ghost.transform.position);
                    var dir = playerCellPos - ghostCellPos;
                    dir.Clamp(Vector3Int.one * -1, Vector3Int.one);
                    if(pathfinding.GetTileDistance(ghostCellPos, playerCellPos) <= 4 && dir != player.MoveDir)
                        ghost.MoveTo(playerCellPos);
                    else
                        ghost.MoveTo(pathfinding.GetNearestTileInBounds(playerCellPos, player.MoveDir * 4));
                }
            }
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool("is_chasing", false);
        }
    }
}
