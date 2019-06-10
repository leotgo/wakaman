using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour, IInteractable
{
    public Transform target;

    public void OnInteract(Player player)
    {
        player.transform.position = target.position;
    }
}
