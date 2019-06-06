using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour
{
    // Component Refs 

    private Animator anim;

    // Gameplay

    [SerializeField] private float speed = 1f;

    // Unity Callbacks

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        Move();
    }

    // Player controls

    private void Move()
    {
        float dt = Time.deltaTime;

        // X and Y values are rounded to avoid smoothing values
        float x = Mathf.Round(Input.GetAxis("Horizontal"));
        float y = Mathf.Round(Input.GetAxis("Vertical"));
        Vector2 dir = Vector2.zero;

        // Movement direction priority like in the old arcade games
        // Right > Left > Down > Up
        if (x > 0f)
            dir = Vector2.right;
        else if (x < 0f)
            dir = Vector2.left;
        else if (y < 0f)
            dir = Vector2.down;
        else if (y > 0f)
            dir = Vector2.up;

        
        var coll = Physics2D.OverlapPoint((Vector2)transform.position + dir);
        if(!coll)
            transform.position += (Vector3)dir * speed * dt;
    }
}
