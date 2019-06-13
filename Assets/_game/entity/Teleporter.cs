using UnityEngine;
using Wakaman.AI;

namespace Wakaman.Entities
{
    public class Teleporter : MonoBehaviour, IInteractable
    {
        public Transform target;

        public void OnInteract(Player player)
        {
            player.transform.position = target.position;
        }

        public void OnInteract(GhostAI ghost)
        {
            Debug.Log("Ghost entered teleport");
            ghost.transform.position = target.position;
        }
    }
}