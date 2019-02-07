using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial2 : MonoBehaviour {


    public CameraMovement camMovement;
    public Transform finalDoor;
    public AxisMovement playerMovement;


    public Text Door;
    public Text Collect;
    public Text Restart;
    public Text Ascensor;
    public Text WaterEmpty;

    public GameObject fondo;
    bool CR_isRunning = false;
    bool collectAdviceSaid = false;
    bool resetAdviceSaid = false;
    bool waterAdviceSaid = false;

    int counter = 0;
    public GameObject winPoint;
    public GameObject ascensorGO;

    bool isCinematicRunning = false;


	// Use this for initialization
	void Start () {

        StartCoroutine(PlayCinematic());
	}
	
	// Update is called once per frame
	void Update () {


        if (!isCinematicRunning)
        {

            if (Vector3.Distance(this.gameObject.transform.position, winPoint.transform.position) <= 2.0f)
            {

                if (CameraMovement.Lateral != GameManager.gManager.requiredLateral

                    )
                {
                    if (!CR_isRunning)
                    {


                        StartCoroutine(DisplayHideMessage(Door));
                    }
                }

            }
            if (GameManager.gManager.waterOrEmpty && !CR_isRunning && !waterAdviceSaid)
            {
                StartCoroutine(DisplayHideMessage(WaterEmpty));
                waterAdviceSaid = true;

            }
            if (GameManager.gManager.rampLimit == 0 && !GameManager.gManager.rampsCollected && !collectAdviceSaid && !CR_isRunning)
            {
                StartCoroutine(DisplayHideMessage(Collect));
                collectAdviceSaid = true;
            }
            if (GameManager.gManager.rampLimit == 0 && !GameManager.gManager.rampsCollected && collectAdviceSaid && !CR_isRunning && !resetAdviceSaid)
            {
                StartCoroutine(DisplayHideMessage(Restart));
                resetAdviceSaid = true;
            }
            if (GameManager.gManager.rampsCollected && GameManager.gManager.rampLimit == 0 && (Vector3.Distance(this.gameObject.transform.position, winPoint.transform.position) >= 3.0f) && !CR_isRunning)
            {
                StartCoroutine(DisplayHideMessage(Restart));

            }
            if (Vector3.Distance(this.gameObject.transform.position, ascensorGO.transform.position) <= 2.0f && !CR_isRunning && counter < 2)
            {
                StartCoroutine(DisplayHideMessage(Ascensor));
                counter++;
            }
        }
    }
    IEnumerator DisplayHideMessage(Text advice)
    {



        CR_isRunning = true;
        advice.gameObject.SetActive(true);
        fondo.SetActive(true);


        yield return new WaitForSeconds(5.0f);
        advice.gameObject.SetActive(false);
        fondo.SetActive(false);


        if (advice == WaterEmpty)
        {
            GameManager.gManager.waterOrEmpty = false;
        }
        CR_isRunning = false;



    }


    IEnumerator PlayCinematic()
    {
        playerMovement.allowMovement = false;
        //original rotation 0 135 0
        //first rotation  0 45 0

        Debug.Log("playing cinematic");
        

        float delay = 2f;
        float originalDelay = camMovement.secondsNeeded;
        camMovement.secondsNeeded = delay;
        isCinematicRunning = true;

        yield return new WaitForSeconds(0.1f);
        playerMovement.allowMovement = false;
        yield return new WaitForSeconds(1f);
        

        for (int i = 0; i < 6; i++)
        {
            finalDoor.localScale *= 1.15f;
            yield return new WaitForSeconds(0.2f);
            finalDoor.localScale *= 1f / 1.15f;
            yield return new WaitForSeconds(0.2f);
        }
        playerMovement.allowMovement = false;

        yield return new WaitForSeconds(0.5f);
        camMovement.RotateTo(CameraMovement.Rotations.lLeft);
        yield return new WaitForSeconds(delay);
        camMovement.RotateTo(CameraMovement.Rotations.lLeft);
        yield return new WaitForSeconds(delay);
        camMovement.RotateTo(CameraMovement.Rotations.lLeft);
        yield return new WaitForSeconds(delay);
        isCinematicRunning = false;
        playerMovement.allowMovement = true;
    }
}
