using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wakaman.AI
{
    /*
     * 
     * GhostsManager: This class is just used as an access
     *                point to references to the ghosts. It
     *                is required by Inky (the cyan ghost)
     *                to calculate his target position when
     *                chasing the player.
     * 
     */
    public class GhostsManager : MonoBehaviour
    {
        // Singleton
        public static GhostsManager instance;

        // Ref Editor-tweakables
        [SerializeField] private GhostAI redGhost;
        [SerializeField] private GhostAI pinkGhost;
        [SerializeField] private GhostAI cyanGhost;
        [SerializeField] private GhostAI orangeGhost;

        // Accessors
        public GhostAI RedGhost { get => redGhost; }
        public GhostAI PinkGhost { get => pinkGhost; }
        public GhostAI CyanGhost { get => cyanGhost; }
        public GhostAI OrangeGhost { get => orangeGhost; }

        private void Start()
        {
            instance = this;
        }
    }
}
