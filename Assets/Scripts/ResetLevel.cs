using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetLevel : MonoBehaviour
{
    public GameObject pauseMenu;

    private SavepointSelect savepointSelect;

    private void Start()
    {
        // find Savepoint Selector
        savepointSelect = GameObject.Find("SavepointSelect").GetComponent<SavepointSelect>();

        Debug.Log(savepointSelect);
    }

    public void ResetObjects()
    {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Red Possessable"))
        {
            // reset the position, scale, and rotation of each Red Possessable in the Scene, and unpossess them
            go.transform.position = go.GetComponent<RedPossessable>().initialPos;
            go.transform.localScale = go.GetComponent<RedPossessable>().initialScale;
            go.transform.rotation = Quaternion.Euler(go.GetComponent<RedPossessable>().initialRot.x, go.GetComponent<RedPossessable>().initialRot.y, go.GetComponent<RedPossessable>().initialRot.z);
            go.GetComponent<RedPossessable>().Unpossess();

            // stop the objects from moving
            if (go.GetComponent<Rigidbody>())
            {
                go.GetComponent<Rigidbody>().velocity = Vector3.zero;
                go.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            }

            // move the camera back to the Savepoint
            savepointSelect.currentSavepoint.Possess();
        }

        // close the Pause Menu
        pauseMenu.SetActive(false);
    }
}
