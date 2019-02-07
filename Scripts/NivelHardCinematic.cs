using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NivelHardCinematic : MonoBehaviour {

    public AxisMovement playerMovement;
    public CameraMovement camMovement;
    public bool isCinematicRunning;
    public Transform finalDoor;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(PlayCinematic());
    }

    // Update is called once per frame
    void Update()
    {
    }

    IEnumerator PlayCinematic()
    {
        playerMovement.allowMovement = false;
        //original rotation 0 135 0
        //first rotation  45 270 270


        Debug.Log("playing cinematic");


        float delay = 2f;
        float originalDelay = camMovement.secondsNeeded;
        camMovement.secondsNeeded = delay;
        isCinematicRunning = true;

        yield return new WaitForSeconds(0.1f);
        playerMovement.allowMovement = false;
        yield return new WaitForSeconds(1.0f);


        for (int i = 0; i < 6; i++)
        {
            finalDoor.localScale *= 1.15f;
            yield return new WaitForSeconds(0.2f);
            finalDoor.localScale *= 1f / 1.15f;
            yield return new WaitForSeconds(0.2f);
        }
        playerMovement.allowMovement = false;

        yield return new WaitForSeconds(0.5f);
        camMovement.RotateTo(CameraMovement.Rotations.dLeftDown);
        yield return new WaitForSeconds(delay);
        isCinematicRunning = false;
        playerMovement.allowMovement = true;
    }
}
