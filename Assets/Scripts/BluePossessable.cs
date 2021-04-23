using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BluePossessable : MonoBehaviour
{
    public GameObject player;
    public bool possessOnStart = false;
    public bool isHighlighted = false;
    public bool isPossessed = false;
    public bool isSavepoint = false;
    public List<GameObject> despawnInPrevRoom;
    public List<GameObject> spawnInPrevRoom;
    public GameObject[] bluePossessables;

    private SavepointSelect savepointSelect;

    private void Start()
    {
        // find Savepoint Selector
        savepointSelect = GameObject.Find("SavepointSelect").GetComponent<SavepointSelect>();

        // find all Blue Possessables
        bluePossessables = GameObject.FindGameObjectsWithTag("Blue Possessable");
        Debug.Log(bluePossessables.Length + " Blue Possessables Detected in Scene");
        
        // if the object is set to be possessed on Start, possess it immediately
        if (possessOnStart)
        {
            Possess();
        }
    }

    /// <summary>
    /// Called when the object is possessed
    /// </summary>
    public void Possess()
    {
        isPossessed = true;

        Debug.Log("Blue Object Possessed");
        // move and rotate the player to match the position of the object
        player.transform.position = this.transform.position;
        player.transform.rotation = this.transform.rotation;

        // disable the object so it does not obstruct the player's view
        this.gameObject.SetActive(false);

        foreach (GameObject go in bluePossessables)
        {
            // unpossess all objects not currently being possessed
            if (go != this.gameObject)
            {
                go.GetComponent<BluePossessable>().Unpossess();
            }
        }

        // if there are any objects in the last room that need to be despawned, deactivate them and unpossess them if necessary
        foreach (GameObject go in despawnInPrevRoom)
        {
            if (go.GetComponent<RedPossessable>())
            {
                go.GetComponent<RedPossessable>().Unpossess();
            }

            go.SetActive(false);
        }

        // if there are any objects in the last room that need to be spawned, spawn them
        foreach (GameObject go in spawnInPrevRoom)
        {
            go.SetActive(true);
        }
    }

    /// <summary>
    /// Called when the object is unpossessed
    /// </summary>
    public void Unpossess()
    {
        isPossessed = false;
        this.gameObject.SetActive(true);
    }

    private void Update()
    {
        // if the object is highlighted
        if (isHighlighted)
        {
            // make the object glow
            GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
        }

        // if the object is not highlighted
        else
        {
            // stop the object from glowing
            GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");
        }

        // if the object is selected as the Savepoint
        if (this == savepointSelect.currentSavepoint)
        {
            // possess the object on Start
            possessOnStart = true;
        }

        // if the object is not selected as the Savepoint
        else
        {
            // prevent the object from being possesses on Start
            possessOnStart = false;
        }
    }
}
