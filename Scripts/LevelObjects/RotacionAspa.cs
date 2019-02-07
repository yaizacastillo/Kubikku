using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotacionAspa : MonoBehaviour {

    // Use this for initialization
    public float speed;
    public Vector3 direction;
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(direction, speed * Time.deltaTime);
        
    }
}
