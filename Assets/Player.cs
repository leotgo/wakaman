using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour
{
    // Editor Tweakables

    [SerializeField] [Range(0.1f, 2f)] private float speed = 1f;
    [SerializeField] private AudioClip[] sfxMunch;

    // Component Refs 

    private Animator anim;
    private AudioSource audioSrc;

    // Runtime vars

    private bool isMoving;
    private Vector3Int lastMoveDir;

    // Unity Callbacks

    private void Start()
    {
        anim = GetComponent<Animator>();
        audioSrc = GetComponent<AudioSource>();
        isMoving = false;
    }

    private void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        Vector3Int dir = GetMovementDir(x, y);
        if (dir.magnitude > 0f)
        {
            if (!isMoving)
                Move(dir);
        }
    }

    // Movement

    // Returns a discrete movement direction based on Horizontal and Vertical axes.
    private Vector3Int GetMovementDir(float x, float y)
    {
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

        return dir;
    }

    // Checks for collision and performs intended player movement when possible.
    private void Move(Vector3Int dir)
    {
        bool isHorizontal = Mathf.Abs(dir.x) > 0f;
        // Adjacent tiles are checked to see if the player is close to a
        // corner to facilitate turning.
        var adjDir1 = isHorizontal ? Vector3Int.up   : Vector3Int.left;
        var adjDir2 = isHorizontal ? Vector3Int.down : Vector3Int.right;
        // Checks if adjacent tiles are not opposed to the current player 
        // movement direction.
        bool adj1IsInverseDir = adjDir1 == (lastMoveDir * -1);
        bool adj2IsInverseDir = adjDir2 == (lastMoveDir * -1);

        // Checks if next tile is a wall.
        if (!CheckMoveCollision(dir))
        {
            anim.SetFloat("dir_x", dir.x);
            anim.SetFloat("dir_y", dir.y);
            StartCoroutine(MoveTileRoutine(dir));
        }
        // Checks if adjacent tiles are walls and assists player movement
        // when turning corners (less precision is required!)
        else if (!adj1IsInverseDir && !CheckMoveCollision(dir + adjDir1))
        {
            anim.SetFloat("dir_x", adjDir1.x);
            anim.SetFloat("dir_y", adjDir1.y);
            StartCoroutine(MoveTileRoutine(adjDir1));
        }
        else if (!adj2IsInverseDir && !CheckMoveCollision(dir + adjDir2))
        {
            anim.SetFloat("dir_x", adjDir2.x);
            anim.SetFloat("dir_y", adjDir2.y);
            StartCoroutine(MoveTileRoutine(adjDir2));
        }
    }

    // Smoothly move the player from his current to the next tile.
    IEnumerator MoveTileRoutine(Vector3Int dir)
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
