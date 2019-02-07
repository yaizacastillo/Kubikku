using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectPieces : MonoBehaviour {
    public int nºPieces = 0;
    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            // Debug.Log("collected");
            GameManager.gManager.PlaySound("pickUp");
            GameManager.gManager.rampLimit += nºPieces;
            GameManager.gManager.rampsCollected = true;
            Destroy(this.gameObject);
        }
    }
    
}
