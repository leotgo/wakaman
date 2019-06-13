using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Wakaman.Entities;
using Wakaman.Utilities;
using Wakaman.UI;

namespace Wakaman.AI
{
    /*
     * GhostAI: Base class for storing all Ghost AI-related parameters,
     *          as well as providing an API for behaviours (contained in
     *          the AI Controller Animator) to execute actions.
     *
     *          Ghost Behaviour is based on what is described in the
     *          following article:
     *          https://gameinternals.com/post/2072558330/understanding-pac-man-ghost-behavior
     */
    public class GhostAI : MonoBehaviour, IInteractable
    {
        // Definitions
        public enum LeaveCageCondition
        {
            None,
            FixedTimer,
            ConsumeDots,
            ConsumedDotsRatio
        }

        // Editor-Tweakables
        [Header("Gameplay")]
        [SerializeField] private float speed = 0.5f;
        [SerializeField] private float frightenedSpeed = 0.2f;
        [SerializeField] private float defaultChaseTime = 18f;
        [SerializeField] private float defaultScatterTime = 8f;

        [Header("Behaviour in Cage")]
        [SerializeField] private bool startInCage = true;
        [SerializeField] private bool startMovingDownInCage = false;
        [SerializeField] private LeaveCageCondition leaveCageCondition = LeaveCageCondition.None;
        [SerializeField] private float leaveCageTime = 5.0f;
        [SerializeField] private int leaveCageDotsNumber = 30;
        [SerializeField] private float leaveCageDotsRatio = 0.7f;

        [Header("AI System Refs")]
        [SerializeField] private Animator sprAnim;
        [SerializeField] private Animator AIController;
        [SerializeField] private Transform[] cornerPos;
        [SerializeField] private Transform cagePos;
        [SerializeField] private LayerMask lmInteractors;

        [Header("Misc")]
        [SerializeField] private UIGhostScore scoreUI;

        // Component Refs
        private SpriteRenderer spr;

        // Runtime
        private Tilemap tilemap;
        private Vector3 startPos;
        private int currentPosId;
        private Vector3[] currentPath;
        private Vector3Int currentMoveDir;
        private float defaultSpeed;
        private float currentSpeed;
        private bool isReady;
        public bool needsTargetRecalc;
        public bool isFrightened;
        public bool isRetreating;

        // Accessors
        public Animator SpriteAnimator {
            get => sprAnim;
        }
        public Animator AIAnimator {
            get => AIController;
        }
        public float ChaseTime {
            get => defaultChaseTime;
        }
        public float ScatterTime {
            get => defaultScatterTime;
        }
        public bool StartInCage {
            get => startInCage;
        }
        public bool StartMovingDownInCage {
            get => startMovingDownInCage;
        }
        public Transform[] CornerPositions {
            get => cornerPos;
        }
        public Vector3 CagePosition {
            get => cagePos.position;
        }
        public bool HasStopped {
            get { return currentPath.Length == 0; }
        }
        public bool CanLeaveCage {
            get {
                switch (leaveCageCondition)
                {
                    case LeaveCageCondition.None:
                        return true;
                    case LeaveCageCondition.FixedTimer:
                        return Game.TimeSinceSpawn > leaveCageTime;
                    case LeaveCageCondition.ConsumeDots:
                        return Game.ConsumedPellets > leaveCageDotsNumber && Game.TimeSinceSpawn > leaveCageTime;
                    case LeaveCageCondition.ConsumedDotsRatio:
                        return Game.ConsumedPelletsRatio >= leaveCageDotsRatio && Game.TimeSinceSpawn > leaveCageTime;
                    default:
                        return true;
                }
            }
        }
        public float CurrentSpeed {
            get => currentSpeed;
            set {
                currentSpeed = value;
            }
        }
        public float DefaultSpeed {
            get => defaultSpeed;
        }
        public float ScaredSpeed {
            get => frightenedSpeed;
        }
        public bool IsReady {
            get => isReady;
        }

        // -------------------------- //
        // Monobehaviour
        // -------------------------- //

        private void Start()
        {
            spr = GetComponentInChildren<SpriteRenderer>();
            tilemap = MapInfo.CollisionMap;
            startPos = transform.position;
            defaultSpeed = speed;
            isReady = false;

            GameEvents.onDeath += OnDeath;
            GameEvents.onFinishLevel += () =>
            {
                spr.enabled = false;
                isReady = false;
            };
            GameEvents.onMatchStart += () =>
            {
                spr.enabled = false;
                isReady = false;
            };
            GameEvents.onRoundStart += Respawn;
            GameEvents.onStartPowerPellet += GetScared;
            GameEvents.onEndPowerPellet += OnEndPowerPellet;
        }

        private void Update()
        {
            if (!isReady)
                return;

            if (currentPath.Length > 0 && !Player.instance.IsDead && !Player.instance.IsFrozen)
            {
                float dt = Time.deltaTime;
                float step = CurrentSpeed * dt;
                Vector3 target = currentPath[currentPosId];
                Vector2 dir = (target - transform.position).normalized;
                sprAnim.SetFloat("dir_x", dir.x);
                sprAnim.SetFloat("dir_y", dir.y);
                currentMoveDir.Set(Mathf.RoundToInt(dir.x), Mathf.RoundToInt(dir.y), 0);
                transform.position = Vector3.MoveTowards(transform.position, target, step);
                if (Vector3.Distance(transform.position, target) < step)
                {
                    currentPosId++;
                    if (currentPosId >= currentPath.Length)
                    {
                        currentPath = new Vector3[0];
                        currentPosId = 0;
                    }
                }

                var colliders = Physics2D.OverlapPointAll(transform.position, lmInteractors.value);
                if (colliders != null)
                {
                    foreach (var coll in colliders)
                    {
                        coll.GetComponent<Teleporter>()?.OnInteract(this);
                        needsTargetRecalc = true;
                    }
                }
            }
        }

        // -------------------------- //
        // Actions
        // -------------------------- //

        private void Respawn()
        {
            currentPath = new Vector3[0];
            currentPosId = 0;
            currentMoveDir = Vector3Int.left;
            AIController.SetBool("is_in_cage", true);
            AIController.SetBool("is_leaving_cage", false);
            AIController.SetBool("is_chasing", false);
            AIController.SetBool("is_scattering", false);
            AIController.SetBool("is_frightened", false);
            AIController.SetBool("is_retreating", false);
            transform.position = startPos;
            spr.enabled = true;
            isReady = true;
            isFrightened = false;
        }

        private void GetScared()
        {
            isFrightened = true;
            AIController.SetBool("is_frightened", true);
            AIController.SetBool("is_retreating", false);
        }

        public void MoveTo(Vector3 target)
        {
            Vector3Int start = tilemap.WorldToCell(transform.position);
            Vector3Int end = tilemap.WorldToCell(target);
            Vector3Int[] path = Pathfinding.instance.GetPath(start, end, currentMoveDir);

            List<Vector3> pathConv = new List<Vector3>();
            foreach (Vector3Int pos in path)
                pathConv.Add(tilemap.GetCellCenterWorld(pos));
            currentPath = pathConv.ToArray();
            currentPosId = 0;
        }

        public void MoveTo(Vector3Int target)
        {
            Vector3Int start = tilemap.WorldToCell(transform.position);
            Vector3Int end = target;
            Vector3Int[] path = Pathfinding.instance.GetPath(start, end, currentMoveDir);

            List<Vector3> pathConv = new List<Vector3>();
            foreach (Vector3Int pos in path)
                pathConv.Add(tilemap.GetCellCenterWorld(pos));
            currentPath = pathConv.ToArray();
            currentPosId = 0;
        }

        public void MoveIgnoreCollision(Vector3[] path)
        {
            currentPosId = 0;
            currentPath = path;
        }

        // -------------------------- //
        // Events
        // -------------------------- //

        public void OnInteract(Player player)
        {
            if (isRetreating)
                return;

            if (isFrightened)
            {
                isRetreating = true;
                AIController.SetBool("is_retreating", true);
                GameEvents.EatGhost();
                spr.enabled = false;
                Time.timeScale = 0f;
                scoreUI.Show(true);
                this.Delay(Game.EAT_GHOST_STUTTER_TIME, () =>
                {
                    scoreUI.Show(false);
                    spr.enabled = true;
                    Time.timeScale = 1f;
                });
            }
            else
                player.Die();
        }

        private void OnEndPowerPellet()
        {
            isFrightened = false;
            AIController.SetBool("is_frightened", false);
            AIController.SetBool("is_frightened", false);
        }

        private void OnDeath()
        {
            if (!Game.IsGameOver)
            {
                spr.enabled = false;
                isReady = false;
                this.Delay(Game.DEATH_STUTTER_TIME + Game.ROUND_START_TIME, Respawn);
            }
        }
    }
}
