using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Wakaman
{
    public class MapInfo : MonoBehaviour
    {
        public static MapInfo instance = null;

        // Editor-Tweakables
        [SerializeField] private Tilemap collisionMap;
        [SerializeField] private Tilemap collectiblesMap;
        [SerializeField] private Transform upperLeftBound;
        [SerializeField] private Transform lowerRightBound;
        [SerializeField] private Transform cageCenter;
        [SerializeField] private Transform cageExit;

        // Runtime
        private bool mapInfoInitialized;
        private int totalPellets;
        private Dictionary<Vector3Int,TileBase> defaultPellets;

        // Accessors
        public static Tilemap CollisionMap {
            get => instance.collisionMap;
        }
        public static Tilemap CollectiblesMap {
            get => instance.collectiblesMap;
        }
        public static Vector3 CageCenter {
            get => instance.cageCenter.position;
        }
        public static Vector3 CageExit {
            get => instance.cageExit.position;
        }
        public static Vector3Int UpperBound {
            get { return instance.collisionMap.WorldToCell(instance.upperLeftBound.position); }
        }
        public static Vector3Int LowerBound {
            get { return instance.collisionMap.WorldToCell(instance.lowerRightBound.position); }
        }
        public static int TotalPellets {
            get { return instance.totalPellets; }
        }

        // -------------------------- //
        // Monobehaviour
        // -------------------------- //

        private void Start()
        {
            mapInfoInitialized = false;
            instance = this;
            InitializeRuntimeInfo();
        }

        // -------------------------- //
        // Map Management
        // -------------------------- //

        private void InitializeRuntimeInfo()
        {
            if (mapInfoInitialized)
                return;

            int xSize = Mathf.Abs(LowerBound.x - UpperBound.x);
            int ySize = Mathf.Abs(LowerBound.y - UpperBound.y);
            int collectableCount = 0;
            defaultPellets = new Dictionary<Vector3Int, TileBase>();
            for (int i = 0; i <= xSize; i++)
            {
                for (int j = 0; j <= ySize; j++)
                {
                    Vector3Int pos = new Vector3Int(UpperBound.x + i, LowerBound.y + j, UpperBound.z);
                    if (collectiblesMap.HasTile(pos))
                    {
                        collectableCount++;
                        defaultPellets.Add(pos, collectiblesMap.GetTile(pos));
                    }
                }
            }
            totalPellets = collectableCount;
            mapInfoInitialized = true;
        }

        public static void RestoreMapPellets()
        {
            foreach(KeyValuePair<Vector3Int, TileBase> pair in instance.defaultPellets)
                instance.collectiblesMap.SetTile(pair.Key, pair.Value);
        }
    }
}