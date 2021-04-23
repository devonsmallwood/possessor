using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePad : MonoBehaviour
{
    public SFX sfx;

    private void OnCollisionEnter(Collision collision)
    {
        // if the object is a Red Possessable
        if (collision.collider.tag == "Red Possessable")
        {
            // play pressure pad down sound
            sfx.PressurePadDown();

            // make the object glow
            GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");

            foreach (Transform child in transform)
            {
                // make each of the object's children glow
                child.GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");

                // if the child object is a Door
                if (child.tag == "Door")
                {
                    // if the child object is active
                    if (child.gameObject.activeInHierarchy)
                    {
                        // deactivate the child object
                        child.gameObject.SetActive(false);
                    }

                    // if the child object is inactive
                    else
                    {
                        // activate the child object
                        child.gameObject.SetActive(true);
                    }
                }
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // play pressure pad up sound
        sfx.PressurePadUp();

        // if the object is a Red Possessable
        if (collision.collider.tag == "Red Possessable")
        {
            // make the object glow
            GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");

            foreach (Transform child in transform)
            {
                // make each of the object's children glow
                child.GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");

                // if the child object is a Door
                if (child.tag == "Door")
                {
                    // if the child object is active
                    if (child.gameObject.activeInHierarchy)
                    {
                        // deactivate the child object
                        child.gameObject.SetActive(false);
                    }

                    // if the child object is inactive
                    else
                    {
                        // activate the child object
                        child.gameObject.SetActive(true);
                    }
                }
            }
        }
    }
}
