using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour {

    public static GameManager gManager;
    public int rampLimit;
    public int cubeLimit;
    //audios
  public  AudioClip rotateAudio;
    public AudioClip pickUpAudio;
    public AudioClip placeAudio2;

    public AudioClip victoryAudio;
    public AudioClip deathAudio;


    public AudioSource myAudio;
    public bool trapped;
    public bool waterOrEmpty;
    public AxisMovement playerMovement;
    public AxisMovement ghostMovement;
    public CameraMovement cameraMovement;
    public PlayerInput inputScript;
    public Transform requiredPosition;
    public CameraMovement.Axes requiredLateral;
    public string nextLevel;
    public GameObject down, left, right, top, front, bottom;

    public LvlTransition transitionScript;
    //particulas
    public GameObject particulas;
    public GameObject particulas2;

    //public Vector3 positionparticulas;
    public float shakeDuration, shakeMagnitude;

    //Canvas
    public Canvas HUD;
    bool openedMenu;
    public HUD menu;

    public GameObject blood;

    public bool rampsCollected = false;

    [HideInInspector] public static bool tutorial1 = false, level1 = false, tutorial2 = false;

    // Use this for initialization
    private void Awake()
    {
         

        if (gManager == null)
        {
            gManager = this;
        }
        else if (gManager != this)
        {
            Destroy(gameObject);
        }
        if (transitionScript != null)
        {


            StartCoroutine(transitionScript.FadeImage(true));
        }
    }

    void Start () {
        openedMenu = false;
     

    }
    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "Tutorial1")
        {
            Physics.gravity = new Vector3(0, 0, 6.0f);
        }
        else if (SceneManager.GetActiveScene().name == "Nivel1")
        {
            Physics.gravity = new Vector3(0, 0, -6.0f);

        }
        else if (SceneManager.GetActiveScene().name == "NivelHard")
        {
            Physics.gravity = new Vector3(0, 0, 6.0f);

        }
        else if (SceneManager.GetActiveScene().name == "Tutorial 2")
        {
            Physics.gravity = new Vector3(0, -6f, 0f);

        }
    }
    public void Loadscene(string scene)
    {

        if (scene == "thisScene")
        {

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        }
        else
        {
             SceneManager.LoadScene(scene);
           
        }
    }
    public IEnumerator LoadNextLevel(string level)
    {
        inputScript.blocked = true;
        playerMovement.ResetRotation();
        PlaySound("win");
       // Instantiate(particulas, positionparticulas, Quaternion.identity);
        particulas.SetActive(true);
        particulas2.SetActive(true);

        yield return new WaitForSeconds(4.5f);

        StartCoroutine(transitionScript.FadeImage(false));
        
        switch (SceneManager.GetActiveScene().name)
        {
            case "Tutorial1":
                tutorial1 = true;
                break;
            case "Nivel1":
                level1 = true;
                break;
            case "Tutorial 2":
                tutorial2 = true;
                break;
        }

       // yield return new WaitForSeconds(victoryAudio.length);
          yield return new WaitForSecondsRealtime(0.8f);
        //   Debug.Log("paso");
        /* if (!transitionScript.CR_isRunning)
         {


             Debug.Log("cargoLVL");
             SceneManager.LoadScene(nextLevel);
             StartCoroutine(transitionScript.FadeImage(true));
         }*/
        SceneManager.LoadScene(level);
        inputScript.blocked = false;



    }

    public void CheckPosition(LevelGrid grid, Vector3 playerPosition, Vector3 belowPosition)
    {
        if (grid.GetCellInWorldPosition(playerPosition).trap || (belowPosition != null && grid.GetCellInWorldPosition(belowPosition) != null && grid.GetCellInWorldPosition(belowPosition).trap))
        {
            PlaySound("death");
            trapped = true;
            if (!playerMovement.isFalling && grid.GetCellInWorldPosition(playerPosition).gameObject.name != "Agua")
            {
                GenerateBlood(grid, playerPosition);
            }
            Respawn();
        }
        if (grid.GetCellInWorldPosition(playerPosition).isEmpty && playerMovement.gameObject.transform.up == grid.GetCellInWorldPosition(playerPosition).gameObject.transform.up)
        {
            PlaySound("death");
            trapped = true;
            waterOrEmpty = true;

            //GenerateBlood(grid, playerPosition);
            Respawn();
        }



        //Check traps

    }
    public void PlaySound (string soundName)
    {
      
        switch (soundName)
        {
            case "rotate":
                myAudio.clip = rotateAudio;
                myAudio.Play();
                break;
            case "pickUp":
                myAudio.clip = pickUpAudio;
                myAudio.Play();
                break;
            case "place2":
                myAudio.clip = placeAudio2;
                myAudio.Play();
                break;
            case "win":
                myAudio.clip = victoryAudio;
                myAudio.Play();
                break;
            case "death":
                myAudio.clip = deathAudio;
                myAudio.Play();
                break;
            default:
                break;
        }

    }


    public void Respawn() {
        playerMovement.ResetPosition();
        playerMovement.ResetRotation();
        cameraMovement.ResetGravity();
        playerMovement.newPlayer();
        StartCoroutine(cameraMovement.Shake(shakeDuration, shakeMagnitude));
    }

    public void ShowControls()
    {
        Time.timeScale = 0;
       // HUD.enabled = false;
        menu.OpenCloseControls(true);
    }

  /*  public void HideControls()
    {
        Time.timeScale = 1;
        Controls.enabled = false;
        HUD.enabled = true;
    }*/

    public void OpenCloneMenu()
    {
        openedMenu = !openedMenu;

      //  menu.OpenCloseMenu(openedMenu);
       // ghostMovement.AllowDenyMovement(!openedMenu);
        //playerMovement.AllowDenyMovement(!openedMenu);
    }

    public void GenerateBlood(LevelGrid grid, Vector3 playerTransform)
    {
        Vector3 bloodRotation = Vector3.zero;
        float extraRotation = Random.Range(-90, 90);

        switch (CameraMovement.Lateral)
        {
            case CameraMovement.Axes.xNeg: bloodRotation = new Vector3(extraRotation, 90, 0); break;
            case CameraMovement.Axes.xPos: bloodRotation = new Vector3(extraRotation, -90, 0); break;
            case CameraMovement.Axes.yNeg: bloodRotation = new Vector3(-90, extraRotation, 0); break;
            case CameraMovement.Axes.yPos: bloodRotation = new Vector3(90, extraRotation, 0); break;
            case CameraMovement.Axes.zNeg: bloodRotation = new Vector3(0, 0, extraRotation); break;
            case CameraMovement.Axes.zPos: bloodRotation = new Vector3(180, 0, extraRotation); break;
        }

        Vector3 belowPos = Vector3.zero;

        switch (CameraMovement.Lateral)
        {
            case CameraMovement.Axes.xNeg: belowPos = new Vector3(1, 0, 0); break;
            case CameraMovement.Axes.xPos: belowPos = new Vector3(-1, 0, 0); break;
            case CameraMovement.Axes.yNeg: belowPos = new Vector3(0, 1, 0); break;
            case CameraMovement.Axes.yPos: belowPos = new Vector3(0, -1, 0); break;
            case CameraMovement.Axes.zNeg: belowPos = new Vector3(0, 0, 1); break;
            case CameraMovement.Axes.zPos: belowPos = new Vector3(0, 0, -1); break;
        }

     GameObject boodInstance=   Instantiate(blood, grid.GetCellInWorldPosition(playerTransform).globalPosition + belowPos / 2.01f, Quaternion.Euler(bloodRotation.x, bloodRotation.y, bloodRotation.z));
        switch (CameraMovement.Lateral)
        {
            case CameraMovement.Axes.xPos: boodInstance.transform.parent = right.transform; break;
            case CameraMovement.Axes.yNeg: boodInstance.transform.parent = top.transform; break;
            case CameraMovement.Axes.yPos: boodInstance.transform.parent = down.transform; break;
            case CameraMovement.Axes.zNeg: boodInstance.transform.parent = front.transform; break;
            case CameraMovement.Axes.zPos: boodInstance.transform.parent = bottom.transform; break;
            case CameraMovement.Axes.xNeg: boodInstance.transform.parent = left.transform; break;
        }
    }


}
