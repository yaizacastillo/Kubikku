using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {

    // Use this for initialization
    public Image fondoNivel1;
    public Image fondoTut2;
    public Image fondoNivel2;


    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        
        if (GameManager.tutorial1)
        {
            fondoNivel1.color = new Color(255f, 255f, 255f);
        }
        if (GameManager.level1)
        {
            fondoTut2.color = new Color(255f, 255f, 255f);
        }
        if (GameManager.tutorial2)
        {
            fondoNivel2.color = new Color(255f, 255f, 255f);
        }


    }

    public void LoadTutorial1()
    {

        GameManager.gManager.Loadscene("Tutorial1");
    }
   
    public void LoadLevell1()
    {
        if (GameManager.tutorial1)
        {
            GameManager.gManager.Loadscene("Nivel1");
        }
    }
    public void LoadTutorial2()
    {
        if (GameManager.level1)
        {
            GameManager.gManager.Loadscene("Tutorial 2");
        }
    }
    public void LoadLevel2()
    {
        if (GameManager.tutorial2)
        {
            GameManager.gManager.Loadscene("NivelHard");
        }
    }
    public void LoadSettings()
    {
        GameManager.gManager.Loadscene("Settings");
    }
    public void Exit()
    {
        Application.Quit();
    }
}
