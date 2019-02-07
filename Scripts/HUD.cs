using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HUD : MonoBehaviour {

    // Use this for initialization
    public Text RampsLeft;
    public Text CubesLeft;

    public List<GameObject> menuElements;

    public GameObject controls;

    GameObject rampsLeftObject;
    GameObject cubesLeftObject;

    bool openedMenu;

	void Start () {

        openedMenu = false;
       // controls.SetActive(false);
        rampsLeftObject = RampsLeft.gameObject;
        cubesLeftObject = CubesLeft.gameObject;

        /*foreach(GameObject element in menuElements)
        {
            element.SetActive(true);
        }*/

        EventSystem.current.SetSelectedGameObject(null);

        rampsLeftObject.SetActive(true);
        cubesLeftObject.SetActive(true);
	}
	
	// Update is called once per frame
	void Update () {
        CubesLeft.text = "x"+ GameManager.gManager.cubeLimit.ToString();
        RampsLeft.text = "x" + GameManager.gManager.rampLimit.ToString();
    }

    public void OpenCloseMenu(bool _openedMenu)
    {
        openedMenu = _openedMenu;

        /*foreach(GameObject element in menuElements)
        {
            element.SetActive(openedMenu);
        }*/

        Debug.Log("Opened menu");

        if(openedMenu)
        {
            EventSystem.current.SetSelectedGameObject(menuElements[0]);
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public void OpenCloseControls(bool active)
    {

        if (controls.activeInHierarchy)
        {
            controls.SetActive(false);

        }
        else
        {
            controls.SetActive(active);

        }
    }



}
