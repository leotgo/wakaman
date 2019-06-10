using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Wakaman.Entities
{
    [RequireComponent(typeof(Animator))]
    public class Player : MonoBehaviour
    {
        // Gameplay
        [Header("Gameplay")]
        [SerializeField] private float speed = 1f;

        // Component Refs 
        private Animator anim;

        // Collision masks
        [Header("Collision masks")]
        [SerializeField] private LayerMask lmWalls;
        [SerializeField] private LayerMask lmInteractors;

        // Runtime vars
        private bool isMoving;
        private Vector3Int lastMoveDir;

        // -------------------------- //
        // Monobehaviour
        // -------------------------- //

        private void Start()
        {
            anim = GetComponent<Animator>();
            isMoving = false;
        }

        private void Update()
        {
            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");
            Vector3Int dir = GetMovementDir(x, y);
            if (dir.magnitude > 0f)
            {
                if (!isMoving)
                    Move(dir);
            }

            if (Input.GetButtonDown("Fire1"))
                Die();
        }

        // -------------------------- //
        // Actions
        // -------------------------- //

        private void Die()
        {
            Game.Death();
            anim.SetBool("is_dead", true);
        }

        // -------------------------- //
        // Movement-related methods
        // -------------------------- //

        // GetMovementDir: Returns a discrete movement direction
        //                 based on Horizontal and Vertical axes.
        private Vector3Int GetMovementDir(float x, float y)
        {
            // Movement direction priority like in the old arcade games.
            // Right > Left > Down > Up
            Vector3Int dir = Vector3Int.zero;
            if (x > 0f)
                dir = Vector3Int.right;
            else if (x < 0f)
                dir = Vector3Int.left;
            else if (y < 0f)
                dir = Vector3Int.down;
            else if (y > 0f)
                dir = Vector3Int.up;

            return dir;
        }

        // Move: Checks for wall collision and performs intended player
        //       movement when possible.
        private void Move(Vector3Int dir)
        {
            bool isHorizontal = Mathf.Abs(dir.x) > 0f;
            // Adjacent tiles are checked to see if the player is close to a
            // corner to facilitate turning.
            var adjDir1 = isHorizontal ? Vector3Int.up : Vector3Int.left;
            var adjDir2 = isHorizontal ? Vector3Int.down : Vector3Int.right;
            // Checks if adjacent tiles are not opposed to the current player
            // movement direction.
            bool adj1IsInverseDir = adjDir1 == (lastMoveDir * -1);
            bool adj2IsInverseDir = adjDir2 == (lastMoveDir * -1);

            // Checks if next tile is a wall.
            if (!CheckMoveCollision(dir))
            {
                StartCoroutine(MoveTileRoutine(dir));
            }
            // Checks if tiles adjacent to the next are walls and assists
            // player movement when turning corners (less precision required!)
            else if (!adj1IsInverseDir && !CheckMoveCollision(dir + adjDir1))
            {
                StartCoroutine(MoveTileRoutine(adjDir1));
            }
            else if (!adj2IsInverseDir && !CheckMoveCollision(dir + adjDir2))
            {
                StartCoroutine(MoveTileRoutine(adjDir2));
            }
        }

        // MoveTileRoutine: Smoothly move the player from his current 
        //                  to the next tile.
        IEnumerator MoveTileRoutine(Vector3Int dir)
        {
            isMoving = true;
            anim.SetBool("is_moving", true);
            anim.SetFloat("dir_x", dir.x);
            anim.SetFloat("dir_y", dir.y);

            float dt = Time.fixedDeltaTime;
            float step = speed * dt;
            Vector3 targetPos = GetCellPosByOffset(dir);
            while (Vector3.Distance(transform.position, targetPos) > step)
            {

                transform.position = Vector3.MoveTowards(transform.position, targetPos, step);
                var coll = Physics2D.OverlapPoint(transform.position, lmInteractors.value);
                if (coll != null)
                {
                    coll.GetComponent<IInteractable>()?.OnInteract(this);
                    var teleporterPos = coll.GetComponent<Teleporter>()?.target?.position;
                    targetPos = teleporterPos.HasValue ? GetCellPosByOffset(dir) : targetPos;
                }
                yield return new WaitForFixedUpdate();
            }
            transform.position = targetPos;
            lastMoveDir = dir;
            isMoving = false;
            anim.SetBool("is_moving", false);
        }

        // CheckMoveCollision: Returns true if the player will collide with 
        //                     a wall in a given movement direction.
        private bool CheckMoveCollision(Vector3Int dir)
        {
            Vector3 nextCellPos = GetCellPosByOffset(dir);
            var coll = Physics2D.OverlapPoint(nextCellPos, lmWalls.value);
            Debug.DrawRay(transform.position, nextCellPos - transform.position);
            return coll != null;
        }

        private Vector3 GetCellPosByOffset(Vector3Int offset)
        {
            Tilemap tilemap = Game.GetTilemap();
            Vector3Int currCell = tilemap.WorldToCell(transform.position);
            Vector3 pos = tilemap.GetCellCenterWorld(currCell + offset);
            return pos;
        }
    }
}