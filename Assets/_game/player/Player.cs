using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
using Wakaman.Utilities;

namespace Wakaman.Entities
{
    [RequireComponent(typeof(Animator))]
    public class Player : MonoBehaviour
    {
        // Singleton
        public static Player instance = null;

        // Gameplay
        [Header("Gameplay")]
        [SerializeField] private float speed = 1f;
        [SerializeField] private float inputDelayTolerance = 0.7f;

        // Component Refs 
        private Animator anim;
        private SpriteRenderer spr;

        // Collision masks
        [Header("Collision masks")]
        [SerializeField] private LayerMask lmWalls;
        [SerializeField] private LayerMask lmInteractors;

        // Runtime vars
        private Vector3 startPos;
        private bool isDead;
        private bool isMoving;
        private bool isFrozen;
        private bool isReady;
        private float lastInputDirTime;
        private Vector3Int lastInputDir;
        private Vector3Int currentMoveDir;
        private Vector3Int lastMoveDir;

        public Vector3Int MoveDir {
            get => currentMoveDir;
        }
        public bool IsDead {
            get => isDead;
        }
        public bool IsReady {
            get => isReady;
        }
        public bool IsFrozen {
            get => isFrozen;
        }

        // -------------------------- //
        // Monobehaviour
        // -------------------------- //

        private void Start()
        {
            if (!instance)
                instance = this;

            anim = GetComponent<Animator>();
            spr = GetComponentInChildren<SpriteRenderer>();
            startPos = transform.position;

            GameEvents.onFinishLevel += OnFinishLevel;
            GameEvents.onMatchStart += OnMatchStart;
            GameEvents.onRoundStart += Respawn;
        }

        private void Update()
        {
            if (!isReady)
                return;

            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");
            var dir = GetMovementDir(x, y);

            lastInputDirTime += Time.deltaTime;
            if(lastInputDirTime > inputDelayTolerance)
                lastInputDir = Vector3Int.zero;

            if (dir.magnitude > 0f)
            {
                // Buffers last input done by the player for a
                // timing tolerance when turning corners.
                lastInputDir = dir;
                lastInputDirTime = 0f;
            }

            if (!isMoving)
            {
                if (lastInputDir.magnitude > 0f)
                {
                    bool moved = Move(lastInputDir);
                    if (!moved)
                        Move(currentMoveDir);
                }
                else
                    Move(currentMoveDir);
            }

            // Cheat code to quickly suicide
            if (Input.GetKeyDown(KeyCode.F10))
                Die();
        }

        // -------------------------- //
        // Actions
        // -------------------------- //

        private void Respawn()
        {
            isDead = false;
            isMoving = false;
            isFrozen = false;
            this.Delay(Game.RESPAWN_FREEZE_TIME, () => { isReady = true; });

            anim.SetBool("is_moving", false);
            anim.SetBool("is_dead", false);
            anim.SetFloat("dir_x", 1f);
            anim.SetFloat("dir_y", 0f);

            transform.position = startPos;
            lastInputDir = Vector3Int.zero;
            currentMoveDir = Vector3Int.right;
        }

        private void Freeze(bool frozen)
        {
            isFrozen = frozen;
        }

        public void Die()
        {
            if (isDead)
                return;

            StopAllCoroutines();
            StartCoroutine(DeathRoutine());
        }

        private IEnumerator DeathRoutine()
        {
            isReady = false;
            isDead = true;
            bool isGameOver = Game.Lives == 0;
            if (isGameOver)
                Game.IsGameOver = true;
            GameEvents.Death();
            Time.timeScale = 0f;
            yield return new WaitForSecondsRealtime(Game.DEATH_STUTTER_TIME);
            Time.timeScale = 1f;
            anim.SetBool("is_dead", true);

            if (!isGameOver)
            {
                yield return new WaitForSecondsRealtime(Game.ROUND_START_TIME);
                Respawn();
                Freeze(true);
                yield return new WaitForSecondsRealtime(Game.RESPAWN_FREEZE_TIME);
                Freeze(false);
                isReady = true;
            }
        }

        // -------------------------- //
        // Events
        // -------------------------- //

        private void OnMatchStart()
        {
            isReady = false;
            StopAllCoroutines();
        }

        private void OnFinishLevel()
        {
            isReady = false;
            StopAllCoroutines();
        }

        // -------------------------- //
        // Movement
        // -------------------------- //

        // Move: Checks for wall collision and performs intended player
        //       movement when possible.
        private bool Move(Vector3Int dir)
        {
            if (isDead || isFrozen)
                return false;

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
            if (!CheckWallCollision(dir))
            {
                StartCoroutine(MoveTileRoutine(dir));
                return true;
            }
            // Checks if tiles adjacent to the next are walls and assists
            // player movement when turning corners (less precision required!)
            else if (!adj1IsInverseDir && !CheckWallCollision(dir + adjDir1))
            {
                StartCoroutine(MoveTileRoutine(adjDir1));
                return true;
            }
            else if (!adj2IsInverseDir && !CheckWallCollision(dir + adjDir2))
            {
                StartCoroutine(MoveTileRoutine(adjDir2));
                return true;
            }
            else
            {
                // Checks if there are any interactor objects in player's position
                // This is redundant code in case the player is idle (by not turning
                // at any wall collision).
                var coll = Physics2D.OverlapPoint(transform.position, lmInteractors.value);
                if (coll != null)
                {
                    coll.GetComponent<IInteractable>()?.OnInteract(this);
                }
                return false;
            }
        }

        // MoveTileRoutine: Smoothly move the player from his current 
        //                  to the next tile.
        IEnumerator MoveTileRoutine(Vector3Int dir)
        {
            currentMoveDir = dir;
            isMoving = true;
            anim.SetBool("is_moving", true);
            anim.SetFloat("dir_x", dir.x);
            anim.SetFloat("dir_y", dir.y);

            float dt = Time.fixedDeltaTime;
            float step = speed * dt;
            Vector3 targetPos = GetCellPosByOffset(dir);
            while (!isDead && Vector3.Distance(transform.position, targetPos) > step)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, step);
                // Checks if there are any interactor objects in player's position
                var coll = Physics2D.OverlapPoint(transform.position, lmInteractors.value);
                if (coll != null)
                {
                    coll.GetComponent<IInteractable>()?.OnInteract(this);
                    // If interactor is a teleporter, the rest of the routine should
                    // move the player to the teleporter's target destination.
                    var teleporterPos = coll.GetComponent<Teleporter>()?.target?.position;
                    targetPos = teleporterPos.HasValue ? GetCellPosByOffset(dir) : targetPos;
                }
                yield return new WaitForFixedUpdate();
            }
            if(!isDead)
                transform.position = targetPos;

            lastMoveDir = dir;
            isMoving = false;
            anim.SetBool("is_moving", false);
        }

        // -------------------------- //
        // Helpers
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

        private bool CheckWallCollision(Vector3Int dir)
        {
            Vector3 nextCellPos = GetCellPosByOffset(dir);
            var coll = Physics2D.OverlapPoint(nextCellPos, lmWalls.value);
            return coll != null;
        }

        private Vector3 GetCellPosByOffset(Vector3Int offset)
        {
            Vector3Int currCell = MapInfo.CollisionMap.WorldToCell(transform.position);
            Vector3 pos = MapInfo.CollisionMap.GetCellCenterWorld(currCell + offset);
            return pos;
        }
    }
}