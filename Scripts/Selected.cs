using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Selected : EventTrigger, ISelectHandler
{

    float HightlightedScale;
    bool isControl;
    HUD hud;

	// Use this for initialization
	void Start () {
        HightlightedScale = 1.2f;
        isControl = this.name == "Controls_Button";
        hud = GameManager.gManager.menu;
	}
	
	// Update is called once per frame
	void Update () {

        


	}

    public override void OnSelect(BaseEventData eventData)
    {
        this.transform.localScale *= HightlightedScale;
        if (isControl) hud.OpenCloseControls(true);
    }


    public override void OnDeselect(BaseEventData data)
    {
        this.transform.localScale /= HightlightedScale;
        if (isControl) hud.OpenCloseControls(false);
    }


}
