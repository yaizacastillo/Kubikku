using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMovement : MonoBehaviour
{

    public GameObject player;
    public LevelGrid grid;
    [HideInInspector] public Cell currentCell, nextCell, initialCell;
    public float speed = 2;
    int cellsMoved = 0;
    public int cells;
    public string direction;
    Vector3 movingDirection;
    public static bool moving = true;
    float waitedTime = 0;
    float totalTime = 2;
    bool done = false;

    void Start()
    {
        currentCell = grid.GetCellInWorldPosition(transform.position);
        transform.position = currentCell.globalPosition;
        nextCell = currentCell;
        movingDirection = getInitialDirection(direction);
        
        initialCell = currentCell;
        Respawn();
    }

    void Update()
    { 
        //if the player steps on the platform, move it
        if (isPlayer())
            Move(cells);
    }

    //is the player on the platform
    public bool isPlayer()
    {
        //round transform.up because if transform.up is not a round number, it gets another cell
        Vector3 fixedUp = new Vector3(Mathf.RoundToInt(transform.up.x), Mathf.RoundToInt(transform.up.y), Mathf.RoundToInt(transform.up.z));

        //print(grid.GetCellInWorldPosition(transform.position).globalPosition + "   " + fixedUp + "   " + grid.GetCellInWorldPosition(transform.position + fixedUp).globalPosition + "   " + grid.GetCellInWorldPosition(player.transform.position).globalPosition);

        if (grid.GetCellInWorldPosition(transform.position+fixedUp) == grid.GetCellInWorldPosition(player.transform.position))
            return true;

        else
            return false;
    }

    //input direction to forward vector of the platform
    Vector3 getInitialDirection(string direction)
    {
        switch (direction)
        {
            case "up": return Vector3.up;
            case "down": return Vector3.down;
            case "forward": return Vector3.forward;
            case "back": return Vector3.back;
            case "left": return Vector3.left;
            case "right": return Vector3.right;
            default: return Vector3.zero;
        }
    }

    void Move(int cells)
    {
        if (moving)
        {
            if (cellsMoved <= cells)
            {
                //move platform
                transform.position = Vector3.MoveTowards(transform.position, nextCell.globalPosition, speed * Time.deltaTime);

                //move player
                player.transform.position = transform.position + transform.up;

                //if it's in the next cell, this cell is non-transitable and the previous one is transitable
                if (transform.position == nextCell.globalPosition)
                {
                    cellsMoved++;
                    currentCell.transitable = true;
                    currentCell.gameObject = null;
                    currentCell = grid.GetCellInWorldPosition(transform.position);
                    currentCell.transitable = false;
                    currentCell.gameObject = gameObject;
                    nextCell = grid.GetCellInWorldPosition(transform.position + movingDirection);
                }
            }

            //when the platform moves x cells, changes its direction
            else
            {
                moving = false;
            }
        }

        else
        {
            waitedTime += Time.deltaTime;

            if (waitedTime >= totalTime)
            {
                moving = true;
                waitedTime = 0;
                movingDirection *= -1;
                cellsMoved = 0;
                nextCell = currentCell;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.up);
    }

    public void Respawn()
    {
        cellsMoved = 0;
        waitedTime = 0;
        currentCell = grid.GetCellInWorldPosition(initialCell.globalPosition);
        transform.position = currentCell.globalPosition;
        nextCell = currentCell;
        movingDirection = getInitialDirection(direction);
        moving = true;
    }
}
