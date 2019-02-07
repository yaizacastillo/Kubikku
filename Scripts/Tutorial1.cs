using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial1 : MonoBehaviour {
    public AxisMovement playerMovement;
    public CameraMovement camMovement;
    public Transform finalDoor;
    public GameManager trapScript;
    public PlayerInput inputsScript;
    public bool trapAdviceSaid = false;
    public bool rotateAdviceSaid = false;
    public bool objectiveAdviceSaid = false;
    public bool wallAdviceSaid = false;
    public bool collectAdviceSaid = false;


    public GameObject winPoint;

    public Text Rotate;
    public Text Wall;
    public Text Trap;
    public Text Build;
   
    public GameObject fondo;

    public bool CR_isRunning;
    bool isCinematicRunning;

    // Use this for initialization
    void Start () {
        CR_isRunning = false;
        StartCoroutine(PlayCinematic());
	}
	
	// Update is called once per frame
	void Update () {

        if (!isCinematicRunning)
        {
            if (!inputsScript.rotatedCam && Time.time >= 3.0f && !rotateAdviceSaid)
                if (!CR_isRunning)
                {
                    {
                        StartCoroutine(DisplayHideMessage(Rotate));
                        rotateAdviceSaid = true;
                    }
                }
            if (Time.time >= 20.0f && !playerMovement.changedWall && !wallAdviceSaid)
            {
                if (!CR_isRunning)
                {
                    StartCoroutine(DisplayHideMessage(Wall));
                    wallAdviceSaid = true;
                }
            }
            if (trapScript.trapped)
            {
                if (!CR_isRunning)
                {
                    StartCoroutine(DisplayHideMessage(Trap));
                    trapAdviceSaid = true;
                }
            }
            if (Vector3.Distance(this.gameObject.transform.position, winPoint.transform.position) <= 4.0f && !objectiveAdviceSaid)
            {
                if (!CR_isRunning)
                {
                    StartCoroutine(DisplayHideMessage(Build));
                    objectiveAdviceSaid = true;
                }
            }
        }
       
        
    }
    IEnumerator DisplayHideMessage(Text advice)
    {


        CR_isRunning = true;
            advice.gameObject.SetActive(true);
        fondo.SetActive(true);

            yield return new WaitForSeconds(3.0f);
        fondo.SetActive(false);
        

        advice.gameObject.SetActive(false);

        if (advice==Trap)
        {
            GameManager.gManager.trapped = false;
        }
            CR_isRunning = false;
        
    }

    IEnumerator PlayCinematic()
    {

        //original rotation 0 135 0
        //first rotation  45 270 90  

        Debug.Log("playing cinematic");


        float delay = 1.5f;
        float originalDelay = camMovement.secondsNeeded;
        camMovement.secondsNeeded = delay;
        isCinematicRunning = true;
        yield return new WaitForSeconds(0.1f);
        playerMovement.allowMovement = false;
        yield return new WaitForSeconds(1.0f);

        playerMovement.allowMovement = false;

        for (int i = 0; i < 6; i++)
            {
            finalDoor.localScale *= 1.1f;
            yield return new WaitForSeconds(0.2f);
            finalDoor.localScale *= 1f/1.1f;
            yield return new WaitForSeconds(0.2f);
        }
        playerMovement.allowMovement = false;

        yield return new WaitForSeconds(0.5f);
        camMovement.RotateTo(CameraMovement.Rotations.dRightUp);
        yield return new WaitForSeconds(delay);
        camMovement.RotateTo(CameraMovement.Rotations.lLeft);
        yield return new WaitForSeconds(delay);
        isCinematicRunning = false;
        playerMovement.allowMovement = true;

    }


}
