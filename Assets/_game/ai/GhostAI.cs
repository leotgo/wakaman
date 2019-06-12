using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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
    public class GhostAI : MonoBehaviour
    {
        // Definitions
        public enum LeaveCageCondition
        {
            None,
            FixedTimer,
            ConsumeDots
        }

        // Editor-Tweakables
        [Header("Gameplay")]
        [SerializeField] private float speed = 0.6f;
        [SerializeField] private float defaultChaseTime = 18f;
        [SerializeField] private float defaultScatterTime = 8f;

        [Header("Behaviour in Cage")]
        [SerializeField] private bool startInCage = true;
        [SerializeField] private bool startMovingDownInCage = false;
        [SerializeField] private LeaveCageCondition leaveCageCondition = LeaveCageCondition.None;
        [SerializeField] private float leaveCageTime = 5.0f;
        [SerializeField] private int leaveCageDotsNumber = 30;

        [Header("AI System Refs")]
        [SerializeField] private Animator sprAnim;
        [SerializeField] private Animator AIController;
        [SerializeField] private Transform[] cornerPos;
        [SerializeField] private Transform cagePos;

        // Runtime
        private Tilemap tilemap;
        private int currentPosId;
        private Vector3[] currentPath;

        // Accessors
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
                switch(leaveCageCondition)
                {
                    case LeaveCageCondition.None:
                        return true;
                    case LeaveCageCondition.FixedTimer:
                        return Game.GetElapsedTime() > leaveCageTime;
                    case LeaveCageCondition.ConsumeDots:
                        return Game.GetConsumedPellets() > leaveCageDotsNumber;
                    default:
                        return true;
                }
            }
        }

        // -------------------------- //
        // Monobehaviour
        // -------------------------- //

        private void Start()
        {
            tilemap = Game.GetTilemap();
            currentPath = new Vector3[0];
            currentPosId = 0;
        }

        private void Update()
        {
            if (currentPath.Length > 0)
            {
                float dt = Time.deltaTime;
                float step = speed * dt;
                Vector3 target = currentPath[currentPosId];
                Vector2 dir = (target - transform.position).normalized;
                sprAnim.SetFloat("dir_x", dir.x);
                sprAnim.SetFloat("dir_y", dir.y);
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
            }
        }

        // -------------------------- //
        // Actions
        // -------------------------- //

        public void MoveTo(Vector3 target)
        {
            Vector3Int start = tilemap.WorldToCell(transform.position);
            Vector3Int end = tilemap.WorldToCell(target);
            Vector3Int[] path = Pathfinding.instance.GetPath(start, end);

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
            Vector3Int[] path = Pathfinding.instance.GetPath(start, end);

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
    }
}
