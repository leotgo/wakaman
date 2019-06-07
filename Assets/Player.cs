using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour
{
    // Editor Tweakables

    [SerializeField] private float speed = 1f;

    // Component Refs 

    private Animator anim;

    // Runtime vars

    private bool isMoving;
    private Vector3Int lastMoveDir;

    // Unity Callbacks

    private void Start()
    {
        anim = GetComponent<Animator>();
        isMoving = false;
    }

    private void Update()
    {
        if(!isMoving)
            Move();
    }

    // Movement

    private void Move()
    {
        float dt = Time.deltaTime;
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        // Movement direction priority like in the old arcade games.
        // Right > Left > Down > Up
        Vector3Int dir = Vector3Int.zero;
        if (x > 0f)
            dir = Vector3Int.right;
        else if (x < 0f)
            dir = Vector3Int.left;
        else if (y < 0f)
            dir = Vector3Int.down;
        else if (y > 0f)
            dir = Vector3Int.up;

        if (dir.magnitude > 0f)
        {
            bool isHorizontal = Mathf.Abs(dir.x) > 0f;
            // Direction of adjacent tiles.
            var adj1 = isHorizontal ? Vector3Int.up : Vector3Int.left;
            var adj2 = isHorizontal ? Vector3Int.down : Vector3Int.right;
            // Checks if adjacent tiles are not opposed to the current player 
            // movement direction.
            bool adj1IsInverseDir = adj1 == (lastMoveDir * -1);
            bool adj2IsInverseDir = adj2 == (lastMoveDir * -1);

            // Checks if next tile is a wall.
            if (!CheckMoveCollision(dir))
            {
                anim.SetFloat("dir_x", dir.x);
                anim.SetFloat("dir_y", dir.y);
                StartCoroutine(MoveTile(dir));
            }
            // Checks if adjacent tiles are walls and fixes player movement 
            // to assist the player in turning corners (only if it does not
            // make the player go back).
            else if(!adj1IsInverseDir && !CheckMoveCollision(dir + adj1))
            {
                anim.SetFloat("dir_x", adj1.x);
                anim.SetFloat("dir_y", adj1.y);
                StartCoroutine(MoveTile(adj1));
            }
            else if(!adj2IsInverseDir && !CheckMoveCollision(dir + adj2))
            {
                anim.SetFloat("dir_x", adj2.x);
                anim.SetFloat("dir_y", adj2.y);
                StartCoroutine(MoveTile(adj2));
            }
        }
    }

    // Smoothly move the player from his current to the next tile.
    IEnumerator MoveTile(Vector3Int dir)
    {
        isMoving = true;
        float dt = Time.fixedDeltaTime;
        Vector3 targetPos = GetCellPosByOffset(dir);
        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * dt);
            yield return new WaitForFixedUpdate();
        }
        transform.position = targetPos;
        lastMoveDir = dir;
        isMoving = false;
    }

    // Returns true if the player will collide with a wall in a given movement direction.
    private bool CheckMoveCollision(Vector3Int dir)
    {
        Vector3 nextCellPos = GetCellPosByOffset(dir);
        var coll = Physics2D.OverlapPoint(nextCellPos);
        Debug.DrawRay(transform.position, nextCellPos - transform.position);
        return coll != null;
    }

    private Vector3 GetCellPosByOffset(Vector3Int offset)
    {
        Tilemap tilemap = Game.GetTilemap();
        Vector3Int currCell = tilemap.WorldToCell(transform.position);
        Vector3 pos = tilemap.GetCellCenterWorld(currCell + offset);
        return pos;
    }
}
