using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LvlTransition : MonoBehaviour {


    Color alpha;
   public Image myImage;
    Sprite mySprite;
   public bool CR_isRunning;

	// Use this for initialization
	void Start () {

        CR_isRunning = true;

        //   mySprite = GetComponent<SpriteRenderer>().sprite;
        myImage = GetComponent<Image>();

        
	}
   
       

    // Update is called once per frame
   public IEnumerator FadeImage(bool fadeAway)
    {
      //  CR_isRunning = true;

        // fade from opaque to transparent
        if (fadeAway)
        {
            // loop over 1 second backwards
            for (float i = 1; i >= 0; i -= Time.deltaTime)
            {
                // set color with i as alpha
                 myImage.color = new Color(0, 0, 0, i);
             //   myImage.color.a = i;
                yield return null;
            }
        }
        // fade from transparent to opaque
        else
        {
            // loop over 1 second
            for (float i = 0; i <= 5; i += Time.deltaTime)
            {
                // set color with i as alpha
                myImage.color = new Color(0, 0, 0, i);
                yield return null;
            }
        }
       // yield return new WaitForSeconds(3.0f);

      //  CR_isRunning = false;

    }
}
