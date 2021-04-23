using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPossessable : MonoBehaviour
{
    public bool isHighlighted;
    public GameObject text;

    private void Update()
    {
        // if the object is highlighted
        if (isHighlighted)
        {
            // make the object glow
            GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
            text.SetActive(true);
        }

        // if the object is not highlighted
        else
        {
            // stop the object from glowing
            GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");
            text.SetActive(false);
        }
    }

    public void HighlightToggle()
    {
        if (!isHighlighted)
        {
            isHighlighted = true;
        }

        else
        {
            isHighlighted = false;
        }
    }
}
