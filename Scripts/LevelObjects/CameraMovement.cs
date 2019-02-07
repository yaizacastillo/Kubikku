using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Rotates all his child gameObjects. In this case, the camera and the level light
/// </summary>
public class CameraMovement : MonoBehaviour {

    public float secondsNeeded;

    private float timer;

   public Quaternion startRotation, initialRotation;
    Transform targetTransform;

    public enum Rotations {stop, lLeft, lRight, dLeftUp, dLeftDown, dRightUp, dRightDown }
    [HideInInspector]public Rotations actualRotation;

    public enum Axes { xPos, yPos, zPos, xNeg, yNeg, zNeg }
     static public Axes DiagonalLeft, DiagonalRight, Lateral;


    public Axes initialDLeft, initialDRight, initialLateral;
    public Axes respawnDLeft, respawnDRight, respawnLateral;
    public Vector3 respawnRotation;

    // Use this for initialization
    void Awake () {

        DiagonalLeft = initialDLeft;
        DiagonalRight = initialDRight;
        Lateral = initialLateral;

        actualRotation = Rotations.stop;
        startRotation = transform.rotation;
        initialRotation = transform.rotation;

        GameObject tempGameObject = new GameObject();
        targetTransform = tempGameObject.transform;
        targetTransform.rotation= transform.rotation;


        //light.transform.rotation = Quaternion.LookRotation(this.transform.position - camera.transform.position);


        //light.transform.position = camera.transform.position; 

}
	
	void Update () {

        
        //If its in movement, rotate
        if (actualRotation != Rotations.stop)
        {
            timer += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(startRotation, targetTransform.rotation, timer / secondsNeeded);
        }

        if (transform.rotation == targetTransform.rotation) actualRotation = Rotations.stop;
	}

    
    /// <summary>
    /// Rotate the targetOrientation with the movement assigned by parameter.
    /// IMPORTANT--> This function does not rotate the camera, this is done in the update function
    /// </summary>
    /// <param name="rotationDirection"></param>
    public void RotateTo(Rotations rotationDirection) {
        GameManager.gManager.PlaySound("rotate");

        //If the camera if moving, return immediatly
        if (actualRotation != Rotations.stop) return;

        timer = 0;

        startRotation = transform.rotation;
        targetTransform.rotation = transform.rotation;

        //Assign the new direction
        actualRotation = rotationDirection;

        Axes targetAxe = Axes.yNeg;
        bool clockwise = true;

        //Select the vertex to rotate
        switch (rotationDirection) {
            case Rotations.lLeft: targetAxe = Lateral; break;
            case Rotations.lRight: targetAxe = Lateral; break;
            case Rotations.dLeftUp: targetAxe = DiagonalLeft; break;
            case Rotations.dLeftDown: targetAxe = DiagonalLeft; break;
            case Rotations.dRightUp: targetAxe = DiagonalRight; break;
            case Rotations.dRightDown: targetAxe = DiagonalRight; break;
        }

        //Selet if going clockwise or counterclockwise
        if (targetAxe == Axes.yPos || targetAxe == Axes.xPos || targetAxe == Axes.zPos)
        {
            if (rotationDirection == Rotations.lLeft || rotationDirection == Rotations.dLeftUp || rotationDirection == Rotations.dRightDown)
            {
                clockwise = true;
            }
            else
            {
                clockwise = false;
            }
        }
        else
        {
            if (rotationDirection == Rotations.lLeft || rotationDirection == Rotations.dLeftUp || rotationDirection == Rotations.dRightDown)
            {
                clockwise = false;
            }
            else
            {
                clockwise = true;
            }
        }
        
        //Rotate 90 degrees de target rotation
        switch (targetAxe)
        {
            case Axes.yPos:
                if (clockwise) targetTransform.RotateAround(transform.position, Vector3.up, -90);
                else targetTransform.RotateAround(transform.position, Vector3.up, 90);
                break;
            case Axes.yNeg:
                if (clockwise) targetTransform.RotateAround(transform.position, Vector3.up, -90);
                else targetTransform.RotateAround(transform.position, Vector3.up, 90);
                break;
            case Axes.xPos:
                if (clockwise) targetTransform.RotateAround(transform.position, Vector3.right, -90);
                else targetTransform.RotateAround(transform.position, Vector3.right, 90);
                break;
            case Axes.xNeg:
                if (clockwise) targetTransform.RotateAround(transform.position, Vector3.right, -90);
                else targetTransform.RotateAround(transform.position, Vector3.right, 90);
                break;
            case Axes.zPos:
                if (clockwise) targetTransform.RotateAround(transform.position, Vector3.forward, -90);
                else targetTransform.RotateAround(transform.position, Vector3.forward, 90);
                break;
            case Axes.zNeg:
                if (clockwise) targetTransform.RotateAround(transform.position, Vector3.forward, -90);
                else targetTransform.RotateAround(transform.position, Vector3.forward, 90);
                break;
        }

        //Change the assigned axes for the next rotations
        if (Lateral != targetAxe) Lateral = ChangeAxis(Lateral, targetAxe, clockwise);
        if (DiagonalRight != targetAxe) DiagonalRight = ChangeAxis(DiagonalRight, targetAxe, clockwise);
        if (DiagonalLeft != targetAxe) DiagonalLeft = ChangeAxis(DiagonalLeft, targetAxe, clockwise);
    }

    Axes ChangeAxis(Axes axis, Axes targetAxe, bool clockwise) {

        if (targetAxe == Axes.zPos || targetAxe == Axes.zNeg)
        {
            //Order --> x y -x -y
            if (clockwise)
            {           
                if (axis == Axes.xPos) return Axes.yNeg;
                else if (axis == Axes.yNeg) return Axes.xNeg;
                else if (axis == Axes.xNeg) return Axes.yPos;
                else if (axis == Axes.yPos) return Axes.xPos;
            }
            else
            {
                if (axis == Axes.xPos) return Axes.yPos;
                else if (axis == Axes.yPos) return Axes.xNeg;
                else if (axis == Axes.xNeg) return Axes.yNeg;
                else if (axis == Axes.yNeg) return Axes.xPos;
            }
        }

        else if (targetAxe == Axes.xPos || targetAxe == Axes.xNeg)
        {
            //Order --> y z -y -z
            if (clockwise)
            {      
                if (axis == Axes.yPos) return Axes.zNeg;
                else if (axis == Axes.zNeg) return Axes.yNeg;
                else if (axis == Axes.yNeg) return Axes.zPos;
                else if (axis == Axes.zPos) return Axes.yPos;
            }
            else
            {
                if (axis == Axes.yPos) return Axes.zPos;
                else if (axis == Axes.zPos) return Axes.yNeg;
                else if (axis == Axes.yNeg) return Axes.zNeg;
                else if (axis == Axes.zNeg) return Axes.yPos;
            }
        }
        else if (targetAxe == Axes.yPos || targetAxe == Axes.yNeg)
        {
            //Order --> z x -z -x
            if (clockwise)
            {    
                if (axis == Axes.zPos) return Axes.xNeg;
                else if (axis == Axes.xNeg) return Axes.zNeg;
                else if (axis == Axes.zNeg) return Axes.xPos;
                else if (axis == Axes.xPos) return Axes.zPos;
            }
            else
            {
                if (axis == Axes.zPos) return Axes.xPos;
                else if (axis == Axes.xPos) return Axes.zNeg;
                else if (axis == Axes.zNeg) return Axes.xNeg;
                else if (axis == Axes.xNeg) return Axes.zPos;
            }
        }
        
        return axis;
    }

    public void ResetGravity()
    {
        actualRotation = Rotations.stop;

        DiagonalLeft = respawnDLeft;
        DiagonalRight = respawnDRight;
        Lateral = respawnLateral;
        transform.rotation = Quaternion.Euler(respawnRotation);


    }

    public IEnumerator Shake(float _duration, float _magnitude)
    {
        Vector3 _originalPosition = this.transform.localPosition;

        //Vibration.instance.StartVibration(_magnitude * 5, _magnitude * 5, _duration);

        float _elapsed = 0;

        while (_elapsed < _duration)
        {


            Vector2 movement = new Vector2(Random.Range(-1f, 1f) * _magnitude, Random.Range(-1f, 1f) * _magnitude);

            this.transform.localPosition += new Vector3(movement.x, movement.y, _originalPosition.z);

            _elapsed += Time.deltaTime;

            yield return null;
            this.transform.localPosition = _originalPosition;

        }

        this.transform.localPosition = _originalPosition;
        yield break;
    }
}
