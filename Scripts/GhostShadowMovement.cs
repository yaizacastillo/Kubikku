using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostShadowMovement : MonoBehaviour {

    public GameObject ghost;
    Transform ghostTransform;

    Transform parent, wall;

    public enum Axis { x, y, z}
    public Axis axisLength;

    Renderer render;

    bool ghostPermision;
    public bool cameraPermision;

	// Use this for initialization
	void Start () {

        ghostPermision = ghost.activeSelf;
        cameraPermision = true;
        parent = transform.parent.transform;
        wall = transform.parent.parent.transform;
        ghostTransform = ghost.transform;
        render = GetComponent<Renderer>();
    }

    void Update()
    {

        ghostPermision = ghost.activeSelf;
        //turn the gameobject visible or invisible ddepenting of the ghost's state

        if (ghostPermision == true)
        {
            render.enabled = true;
        } else {
            render.enabled = false;
        }

        if (transform.localScale.x <= 0.2f) render.enabled = false;

        //render.enabled = ghost.activeSelf;
    }

    void LateUpdate () {

        //Changes his position and length
        parent.position = ghostTransform.position;
        SetLength();
        SetPosition();
        
    }


    void SetLength()
    {
        float newLength = 0;

        switch (axisLength)
        {
            case Axis.x:
                newLength = Mathf.Abs(parent.position.x - wall.position.x) - 0.5f;
            break;

            case Axis.y:
                newLength = Mathf.Abs(parent.position.y - wall.position.y) - 0.5f;
            break;

            case Axis.z:
                newLength = Mathf.Abs(parent.position.z - wall.position.z) - 0.5f;
            break;
        }

        transform.localScale = new Vector3(newLength, transform.localScale.y, transform.localScale.z);
       
    }

    void SetPosition()
    {
          transform.localPosition = new Vector3(transform.localScale.x / 2.0f + 0.5f, transform.localPosition.y, transform.localPosition.z);
    }

}


