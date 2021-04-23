using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavepointSelect : MonoBehaviour
{
    public BluePossessable currentSavepoint;
    public List<BluePossessable> cameras;
    public bool camerasAdded;

    // Start is called before the first frame update
    void Start()
    {
        // get all Blue Possessables
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Blue Possessable"))
        {
            cameras.Add(go.GetComponent<BluePossessable>());
            camerasAdded = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // if all Blue Possessables have been added to the list
        if (camerasAdded)
        {
            foreach (BluePossessable camera in cameras)
            {
                // if the object is possessed and is marked as a Savepoint
                if (camera.isPossessed && camera.isSavepoint)
                {
                    // set it to be possessed on Start and make it a Savepoint
                    camera.possessOnStart = true;
                    currentSavepoint = camera;
                }

                // if the object is not possessed / not marked as a Savepoint
                else
                {
                    // prevent it from being possessed on Start
                    camera.possessOnStart = false;
                }
            }
        }
    }
}
