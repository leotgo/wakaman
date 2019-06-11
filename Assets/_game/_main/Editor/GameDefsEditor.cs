using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Wakaman
{
    [CustomEditor(typeof(GameDefs))]
    public class GameDefsEditor : Editor
    {
        override public void OnInspectorGUI()
        {
            DrawDefaultInspector();
            GameDefs gameDefs = (GameDefs)target;
            if (GUILayout.Button("Load Defs File"))
            {
                gameDefs.LoadDefs();
            }
        }
    }
}
