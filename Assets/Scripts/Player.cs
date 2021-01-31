using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Moves
{
    public Vector3 fromPos;
    public Vector3 toPos;
    public Box whichBox;
    public Vector3 boxPos;
    public bool withBox;
}

public struct BlockedBy
{
    public bool isBlocked;
    public bool isBlockedByBox;
}

public class Player : MonoBehaviour
{
    public int num_moves;
    public Stack<Moves> moves = new Stack<Moves>();
    public Moves move;

    public bool Move(Vector2 direction)
    {
        if(Mathf.Abs(direction.x) < 0.5)//block diagonal move
        {
            direction.x = 0;
        }
        else
        {
            direction.y = 0;
        }
        direction.Normalize(); // Move only 1 unit
        if(IsBlocked(transform.position, direction).isBlockedByBox)
        {
            if(IsBlocked(transform.position, direction).isBlocked)
            {
                Debug.Log("Is Blocked");
                return false;
            }
            else
            {
                Debug.Log("Is Moving but with box");
                move.fromPos = transform.position;
                move.toPos = new Vector3(direction.x, direction.y, 0);
                move.withBox = true;
                moves.Push(move);
                Debug.Log("Moved from: " + move.fromPos.ToString() + "Towards: " + move.toPos.ToString() + "With Box");

                transform.Translate(direction);
                num_moves++;
            }
        }
        else
        {
            if (IsBlocked(transform.position, direction).isBlocked)
            {
                Debug.Log("Is Blocked");
                return false;
            }
            else
            {
                Debug.Log("Is Moving but NO box");
                move.fromPos = transform.position;
                move.toPos = new Vector3(direction.x, direction.y, 0);
                move.withBox = false;
                moves.Push(move);
                Debug.Log("Moved from: " + move.fromPos.ToString() + "Towards: " + move.toPos.ToString() + "Without Box");

                transform.Translate(direction);
                num_moves++;
            }
        }
        return true;
    }

    BlockedBy IsBlocked(Vector3 position, Vector2 direction)
    {
        Vector2 newPos = new Vector2(position.x, position.y) + direction;
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
        BlockedBy blocked;

        foreach(var wall in walls)
        {
            if(wall.transform.position.x == newPos.x && wall.transform.position.y == newPos.y)
            {
                blocked.isBlocked = true;
                blocked.isBlockedByBox = false;
                return blocked;
            }
        }

        GameObject[] boxes = GameObject.FindGameObjectsWithTag("Box");

        foreach (var box in boxes)
        {
            if (box.transform.position.x == newPos.x && box.transform.position.y == newPos.y)
            {
                Box theBox = box.GetComponent<Box>();
                if(theBox)
                {
                    move.whichBox = theBox;
                    move.boxPos = theBox.transform.position;
                }
                if (theBox && theBox.Move(direction))
                {
                    blocked.isBlocked = false;
                    blocked.isBlockedByBox = true;
                    
                    return blocked;
                }
                else
                {
                    blocked.isBlocked = true;
                    blocked.isBlockedByBox = true;
                    return blocked;
                }
            }
        }
        blocked.isBlocked = false;
        blocked.isBlockedByBox = false;
        return blocked;
    }
}
