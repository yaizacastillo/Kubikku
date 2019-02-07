using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class isVisible : MonoBehaviour
{

    //public Renderer myRender;
    //public CameraMovement camMovement;
    //public GameObject shadow;

    public float secondsNeededToFade;
    public float fadeLimit;
    public float shadowMaxLimit;

    public bool shadowTotalFade;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
   
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Wall") FadeObjectAndChilds(other.gameObject, false);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Wall") FadeObjectAndChilds(other.gameObject, true);
    }

    void FadeObjectAndChilds(GameObject obj, bool show)
    {
        Renderer render = obj.GetComponent<Renderer>();

        //Start with the gameObject
        if (render != null)
        {
            //if (obj.tag == "shadow")
            //{
            //    obj.GetComponent<GhostShadowMovement>().cameraPermision = show;
            //}
            //else
            //{
                StartCoroutine(Fade(obj, !show));
            //}
        }

        //Check all his childs, if any
        if (obj.transform.childCount != 0)
        {
            foreach (Transform child in obj.transform)
            {           
                    FadeObjectAndChilds(child.gameObject, show);
                if (child.GetComponent<SpriteRenderer>()|| child.tag == "Water")
                {
                    FadeSprites(child.gameObject, show);
                }  
            }
        }
    }
    void FadeSprites(GameObject obj, bool show)
    {
        if (show)
        {
            obj.gameObject.SetActive(true);
        }
        else
        {
            obj.gameObject.SetActive(false);

        }
    }
    public IEnumerator Fade(GameObject obj, bool fadeAway)
    {

        float _fadeLimit;
        float _maxLimit;

        if(shadowTotalFade && obj.tag == "shadow") {
            _fadeLimit = 0;
            _maxLimit = shadowMaxLimit;
        } else
        {
            _fadeLimit = fadeLimit;
            _maxLimit = 1;
        }



        //  CR_isRunning = true;
        if (obj.tag != "cristal")
        {


            Renderer render = obj.GetComponent<Renderer>();
            Color[] oldColors;
            Material[] materials;

            materials = render.materials;
            oldColors = new Color[materials.Length];


            //if (render.material.HasProperty("_Color"))
            //{
                // fade from opaque to transparent
            if (fadeAway)
            {
                // loop over 1 second backwards
                for (float i = _maxLimit; i >= _fadeLimit; i -= Time.deltaTime / secondsNeededToFade)
                {
                    for (int idx = 0; idx < materials.Length; idx++)
                    {
                        if (obj.name == "Bolsita")
                        {
                            materials[idx].DisableKeyword("_EMISSION");
                        }

                        if (materials[idx].HasProperty("_Color"))
                        {
                            Color col = materials[idx].color;
                            materials[idx].color = new Color(col.r, col.g, col.b, i);
                        }                 
                    } 
                    //oldColors = render.materials.color;
                    //render.material.color = new Color(oldColors.r, oldColors.g, oldColors.b, i);

                    yield return null;
                }
            }
            // fade from transparent to opaque
            else
            {
                // loop over 1 second
                for (float i = _fadeLimit; i <= _maxLimit; i += Time.deltaTime / secondsNeededToFade)
                {
                    for (int idx = 0; idx < materials.Length; idx++)
                    {
                        if (obj.name == "Bolsita")
                        {
                            materials[idx].EnableKeyword("_EMISSION");
                        }

                        if (materials[idx].HasProperty("_Color"))
                        {
                            Color col = materials[idx].color;
                            materials[idx].color = new Color(col.r, col.g, col.b, i);
                        }
                    }

                    //oldColors = render.material.color;
                    //render.material.color = new Color(oldColors.r, oldColors.g, oldColors.b, i);
                    yield return null;
                }
            }
            //}
            // yield return new WaitForSeconds(3.0f);

            //  CR_isRunning = false;
        }
    }
}
