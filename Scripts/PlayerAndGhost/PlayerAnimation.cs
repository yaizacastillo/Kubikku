using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{

    public GameObject player;
    public float maxGrowth, speed;
    float totalGrowth = 0;
    private enum direction { up, down }
    private direction actualDirection = direction.up;
    Vector3 upPos;
    Vector3 abovePos;
    public float rotationSpeed;
    public static float totalRotated = 0;
    public float maxRotation;

    // Update is called once per frame
    void Update()
    {

        if (!player.GetComponent<AxisMovement>().isMoving)
        {
            Levitation();
        }

        else
        {
            Inclination();
        }
    }

    void Levitation()
    {
        //RESET ROTATION resting the rotated degrees to stop slowly
        if (totalRotated > 0)
        {
            transform.Rotate(-Time.deltaTime * rotationSpeed, 0, 0);
            totalRotated -= Time.deltaTime * rotationSpeed;
        }

        //to know the up direction and the down direction

        else
        {
            checkDirections();

            if (actualDirection == direction.up)
                transform.position += upPos * speed;

            else
                transform.position += abovePos * speed;

            totalGrowth += 1 * speed;

            if (totalGrowth >= maxGrowth)
            {
                totalGrowth = 0;

                if (actualDirection == direction.up)
                    actualDirection = direction.down;

                else
                    actualDirection = direction.up;
            }
        }
        
    }

    void Inclination()
    {

        if(totalRotated<maxRotation)
        {
            transform.Rotate(Time.deltaTime * rotationSpeed, 0, 0);

            //save angles rotated to reset them later
            totalRotated += Time.deltaTime * rotationSpeed;
        }
    }

    void checkDirections()
    {
        upPos = Vector3.zero;

        switch (CameraMovement.Lateral)
        {
            case CameraMovement.Axes.xNeg: upPos = new Vector3(-1, 0, 0); break;
            case CameraMovement.Axes.xPos: upPos = new Vector3(1, 0, 0); break;
            case CameraMovement.Axes.yNeg: upPos = new Vector3(0, -1, 0); break;
            case CameraMovement.Axes.yPos: upPos = new Vector3(0, 1, 0); break;
            case CameraMovement.Axes.zNeg: upPos = new Vector3(0, 0, -1); break;
            case CameraMovement.Axes.zPos: upPos = new Vector3(0, 0, 1); break;
        }

        abovePos = Vector3.zero;

        switch (CameraMovement.Lateral)
        {
            case CameraMovement.Axes.xNeg: abovePos = new Vector3(1, 0, 0); break;
            case CameraMovement.Axes.xPos: abovePos = new Vector3(-1, 0, 0); break;
            case CameraMovement.Axes.yNeg: abovePos = new Vector3(0, 1, 0); break;
            case CameraMovement.Axes.yPos: abovePos = new Vector3(0, -1, 0); break;
            case CameraMovement.Axes.zNeg: abovePos = new Vector3(0, 0, 1); break;
            case CameraMovement.Axes.zPos: abovePos = new Vector3(0, 0, -1); break;
        }
    }
}

