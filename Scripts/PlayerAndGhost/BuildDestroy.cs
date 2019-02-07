using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildDestroy : MonoBehaviour {

    public Transform ghostTransform;
    public Transform playerTransform;
    public LevelGrid grid;
    public GameObject cubePrefab;
    public GameObject rampPrefab;
    public GameObject down, left, right, top, front, bottom;
    public MeshFilter myMesh;
    public Mesh cubeMesh;
    public Mesh rampMesh;
    public Renderer myRender;
    Color mycolor;

    public Shader myShader;

    //public int rampLimit;
    //public int cubeLimit;
    public int distanceLimit;

   public  bool allowBuild;
    public bool isCube;
    bool builtSuccess;
    bool wallToBuild; //wall or gameobject near 
   // int rampsBuilt;
    //int cubesBuilt;

    // Use this for initialization
    void Start () {
        allowBuild = false;
        this.gameObject.SetActive(false);
        isCube = true;

        cubeMesh = myMesh.mesh;
        //mycolor = myRender.material.color;
        mycolor = Color.green;
        mycolor.a = 0.9f;
        myRender.material.color = mycolor;

        //rampLimit = 3;
        //cubeLimit = 3;
        builtSuccess = false;
        distanceLimit = 8;
        wallToBuild = false;
    }

    // Update is called once per frame
    void Update () {
    }
    private void LateUpdate()
    {
        CheckColor();
    }


    public void CheckColor()
    {

        Vector3 ghostPosition = ghostTransform.position;
        Vector3 playerPosition = playerTransform.position;
       
        Cell ghostCell = grid.GetCellInWorldPosition(ghostTransform.position);
        Cell playerCell = grid.GetCellInWorldPosition(playerTransform.position);

        Color newColor;

        if (isCube && 0 == GameManager.gManager.cubeLimit ||
           !isCube && 0 == GameManager.gManager.rampLimit)
        {
            newColor = Color.red;
        }

        else if (!(Mathf.Abs(ghostCell.globalPosition.x - playerCell.globalPosition.x) < distanceLimit &&
                Mathf.Abs(ghostCell.globalPosition.y - playerCell.globalPosition.y) < distanceLimit &&
                Mathf.Abs(ghostCell.globalPosition.z - playerCell.globalPosition.z) < distanceLimit))
        {
            newColor = Color.red;
        }

        else if (!ghostCell.transitable || !ghostCell.buildable)
        {
            newColor = Color.blue;
        }

        else if(grid.GetNeighbours(ghostCell).Count <=0)
        {
            newColor = Color.red;
        }

        else if(!grid.hasNeighboursOrWall(ghostCell))
        {
            newColor = Color.red;
        }

        else if(ghostCell == playerCell)
        {
            newColor = Color.red;
        }

        else
        {
            newColor = Color.green;
        }

        newColor.a = 0.9f;
        myRender.material.color = newColor;
    }

    public void Build(GameObject piecePrefab)
    {

        Cell ghostCell = grid.GetCellInWorldPosition(ghostTransform.position);
        Cell playerCell = grid.GetCellInWorldPosition(playerTransform.position);

        //Check that the cell exist. If all works, this has never to be false
        
        if (ghostCell != null)
        {
            if (ghostCell.buildable)
            {

            
            //Check that the cell is empty, that have no gameobject
            if ((ghostCell.gameObject == null || ghostCell.gameObject.tag == "Vacio")) 
            {
                    List<Cell> neighbours = grid.GetNeighbours(ghostCell);
                    foreach (Cell vecino in neighbours)
                    {
                        if (vecino == null)
                        {
                            wallToBuild = true;
                        }
                        else if (vecino.gameObject != null)
                        {
                            wallToBuild = true;
                        }

                    }
                    if (wallToBuild)
                    {


                        //Check that is near to the player
                        if ((Mathf.Abs(ghostCell.globalPosition.x - playerCell.globalPosition.x)) < distanceLimit &&
                            (Mathf.Abs(ghostCell.globalPosition.y - playerCell.globalPosition.y)) < distanceLimit &&
                            (Mathf.Abs(ghostCell.globalPosition.z - playerCell.globalPosition.z)) < distanceLimit
                           )
                        {
                            //Check that the ghost is not in the same cell that the player
                            if (ghostCell != playerCell)
                            {
                                GameObject newPiece = Instantiate(piecePrefab, ghostCell.globalPosition, ghostTransform.rotation);

                                ghostCell.gameObject = newPiece;
                                ghostCell.transitable = false;
                                ghostCell.destroyable = true;
                                switch (CameraMovement.Lateral)
                                {
                                    case CameraMovement.Axes.xPos: newPiece.transform.parent = right.transform; break;
                                    case CameraMovement.Axes.yNeg: newPiece.transform.parent = top.transform; break;
                                    case CameraMovement.Axes.yPos: newPiece.transform.parent = down.transform; break;
                                    case CameraMovement.Axes.zNeg: newPiece.transform.parent = front.transform; break;
                                    case CameraMovement.Axes.zPos: newPiece.transform.parent = bottom.transform; break;
                                    case CameraMovement.Axes.xNeg: newPiece.transform.parent = left.transform; break;
                                }
                                builtSuccess = true;
                                wallToBuild = false;
                            }
                        }
                    }
                }
            }
        }
    }
    public void RotateRamp (bool trueToClockwise)
    {
        if (trueToClockwise)
            this.gameObject.transform.Rotate(new Vector3(0.0f, 90.0f, 0.0f));
        else
            this.gameObject.transform.Rotate(new Vector3(0.0f, -90.0f, 0.0f));
    }
    public void SetBuildMode ()
    {
        if (allowBuild)
        {
            
            this.gameObject.SetActive(false);

            allowBuild = false;
        }
        else
        {
            this.transform.position = playerTransform.position;
            this.transform.up = playerTransform.up;
            
            this.gameObject.SetActive(true);
            allowBuild = true;
            
        }
    }
    public void SwitchBuilding ()
    {
        // isCube = !isCube;
        if (isCube)
        {
            myMesh.mesh = rampMesh;
            isCube = false;
        }
        else
        {
            myMesh.mesh = cubeMesh;
            isCube = true;
        }
    }
    public void BuildCubeOrRamp()
    {
        if (allowBuild)
        {


            if (isCube)
            {
                if (GameManager.gManager.cubeLimit > 0)
                {


                    Build(cubePrefab);
                    if (builtSuccess)
                    {
                        GameManager.gManager.PlaySound("place2");

                        GameManager.gManager.cubeLimit--;
                        builtSuccess = false;

                    }
                }
            }
            else
            {
                if (GameManager.gManager.rampLimit > 0)
                {


                    Build(rampPrefab);
                    if (builtSuccess)
                    {
                        GameManager.gManager.PlaySound("place2");

                        GameManager.gManager.rampLimit--;

                        builtSuccess = false;

                    }

                }
            }
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    

    }
   
    
  
    public void Destroy()
    {
        Cell ghostCell = grid.GetCellInWorldPosition(ghostTransform.position);
        Cell playerCell = grid.GetCellInWorldPosition(playerTransform.position);

        //Check that the cell exits
        if (ghostCell != null)
        {
            //Check that the cell can be destroyed and exists something to destroy
            //ghostCell.destroyable &&
            if ( ghostCell.gameObject != null)
            {
                if (ghostCell.destroyable)
                {


                    if ((Mathf.Abs(ghostCell.globalPosition.x - playerCell.globalPosition.x)) < distanceLimit &&
                        (Mathf.Abs(ghostCell.globalPosition.y - playerCell.globalPosition.y)) < distanceLimit &&
                        (Mathf.Abs(ghostCell.globalPosition.z - playerCell.globalPosition.z)) < distanceLimit
                       )
                    {
                        DestroyObject(ghostCell.gameObject);
                        ghostCell.transitable = true;
                        if (ghostCell.gameObject.tag == "RAMP")
                        {
                            GameManager.gManager.rampLimit++;
                        }
                        if (ghostCell.gameObject.tag == "CUBE")
                        {
                            GameManager.gManager.cubeLimit++;
                        }
                    }
                }
            }
        }
    }


}
