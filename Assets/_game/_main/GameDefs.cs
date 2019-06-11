using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Wakaman
{
    [CreateAssetMenu(menuName = "Wakaman/Game/GameDefs")]
    public class GameDefs : ScriptableObject
    {
        [System.Serializable]
        public class StageDefs
        {
            public int pelletScore;
            public int powerPelletScore;
            public FruitType fruitType;
            public int fruitScore;
            public int ghostBlueTime;
            public int ghostFlashes;
        }
        [SerializeField] private StageDefs[] stages;

        // Level config file
        [SerializeField] private TextAsset stageDefsFile;

        // -------------------------- //
        // Accessors
        // -------------------------- //

        public int GetPelletScore(int stageIndex)
        {
            var s = GetStageDefs(stageIndex);
            if (s != null)
                return s.pelletScore;
            else
                return 0;
        }

        public int GetPowerPelletScore(int stageIndex)
        {
            var s = GetStageDefs(stageIndex);
            if (s != null)
                return s.powerPelletScore;
            else
                return 0;
        }

        public int GetFruitScore(int stageIndex)
        {
            var s = GetStageDefs(stageIndex);
            if (s != null)
                return s.fruitScore;
            else
                return 0;
        }

        public FruitType GetFruitType(int stageIndex)
        {
            var s = GetStageDefs(stageIndex);
            if (s != null)
                return s.fruitType;
            else
                return FruitType.None;
        }

        public int GetGhostBlueTime(int stageIndex)
        {
            var s = GetStageDefs(stageIndex);
            if (s != null)
                return s.ghostBlueTime;
            else
                return 0;
        }

        // -------------------------- //
        // Helpers
        // -------------------------- //

        private StageDefs GetStageDefs(int stageIndex)
        {
            if (stageIndex >= 0)
            {
                if (stageIndex >= stages.Length)
                    return stages[stages.Length - 1];
                else
                    return stages[stageIndex];
            }
            else
            {
                Debug.LogErrorFormat("Invalid stage index {0}", stageIndex);
                return null;
            }
        }

        // -------------------------- //
        // File loading
        // -------------------------- //

#if UNITY_EDITOR
        private void Awake()
        {
            LoadDefs();
        }

        public void LoadDefs()
        {
            var lines = stageDefsFile.text.Split('\n');
            int n = lines.Length - 1;
            var stagesList = new List<StageDefs>();
            for (int i = 0; i < n; i++)
            {
                string[] entry = lines[i + 1].Split(',');
                var s = new StageDefs();
                s.pelletScore = int.Parse(entry[1]);
                s.powerPelletScore = int.Parse(entry[2]);
                s.fruitType = ParseFruitType(entry[3]);
                s.fruitScore = int.Parse(entry[4]);
                s.ghostBlueTime = int.Parse(entry[5]);
                s.ghostFlashes = int.Parse(entry[6]);
                stagesList.Add(s);
            }
            stages = stagesList.ToArray();
        }

        private FruitType ParseFruitType(string text)
        {
            if (text.Equals("Cherry"))
                return FruitType.Cherry;
            else if (text.Equals("Strawberry"))
                return FruitType.Strawberry;
            else if (text.Equals("Orange"))
                return FruitType.Orange;
            else if (text.Equals("Apple"))
                return FruitType.Apple;
            else if (text.Equals("Melon"))
                return FruitType.Melon;
            else if (text.Equals("Galaxian"))
                return FruitType.Galaxian;
            else if (text.Equals("Bell"))
                return FruitType.Bell;
            else if (text.Equals("Key"))
                return FruitType.Key;
            else
                return FruitType.None;
        }
#endif
    }
}