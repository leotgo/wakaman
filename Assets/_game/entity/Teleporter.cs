using UnityEngine;

namespace Wakaman.Entities
{
    public class Teleporter : MonoBehaviour, IInteractable
    {
        public Transform target;

        public void OnInteract(Player player)
        {
            player.transform.position = target.position;
        }
    }
}