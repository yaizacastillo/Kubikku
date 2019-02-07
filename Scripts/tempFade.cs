using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tempFade : MonoBehaviour {

    bool started;
    Renderer render;

	// Use this for initialization
	void Start () {
        started = false;
        render = this.gameObject.GetComponent<Renderer>();
	}
	
	// Update is called once per frame
	void Update () {
		
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("down");
            StartCoroutine(FadeImage(true));
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("up");
            StartCoroutine(FadeImage(false));

        }


    }

    public IEnumerator FadeImage(bool fadeAway)
    {
        //  CR_isRunning = true;

        Color old;

        // fade from opaque to transparent
        if (fadeAway)
        {
            // loop over 1 second backwards
            for (float i = 1; i >= 0; i -= Time.deltaTime)
            {

                old = render.material.color;
                render.material.color = new Color(old.r, old.g, old.b, i);

                yield return null;
            }
        }
        // fade from transparent to opaque
        else
        {
            // loop over 1 second
            for (float i = 0; i <= 1; i += Time.deltaTime)
            {
                old = render.material.color;
                render.material.color = new Color(old.r, old.g, old.b, i);
                yield return null;
            }
        }
        // yield return new WaitForSeconds(3.0f);

        //  CR_isRunning = false;

    }
}
