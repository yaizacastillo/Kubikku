using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxisMovement : MonoBehaviour {

    //Grid
    public LevelGrid grid;
    Cell currentCell, nextCell, initialCell, firstFallingCell;

    //Movement
    bool rotatingGradually;

    public bool allowMovement;

    public float speed;
    public float fallingSpeedMultiplier;
    public bool usePhysics;
    public bool rotateInMoveDirection;
    public bool checkPosition;
    public float timeToRotate;
    [HideInInspector] public bool isMoving, falling;

    Vector3 director;
    [HideInInspector] public bool isFalling = false;
    public bool isClimbing;

    Quaternion initialRotation;

    public int deadDistance;

    public bool movingPlayer; //difference with ghost movement

    public enum Directions { leftUp, leftDown, rightUp, rightDown, up, down };
    Directions lastDirection;

    //Camera
    public CameraMovement camMovement;
    public Camera cam;

    public bool changedWall = false; //tutorial

    //Character
    public MeshFilter myMesh;
    public Mesh[] characterColors;
    public float changeCharacter;
     float characterTimer = 0;
    int actualCharacter = 0;
    public GameObject blood;
    public GameManager gameManager;

    public PlatformMovement platformMovement;

    // Use this for initialization
    void Start() {
        allowMovement = true;
        rotatingGradually = false;

        currentCell = grid.GetCellInWorldPosition(transform.position);
        transform.position = currentCell.globalPosition;

        initialCell = currentCell;
        initialRotation = transform.rotation;

        isMoving = false;
        isClimbing = false;
        isFalling = false;

    }

    // Update is called once per frame
    void Update() {
        if (allowMovement)
        {
            //Check if arribed to the center of the next cell and stop if necessary
            //if not, continue moving
            
            if (isMoving && transform.position == nextCell.globalPosition)
            {
                currentCell = grid.GetCellInWorldPosition(transform.position);

                if (checkPosition)
                {
                    GameManager.gManager.CheckPosition(grid, transform.position, GetBelowPosition());

                    if (currentCell.winPoint && CameraMovement.Lateral == GameManager.gManager.requiredLateral && movingPlayer)
                    {
                        Debug.Log("you win");
                       // GameManager.gManager.PlaySound("win");
                       // Instantiate(particulas, new Vector3 (7.7f,5f,8.5f),Quaternion.identity);
                        //particulas.SetActive(true);
                        // Instantiate(particulas, this.gameObject.transform.position,this.gameObject.transform.rotation);
                       //GameObject pSys= Instantiate(particulas);
                       // pSys.startRotation = gameObject.transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
                        //  GameManager.gManager.Loadscene(GameManager.gManager.nextLevel);
                        StartCoroutine(GameManager.gManager.LoadNextLevel(GameManager.gManager.nextLevel));
                    }
                }

                isMoving = false;

                //climbing is going through a wall
                if (isClimbing)
                {
                    isClimbing = false;
                    MoveTo(lastDirection);
                }

                //When arribed to the cell, check if the player have to fall
                if (usePhysics && !isClimbing && !isMoving)
                {
                    CheckFalling();
                    CheckRampBelow();
                }
            }
            else if (isMoving &!rotatingGradually)
            {
               
               //else
                {
                    if (isFalling)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, nextCell.globalPosition, speed * fallingSpeedMultiplier * Time.deltaTime);
                    }
                    else
                    {
                        transform.position = Vector3.MoveTowards(transform.position, nextCell.globalPosition, speed * Time.deltaTime);
                    }
                }
            }
        }
    }

    #region Checkers
    public void CheckFalling()
    {
        Vector3 belowPos = GetBelowPosition();

        if (grid.GetCellInWorldPosition(belowPos) != null)
        {
            if (grid.GetCellInWorldPosition(belowPos).gameObject == null || grid.GetCellInWorldPosition(belowPos).gameObject.tag == "Vacio")
            {
                //start fall
                if (!isFalling)
                {
                    isFalling = true;
                    firstFallingCell = currentCell;
                }
                MoveTo(Directions.down);
            }
            else
            {
                EndFall();
            }
        }
        else
        {
            EndFall();
        }
    }

    private void EndFall()
    {
        if (isFalling)
        {
            isFalling = false;

            Vector3 diferenceBetweenCells = new Vector3(firstFallingCell.globalPosition.x - currentCell.globalPosition.x, firstFallingCell.globalPosition.y - currentCell.globalPosition.y, firstFallingCell.globalPosition.z - currentCell.globalPosition.z);

            if (diferenceBetweenCells.sqrMagnitude >= deadDistance)
            {
                GameManager.gManager.PlaySound("death");
                GameManager.gManager.GenerateBlood(grid, transform.position);
                
                ResetPosition();
                ResetRotation();
                camMovement.ResetGravity();
                newPlayer();
                if (platformMovement!=null)
                {
                    platformMovement.Respawn();
                }
            }

            firstFallingCell = null;
        }
    }
    
    public void CheckRampBelow()
    {


        Vector3 belowPos = GetBelowPosition(); 

        if (grid.GetCellInWorldPosition(belowPos) != null)
        {
            //Check if the object is a ramp
            if (grid.GetCellInWorldPosition(belowPos).gameObject != null && grid.GetCellInWorldPosition(belowPos).gameObject.tag == "RAMP")
            {

               // Debug.Log("ramp below");
   
                Transform ramp = grid.GetCellInWorldPosition(belowPos).gameObject.transform;

                Vector3 horizonalDirector = Vector3.zero;
                Vector3 relativeUp = Vector3.up;
                Vector3 newDirector = Vector3.zero;
                bool orientedCorrectly = true;

                switch (CameraMovement.Lateral)
                {
                    case CameraMovement.Axes.xPos:
                        if (-ramp.forward == Vector3.right) horizonalDirector = ramp.up;
                        else if (ramp.up == Vector3.right) horizonalDirector = -ramp.forward;
                        else orientedCorrectly = false;
                        relativeUp = Vector3.right;
                        newDirector = horizonalDirector - Vector3.right;
                        break;

                    case CameraMovement.Axes.xNeg:
                        if (-ramp.forward == Vector3.left) horizonalDirector = ramp.up;
                        else if (ramp.up == Vector3.left) horizonalDirector = -ramp.forward;
                        else orientedCorrectly = false;
                        relativeUp = Vector3.left;
                        newDirector = horizonalDirector - Vector3.left;
                        break;

                    case CameraMovement.Axes.yPos:
                        if (-ramp.forward == Vector3.up) horizonalDirector = ramp.up;
                        else if (ramp.up == Vector3.up) horizonalDirector = -ramp.forward;
                        else orientedCorrectly = false;
                        relativeUp = Vector3.up;
                        newDirector = horizonalDirector - Vector3.up;
                        break;

                    case CameraMovement.Axes.yNeg:
                        if (-ramp.forward == Vector3.down) horizonalDirector = ramp.up;
                        else if (ramp.up == Vector3.down) horizonalDirector = -ramp.forward;
                        else orientedCorrectly = false;
                        relativeUp = Vector3.down;
                        newDirector = horizonalDirector - Vector3.down;
                        break;

                    case CameraMovement.Axes.zPos:
                        if (-ramp.forward == Vector3.forward) horizonalDirector = ramp.up;
                        else if (ramp.up == Vector3.forward) horizonalDirector = -ramp.forward;
                        else orientedCorrectly = false;
                        relativeUp = Vector3.forward;
                        newDirector = horizonalDirector - Vector3.forward;
                        break;

                    case CameraMovement.Axes.zNeg:
                        if (-ramp.forward == Vector3.back) horizonalDirector = ramp.up;
                        else if (ramp.up == Vector3.back) horizonalDirector = -ramp.forward;
                        else orientedCorrectly = false;
                        relativeUp = Vector3.back;
                        newDirector = horizonalDirector - Vector3.back;
                        break;
                }

                if (orientedCorrectly)
                {
                   // Debug.Log("oriented correctly");
                    transform.rotation = Quaternion.LookRotation(horizonalDirector, relativeUp);

                    //Set the player position in the exact center of his current cell (just in case) and Get the target cell
                    transform.position = grid.GetCellInWorldPosition(transform.position).globalPosition;
                   // Debug.Log(transform.position + newDirector);
                    nextCell = grid.GetCellInWorldPosition(transform.position + newDirector);

                    if (nextCell != null && nextCell.transitable) {
                        isMoving = true;
                       // Debug.Log("moving to the next cell");
                    }

                }
            }
        }
    }

    #endregion

    public void MoveTo(Directions newDirection)
    {
        //Select the target cell to go
        if (!isMoving)
        {
            //Get the new vector director
            director = GetVectorDirector(newDirection);

            //Rotate to the new direction if allowed
            if(rotateInMoveDirection && !isFalling && !rotatingGradually)
            {
                //if (changedWall)
                //{
                    //changedWall = false;
                    //StartCoroutine(RotateGradually(director));
                //}
                //else
                //{
                    RotateGameobject(director);
                //}
                
            }

            //Set the player position in the exact center of his current cell (just in case) and Get the target cell
            transform.position = grid.GetCellInWorldPosition(transform.position).globalPosition;
            nextCell = grid.GetCellInWorldPosition(transform.position + director);

            //In case it is a ghost piece and it collisions with a wall, do not move or change gravity
            //Otherwise, if the cell exist, move whithout check if the cell is transitable
            if (!usePhysics) {
                if (nextCell == null)
                {
                    return;
                }
                else
                {
                    isMoving = true;
                    return;
                }
            }

            //If collisions with a wall, rotate
            if (nextCell == null)
            {

                Vector2 actualScreenPoint = cam.WorldToScreenPoint(transform.position);
                Vector2 futureScreenPoint = cam.WorldToScreenPoint(transform.position + director);

                if (futureScreenPoint.x > actualScreenPoint.x && futureScreenPoint.y > actualScreenPoint.y)
                {
                    camMovement.RotateTo(CameraMovement.Rotations.dRightUp);
                }
                else if (futureScreenPoint.x < actualScreenPoint.x && futureScreenPoint.y < actualScreenPoint.y)
                {
                    camMovement.RotateTo(CameraMovement.Rotations.dRightDown);
                }
                else if (futureScreenPoint.x < actualScreenPoint.x && futureScreenPoint.y > actualScreenPoint.y)
                {
                    camMovement.RotateTo(CameraMovement.Rotations.dLeftUp);
                }
                else if (futureScreenPoint.x > actualScreenPoint.x && futureScreenPoint.y < actualScreenPoint.y)
                {
                    camMovement.RotateTo(CameraMovement.Rotations.dLeftDown);
                }
                changedWall = true;
                ResetRotation();
            }
            else
            {
                //If the cell is transitable, allow movement
                if (nextCell.transitable)
                {
                    //If the cell under the empty space is a ramp
                    isMoving = true;
                }
                //If the next cell is a ramp, change the nextCell
                else if (nextCell.gameObject != null && nextCell.gameObject.tag == "RAMP")
                {          
                    Vector3 rampForward = nextCell.gameObject.transform.forward;
                    Vector3 rampUp = nextCell.gameObject.transform.up;

                    Vector3 alternativeRampXForward = nextCell.gameObject.transform.right;
                    Vector3 alternativeRampXUp = nextCell.gameObject.transform.right;

                    Vector3 playerDirector = Vector3.Normalize(nextCell.globalPosition - currentCell.globalPosition);

                    switch (CameraMovement.Lateral)
                    {
                        case CameraMovement.Axes.xPos:
                            alternativeRampXForward = Quaternion.Euler(-90, 0, 0) * alternativeRampXForward;
                            alternativeRampXUp = Quaternion.Euler(-90, 0, 0) * alternativeRampXUp;
                            break;
                        case CameraMovement.Axes.xNeg:
                            alternativeRampXForward = Quaternion.Euler(90, 0, 0) * alternativeRampXForward;
                            alternativeRampXUp = Quaternion.Euler(90, 0, 0) * alternativeRampXUp;
                            break;
                        case CameraMovement.Axes.yPos:
                            alternativeRampXForward = Quaternion.Euler(0, -90, 0) * alternativeRampXForward;
                            alternativeRampXUp = Quaternion.Euler(0, -90, 0) * alternativeRampXUp;
                            break;
                        case CameraMovement.Axes.yNeg:
                            alternativeRampXForward = Quaternion.Euler(0, 90, 0) * alternativeRampXForward;
                            alternativeRampXUp = Quaternion.Euler(0, 90, 0) * alternativeRampXUp;
                            break;
                        case CameraMovement.Axes.zPos:
                            alternativeRampXForward = Quaternion.Euler(0, 0, -90) * alternativeRampXForward;
                            alternativeRampXUp = Quaternion.Euler(0, 0, -90) * alternativeRampXUp;
                            break;
                        case CameraMovement.Axes.zNeg:
                            alternativeRampXForward = Quaternion.Euler(0, 0, 90) * alternativeRampXForward;
                            alternativeRampXUp = Quaternion.Euler(0, 0, 90) * alternativeRampXUp;
                            break;
                    }

                    if (playerDirector == rampForward && playerDirector == alternativeRampXForward ||
                        playerDirector == -rampUp && playerDirector == -alternativeRampXUp)
                    {
                        isMoving = true;
                        isClimbing = true;
                        lastDirection = newDirection;

                        //Get the new vector director
                        director = GetRampVectorDirector(newDirection);

                        //Set the player position in the exact center of his current cell (just in case) and Get the target cell
                        transform.position = grid.GetCellInWorldPosition(transform.position).globalPosition;
                        nextCell = grid.GetCellInWorldPosition(transform.position + director);
                    }
                }               
            }
        }
    }


    private Vector3 GetBelowPosition()
    {
        Vector3 belowPos = Vector3.zero;

        switch (CameraMovement.Lateral)
        {
            case CameraMovement.Axes.xNeg: belowPos = new Vector3(1, 0, 0); break;
            case CameraMovement.Axes.xPos: belowPos = new Vector3(-1, 0, 0); break;
            case CameraMovement.Axes.yNeg: belowPos = new Vector3(0, 1, 0); break;
            case CameraMovement.Axes.yPos: belowPos = new Vector3(0, -1, 0); break;
            case CameraMovement.Axes.zNeg: belowPos = new Vector3(0, 0, 1); break;
            case CameraMovement.Axes.zPos: belowPos = new Vector3(0, 0, -1); break;
        }

        return transform.position + belowPos;
    }

    public void ResetPosition ()
    {
        transform.position = initialCell.globalPosition;
        transform.rotation = initialRotation;
        isMoving = false;
    }

    public void AllowDenyMovement(bool allowDeny)
    {
        allowMovement = allowDeny;
    }

    public void ResetRotation()
    {
        myMesh.transform.Rotate(-PlayerAnimation.totalRotated, 0, 0);
        PlayerAnimation.totalRotated = 0;
    }

    public void newPlayer()
    {
        int newSkin = Random.Range(0, 5);
        myMesh.mesh = characterColors[newSkin];
    }

    #region Mathematical

    /// <summary>
    /// Return a vector with de orientation to move in the diagonal requiered indepent of the orientation of the level
    /// </summary>
    /// <param name="newDirection"></param>
    /// <returns></returns>
    Vector3 GetVectorDirector(Directions newDirection) {

        CameraMovement.Axes lateral = CameraMovement.Lateral;
        CameraMovement.Axes dLeft = CameraMovement.DiagonalLeft;
        CameraMovement.Axes dRight = CameraMovement.DiagonalRight;

        Vector3 newDirector = new Vector3(0,0,0);

        switch(newDirection) {

            case Directions.leftUp:
                if (dRight == CameraMovement.Axes.xPos) newDirector = new Vector3(1,0,0);
                else if (dRight == CameraMovement.Axes.yPos) newDirector = new Vector3(0,1,0);
                else if (dRight == CameraMovement.Axes.zPos) newDirector = new Vector3(0,0,1);
                else if (dRight == CameraMovement.Axes.xNeg) newDirector = new Vector3(-1,0,0);
                else if (dRight == CameraMovement.Axes.yNeg) newDirector = new Vector3(0,-1,0);
                else if (dRight == CameraMovement.Axes.zNeg) newDirector = new Vector3(0,0,-1);
                break;

            case Directions.leftDown:
                if (dRight == CameraMovement.Axes.xPos) newDirector = new Vector3(-1, 0, 0);
                else if (dRight == CameraMovement.Axes.yPos) newDirector = new Vector3(0, -1, 0);
                else if (dRight == CameraMovement.Axes.zPos) newDirector = new Vector3(0, 0, -1);
                else if (dRight == CameraMovement.Axes.xNeg) newDirector = new Vector3(1, 0, 0);
                else if (dRight == CameraMovement.Axes.yNeg) newDirector = new Vector3(0, 1, 0);
                else if (dRight == CameraMovement.Axes.zNeg) newDirector = new Vector3(0, 0, 1);
                break;

            case Directions.rightUp:
                if (dLeft == CameraMovement.Axes.xPos) newDirector = new Vector3(1, 0, 0);
                else if (dLeft == CameraMovement.Axes.yPos) newDirector = new Vector3(0, 1, 0);
                else if (dLeft == CameraMovement.Axes.zPos) newDirector = new Vector3(0, 0, 1);
                else if (dLeft == CameraMovement.Axes.xNeg) newDirector = new Vector3(-1, 0, 0);
                else if (dLeft == CameraMovement.Axes.yNeg) newDirector = new Vector3(0, -1, 0);
                else if (dLeft == CameraMovement.Axes.zNeg) newDirector = new Vector3(0, 0, -1);
                break;

            case Directions.rightDown:
                if (dLeft == CameraMovement.Axes.xPos) newDirector = new Vector3(-1, 0, 0);
                else if (dLeft == CameraMovement.Axes.yPos) newDirector = new Vector3(0, -1, 0);
                else if (dLeft == CameraMovement.Axes.zPos) newDirector = new Vector3(0, 0, -1);
                else if (dLeft == CameraMovement.Axes.xNeg) newDirector = new Vector3(1, 0, 0);
                else if (dLeft == CameraMovement.Axes.yNeg) newDirector = new Vector3(0, 1, 0);
                else if (dLeft == CameraMovement.Axes.zNeg) newDirector = new Vector3(0, 0, 1);
                break;

            case Directions.up:
                if (lateral == CameraMovement.Axes.xPos) newDirector = new Vector3(1, 0, 0);
                else if (lateral == CameraMovement.Axes.yPos) newDirector = new Vector3(0, 1, 0);
                else if (lateral == CameraMovement.Axes.zPos) newDirector = new Vector3(0, 0, 1);
                else if (lateral == CameraMovement.Axes.xNeg) newDirector = new Vector3(-1, 0, 0);
                else if (lateral == CameraMovement.Axes.yNeg) newDirector = new Vector3(0, -1, 0);
                else if (lateral == CameraMovement.Axes.zNeg) newDirector = new Vector3(0, 0, -1);
                break;

            case Directions.down:
                if (lateral == CameraMovement.Axes.xPos) newDirector = new Vector3(-1, 0, 0);
                else if (lateral == CameraMovement.Axes.yPos) newDirector = new Vector3(0, -1, 0);
                else if (lateral == CameraMovement.Axes.zPos) newDirector = new Vector3(0, 0, -1);
                else if (lateral == CameraMovement.Axes.xNeg) newDirector = new Vector3(1, 0, 0);
                else if (lateral == CameraMovement.Axes.yNeg) newDirector = new Vector3(0, 1, 0);
                else if (lateral == CameraMovement.Axes.zNeg) newDirector = new Vector3(0, 0, 1);
                break;
        }

        

        return newDirector;
    }

    /// <summary>
    /// Return a vector with de orientation to move in the diagonal requiered indepent of the orientation of the level
    /// </summary>
    /// <param name="newDirection"></param>
    /// <returns></returns>
    Vector3 GetRampVectorDirector(Directions newDirection)
    {

        CameraMovement.Axes lateral = CameraMovement.Lateral;
        CameraMovement.Axes dLeft = CameraMovement.DiagonalLeft;
        CameraMovement.Axes dRight = CameraMovement.DiagonalRight;

        Vector3 newDirector = new Vector3(0, 0, 0);
        int xAdded = 0;
        int yAdded = 0;
        int zAdded = 0;


        switch (newDirection)
        {

            case Directions.leftUp:
                if (dRight == CameraMovement.Axes.xPos) newDirector = new Vector3(1, 0, 0);
                else if (dRight == CameraMovement.Axes.yPos) newDirector = new Vector3(0, 1, 0);
                else if (dRight == CameraMovement.Axes.zPos) newDirector = new Vector3(0, 0, 1);
                else if (dRight == CameraMovement.Axes.xNeg) newDirector = new Vector3(-1, 0, 0);
                else if (dRight == CameraMovement.Axes.yNeg) newDirector = new Vector3(0, -1, 0);
                else if (dRight == CameraMovement.Axes.zNeg) newDirector = new Vector3(0, 0, -1);
                break;

            case Directions.leftDown:
                if (dRight == CameraMovement.Axes.xPos) newDirector = new Vector3(-1, 0, 0);
                else if (dRight == CameraMovement.Axes.yPos) newDirector = new Vector3(0, -1, 0);
                else if (dRight == CameraMovement.Axes.zPos) newDirector = new Vector3(0, 0, -1);
                else if (dRight == CameraMovement.Axes.xNeg) newDirector = new Vector3(1, 0, 0);
                else if (dRight == CameraMovement.Axes.yNeg) newDirector = new Vector3(0, 1, 0);
                else if (dRight == CameraMovement.Axes.zNeg) newDirector = new Vector3(0, 0, 1);
                break;

            case Directions.rightUp:
                if (dLeft == CameraMovement.Axes.xPos) newDirector = new Vector3(1, 0, 0);
                else if (dLeft == CameraMovement.Axes.yPos) newDirector = new Vector3(0, 1, 0);
                else if (dLeft == CameraMovement.Axes.zPos) newDirector = new Vector3(0, 0, 1);
                else if (dLeft == CameraMovement.Axes.xNeg) newDirector = new Vector3(-1, 0, 0);
                else if (dLeft == CameraMovement.Axes.yNeg) newDirector = new Vector3(0, -1, 0);
                else if (dLeft == CameraMovement.Axes.zNeg) newDirector = new Vector3(0, 0, -1);
                break;

            case Directions.rightDown:
                if (dLeft == CameraMovement.Axes.xPos) newDirector = new Vector3(-1, 0, 0);
                else if (dLeft == CameraMovement.Axes.yPos) newDirector = new Vector3(0, -1, 0);
                else if (dLeft == CameraMovement.Axes.zPos) newDirector = new Vector3(0, 0, -1);
                else if (dLeft == CameraMovement.Axes.xNeg) newDirector = new Vector3(1, 0, 0);
                else if (dLeft == CameraMovement.Axes.yNeg) newDirector = new Vector3(0, 1, 0);
                else if (dLeft == CameraMovement.Axes.zNeg) newDirector = new Vector3(0, 0, 1);
                break;
        }

        switch (lateral) {
            case CameraMovement.Axes.xPos: xAdded = 1; break;
            case CameraMovement.Axes.xNeg: xAdded = -1; break;
            case CameraMovement.Axes.yPos: yAdded = 1; break;
            case CameraMovement.Axes.yNeg: yAdded = -1; break;
            case CameraMovement.Axes.zPos: zAdded = 1; break;
            case CameraMovement.Axes.zNeg: zAdded = -1; break;
        }

        newDirector = new Vector3(newDirector.x + xAdded, newDirector.y + yAdded, newDirector.z + zAdded);
        return newDirector;
    }

    void RotateGameobject(Vector3 direction)
    {
        Vector3 upwards = Vector3.up;

        switch(CameraMovement.Lateral)
        {
            case CameraMovement.Axes.xPos: upwards = Vector3.right; break;
            case CameraMovement.Axes.xNeg: upwards = Vector3.left; break;
            case CameraMovement.Axes.yPos: upwards = Vector3.up; break;
            case CameraMovement.Axes.yNeg: upwards = Vector3.down; break;
            case CameraMovement.Axes.zPos: upwards = Vector3.forward; break;
            case CameraMovement.Axes.zNeg: upwards = Vector3.back; break;
        }

        this.transform.rotation = Quaternion.LookRotation(direction, upwards);
    }

    IEnumerator RotateGradually(Vector3 direction)
    {
        Quaternion finalRotation;
        Quaternion initialRotation = transform.rotation;
        Vector3 angleToRotate = new Vector3(0, 0, 0);
        float passedRotation = 0;
        Vector3 upwards = Vector3.up;

        switch (CameraMovement.Lateral)
        {
            case CameraMovement.Axes.xPos: upwards = Vector3.right; break;
            case CameraMovement.Axes.xNeg: upwards = Vector3.left; break;
            case CameraMovement.Axes.yPos: upwards = Vector3.up; break;
            case CameraMovement.Axes.yNeg: upwards = Vector3.down; break;
            case CameraMovement.Axes.zPos: upwards = Vector3.forward; break;
            case CameraMovement.Axes.zNeg: upwards = Vector3.back; break;
        }

        finalRotation = Quaternion.LookRotation(direction, upwards);


        rotatingGradually = true;

        finalRotation = Quaternion.Euler(transform.rotation.eulerAngles + angleToRotate);

        while (transform.rotation.eulerAngles != finalRotation.eulerAngles)
        {
            passedRotation += Time.deltaTime / timeToRotate;
            transform.rotation = Quaternion.Lerp(initialRotation, finalRotation, passedRotation);
            yield return null;
        }
        rotatingGradually = false;
        yield return null;
    }

    #endregion
}
