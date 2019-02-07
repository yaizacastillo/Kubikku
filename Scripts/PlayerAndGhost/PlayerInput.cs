using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour {

    public CameraMovement camMovement;
    public AxisMovement PlayerMovement;
    public AxisMovement GhostMovement;
    public BuildDestroy BuildScript;
    public PlatformMovement platformMovement;

    GameManager gameManager;

    public bool allowAdminOrientations;
    public float sensibility;
    public bool blocked = false;
    public bool rotatedCam = false; 
	// Use this for initialization
	void Start () {
        gameManager = GameManager.gManager;

            gameManager.OpenCloneMenu();

    }

    // Update is called once per frame
    void Update()
    {
        if (!blocked)
        {


            //if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Controller_Start"))
            //{
            //    gameManager.OpenCloneMenu();
            //}

            //Change Orientation

            if (PlayerMovement.allowMovement)
            {

                if ((Input.GetKeyDown(KeyCode.A) || Input.GetAxisRaw("Controller_LTrigger") == 1) && !PlayerMovement.isClimbing && !BuildScript.allowBuild)
                {
                    camMovement.RotateTo(CameraMovement.Rotations.lLeft);
                    rotatedCam = true;
                }
                else if ((Input.GetKeyDown(KeyCode.D) || Input.GetAxisRaw("Controller_RTrigger") == 1) && !PlayerMovement.isClimbing && !BuildScript.allowBuild)
                {
                    camMovement.RotateTo(CameraMovement.Rotations.lRight);
                    rotatedCam = true;

                }
            }

            //Change mode fron build to move and viceversa
            if (Input.GetKeyDown(KeyCode.Q) || Input.GetButtonDown("Controller_Y")) BuildScript.SetBuildMode();

            //When is in "moving" mode
            if (!BuildScript.allowBuild)
            {
                // Debug.Log("Horizontal " + Input.GetAxisRaw("Controller_LJoystickH"));
                //Debug.Log("Vertical " + Input.GetAxisRaw("Controller_LJoystickV"));

                //Player movement
                if (platformMovement == null || (platformMovement.isPlayer() && PlatformMovement.moving == false) || !platformMovement.isPlayer())
                {
                    if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetAxisRaw("Controller_LJoystickV") < -sensibility) PlayerMovement.MoveTo(AxisMovement.Directions.leftUp);
                    else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetAxisRaw("Controller_LJoystickV") > sensibility) PlayerMovement.MoveTo(AxisMovement.Directions.leftDown);
                    else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetAxisRaw("Controller_LJoystickH") > sensibility) PlayerMovement.MoveTo(AxisMovement.Directions.rightUp);
                    else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetAxisRaw("Controller_LJoystickH") < -sensibility) PlayerMovement.MoveTo(AxisMovement.Directions.rightDown);
                }
            }
            else
            {
                //Ghost movement
                if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetAxisRaw("Controller_LJoystickV") < -sensibility) GhostMovement.MoveTo(AxisMovement.Directions.leftUp);
                else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetAxisRaw("Controller_LJoystickV") > sensibility) GhostMovement.MoveTo(AxisMovement.Directions.leftDown);
                else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetAxisRaw("Controller_LJoystickH") > sensibility) GhostMovement.MoveTo(AxisMovement.Directions.rightUp);
                else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetAxisRaw("Controller_LJoystickH") < -sensibility) GhostMovement.MoveTo(AxisMovement.Directions.rightDown);
                else if (Input.GetKeyDown(KeyCode.S) || Input.GetAxisRaw("Controller_RJoystickV") < -sensibility) GhostMovement.MoveTo(AxisMovement.Directions.up);
                else if (Input.GetKeyDown(KeyCode.X) || Input.GetAxisRaw("Controller_RJoystickV") > sensibility) GhostMovement.MoveTo(AxisMovement.Directions.down);


                //Building Inputs
                if (Input.GetKeyDown(KeyCode.W) || Input.GetButtonDown("Controller_A")) BuildScript.BuildCubeOrRamp();
                //else if (Input.GetKeyDown(KeyCode.O) || Input.GetButtonDown("Controller_B")) BuildScript.Destroy();
                else if (Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("Controller_X")) BuildScript.SwitchBuilding();
                else if (Input.GetKeyDown(KeyCode.A) || Input.GetButtonDown("Controller_LBumper")) BuildScript.RotateRamp(false);
                else if (Input.GetKeyDown(KeyCode.D) || Input.GetButtonDown("Controller_RBumper")) BuildScript.RotateRamp(true);
            }
        }
        //Change gravity manually if the option is actiated
        if (allowAdminOrientations)
        {
            if (Input.GetKeyDown(KeyCode.Z)) camMovement.RotateTo(CameraMovement.Rotations.dRightDown);
            else if (Input.GetKeyDown(KeyCode.E)) camMovement.RotateTo(CameraMovement.Rotations.dRightUp);
            else if (Input.GetKeyDown(KeyCode.C)) camMovement.RotateTo(CameraMovement.Rotations.dLeftDown);
            else if (Input.GetKeyDown(KeyCode.Q)) camMovement.RotateTo(CameraMovement.Rotations.dLeftUp);
        }

        
    }
}
