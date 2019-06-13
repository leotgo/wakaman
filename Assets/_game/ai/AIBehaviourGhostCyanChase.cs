using UnityEngine;
using UnityEngine.Tilemaps;
using Wakaman.Entities;

namespace Wakaman.AI
{
    public class AIBehaviourGhostCyanChase : StateMachineBehaviour
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
                    Vector3Int playerCellPos = tilemap.WorldToCell(player.transform.position);
                    Vector3Int ghostCellPos = tilemap.WorldToCell(ghost.transform.position);
                    Vector3Int dir = playerCellPos - ghostCellPos;
                    dir.Clamp(Vector3Int.one * -1, Vector3Int.one);
                    if (pathfinding.GetTileDistance(ghostCellPos, playerCellPos) <= 4 && dir != player.MoveDir)
                        ghost.MoveTo(playerCellPos);
                    else
                    {
                        /* 
                         * According to the reference article for Ghost AI:
                         *   To locate Inky (cyan) target:
                         *    1 - Select the position two tiles in front of Pac-Man in his 
                         *        current direction of travel.
                         *    2 - Create a vector from Blinky position to this tile, and 
                         *        then double the length of the vector.
                         *    3 - The ending position of the vector (starting from Blinky)
                         *        is Inky's target.
                         */
                        Vector3Int redGhostCellPos = tilemap.WorldToCell(GhostsManager.instance.RedGhost.transform.position);
                        Vector3Int vectorEnd = playerCellPos + player.MoveDir * 2;
                        Vector3Int vec = (vectorEnd - redGhostCellPos) * 2;
                        ghost.MoveTo(pathfinding.GetNearestTileInBounds(redGhostCellPos, vec));
                    }
                }
            }
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool("is_chasing", false);
        }
    }
}
