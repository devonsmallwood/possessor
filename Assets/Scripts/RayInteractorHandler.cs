using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.SceneManagement;

public class RayInteractorHandler : MonoBehaviour
{
    public List<GameObject> redPossessableObjects;

    public bool lRayActive = false;
    public bool rRayActive = false;
    public Transform camTransform;
    public GameObject lHandPresence;
    public GameObject rHandPresence;
    public GameObject lRayGO;
    public GameObject rRayGO;
    public GameObject pauseMenu;
    public SFX sfx;
    public float waitTime;
    public Animator musicAnim;

    private Gradient blueGradient;
    private GradientColorKey[] blueColorKey;
    private GradientAlphaKey[] blueAlphaKey;

    private Gradient redGradient;
    private GradientColorKey[] redColorKey;
    private GradientAlphaKey[] redAlphaKey;

    private Gradient whiteGradient;
    private GradientColorKey[] whiteColorKey;
    private GradientAlphaKey[] whiteAlphaKey;

    private string leftTargetColor;
    private string rightTargetColor;
    private GameObject lTarget;
    private GameObject rTarget;
    private GameObject lLastPossessed;
    private GameObject rLastPossessed;

    private bool menuButtonReleased = true;
    private Vector3 movement;

    // Start is called before the first frame update
    void Start()
    {
        blueGradient = new Gradient();
        blueColorKey = new GradientColorKey[2];
        blueColorKey[0].color = Color.blue;
        blueColorKey[0].time = 0.0f;
        blueColorKey[1].color = Color.blue;
        blueColorKey[1].time = 1.0f;
        blueAlphaKey = new GradientAlphaKey[2];
        blueAlphaKey[0].alpha = 0.0f;
        blueAlphaKey[0].time = 0.0f;
        blueAlphaKey[1].alpha = 0.5f;
        blueAlphaKey[1].time = 1.0f;
        blueGradient.SetKeys(blueColorKey, blueAlphaKey);

        redGradient = new Gradient();
        redColorKey = new GradientColorKey[2];
        redColorKey[0].color = Color.red;
        redColorKey[0].time = 0.0f;
        redColorKey[1].color = Color.red;
        redColorKey[1].time = 1.0f;
        redAlphaKey = new GradientAlphaKey[2];
        redAlphaKey[0].alpha = 0.0f;
        redAlphaKey[0].time = 0.0f;
        redAlphaKey[1].alpha = 0.5f;
        redAlphaKey[1].time = 1.0f;
        redGradient.SetKeys(redColorKey, redAlphaKey);

        whiteGradient = new Gradient();
        whiteColorKey = new GradientColorKey[2];
        whiteColorKey[0].color = Color.white;
        whiteColorKey[0].time = 0.0f;
        whiteColorKey[1].color = Color.white;
        whiteColorKey[1].time = 1.0f;
        whiteAlphaKey = new GradientAlphaKey[2];
        whiteAlphaKey[0].alpha = 0.0f;
        whiteAlphaKey[0].time = 0.0f;
        whiteAlphaKey[1].alpha = 0.5f;
        whiteAlphaKey[1].time = 1.0f;
        whiteGradient.SetKeys(whiteColorKey, whiteAlphaKey);

        // get all Red Possessable objects
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Red Possessable"))
        {
            redPossessableObjects.Add(go);
        }
    }

    /// <summary>
    /// Checks the color of the Target that is being hit by the Left Hand Device's laser
    /// </summary>
    public void CheckLeftTargetColor()
    {
        // get the object hit by the Left Hand Device's laser
        lRayGO.GetComponent<XRRayInteractor>().GetCurrentRaycastHit(out RaycastHit raycastHit);

        Debug.Log("Left Object Tag: " + raycastHit.collider.gameObject.tag);

        // if the object is a Blue Possessable
        if (raycastHit.collider.gameObject.tag == "Blue Possessable")
        {
            // set the target and highlight it
            leftTargetColor = "blue";
            lTarget = raycastHit.collider.gameObject;
            lTarget.GetComponent<BluePossessable>().isHighlighted = true;
        }

        // if the object is a Red Possessable
        else if (raycastHit.collider.gameObject.tag == "Red Possessable")
        {
            // set the target and highlight it
            leftTargetColor = "red";
            lTarget = raycastHit.collider.gameObject;
            lTarget.GetComponent<RedPossessable>().isHighlighted = true;
        }
    }

    /// <summary>
    /// Checks the color of the Target that is being hit by the Right Hand Device's laser
    /// </summary>
    public void CheckRightTargetColor()
    {
        // get the object hit by the Right Hand Device's laser
        rRayGO.GetComponent<XRRayInteractor>().GetCurrentRaycastHit(out RaycastHit raycastHit);

        Debug.Log("Right Object Tag: " + raycastHit.collider.gameObject.tag);

        // if the object is a Blue Possessable
        if (raycastHit.collider.gameObject.tag == "Blue Possessable")
        {
            // set the target and highlight it
            rightTargetColor = "blue";
            rTarget = raycastHit.collider.gameObject;
            rTarget.GetComponent<BluePossessable>().isHighlighted = true;
        }

        // if the object is a Red Possessable
        else if (raycastHit.collider.gameObject.tag == "Red Possessable")
        {
            // set the target and highlight it
            rightTargetColor = "red";
            rTarget = raycastHit.collider.gameObject;
            rTarget.GetComponent<RedPossessable>().isHighlighted = true;
        }
    }

    /// <summary>
    /// The movement code for Red Possessable objects
    /// </summary>
    private void FixedUpdate()
    {
        foreach (GameObject go in redPossessableObjects)
        {
            // if a Red Possessable object is possessed
            if (go.GetComponent<RedPossessable>().isPossessed)
            {
                // if the object is being possessed by the Left Hand Device
                if (go.GetComponent<RedPossessable>().hand == RedPossessable.Hand.Left)
                {
                    switch (go.GetComponent<RedPossessable>().manipulationType)
                    {
                        // if the object's type is Ball
                        case RedPossessable.ManipulationType.Ball:

                            // if the Left Hand Device exists
                            if (lHandPresence.GetComponent<HandPresence>().handDevice != null)
                            {
                                // get the value of the Left Thumbstick
                                lHandPresence.GetComponent<HandPresence>().handDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 primary2DAxisValue);

                                float moveHorizontal = primary2DAxisValue.x;
                                float moveVertical = primary2DAxisValue.y;

                                movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

                                // rotate with camera
                                movement = RotateWithView();

                                // Move the object by adding Force in the direction of the Left Thumbstick
                                go.GetComponent<Rigidbody>().AddForce(movement * go.GetComponent<RedPossessable>().ballSpeed * Time.deltaTime);
                            }

                            break;

                        // if the object's type is Translatable
                        case RedPossessable.ManipulationType.Translatable:

                            // if the Left Hand Device exists
                            if (lHandPresence.GetComponent<HandPresence>().handDevice != null)
                            {
                                // get the value of the Left Thumbstick
                                lHandPresence.GetComponent<HandPresence>().handDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 primary2DAxisValue);

                                float moveHorizontal = primary2DAxisValue.x;
                                float moveVertical = primary2DAxisValue.y;

                                // Get the current position of the object
                                float currentXPosition = go.transform.position.x;
                                float currentYPosition = go.transform.position.y;
                                float currentZPosition = go.transform.position.z;

                                // if X axis movement is allowed for this object
                                if (go.GetComponent<RedPossessable>().translateX)
                                {
                                    // if the Left Thumbstick is not being moved along the X axis
                                    if (primary2DAxisValue.x == 0)
                                    {
                                        // keep the object's current position
                                        go.transform.position = new Vector3(currentXPosition, currentYPosition, currentZPosition);
                                    }

                                    // if the Left Thumbstick is being moved along the X axis
                                    else
                                    {
                                        // if the object is between the Minimum and Maximum X values
                                        if (go.transform.position.x <= go.GetComponent<RedPossessable>().maxX && go.transform.position.x >= go.GetComponent<RedPossessable>().minX)
                                        {
                                            // move the object along the X axis
                                            go.transform.position = new Vector3(currentXPosition += moveHorizontal * go.GetComponent<RedPossessable>().translationSpeed, currentYPosition, currentZPosition);
                                        }

                                        // if the object goes above the Maximum X value
                                        if (go.transform.position.x > go.GetComponent<RedPossessable>().maxX)
                                        {
                                            // set the object's X position to the Maximum X value
                                            go.transform.position = new Vector3(go.GetComponent<RedPossessable>().maxX, currentYPosition, currentZPosition);
                                        }

                                        // if the object goes below the Minimum X value
                                        if (go.transform.position.x < go.GetComponent<RedPossessable>().minX)
                                        {
                                            // set the object's X position to the Minimum X value
                                            go.transform.position = new Vector3(go.GetComponent<RedPossessable>().minX, currentYPosition, currentZPosition);
                                        }
                                    }
                                }

                                // if Y axis movement is allowed for this object
                                if (go.GetComponent<RedPossessable>().translateY)
                                {
                                    // if the Left Thumbstick is not being moved along the Y axis
                                    if (primary2DAxisValue.y == 0)
                                    {
                                        // keep the object's current position
                                        go.transform.position = new Vector3(currentXPosition, currentYPosition, currentZPosition);
                                    }

                                    else
                                    {
                                        // if the object is between the Minimum and Maximum Y values
                                        if (go.transform.position.y <= go.GetComponent<RedPossessable>().maxY && go.transform.position.y >= go.GetComponent<RedPossessable>().minY)
                                        {
                                            // move the object along the Y axis
                                            go.transform.position = new Vector3(currentXPosition, currentYPosition += moveVertical * go.GetComponent<RedPossessable>().translationSpeed, currentZPosition);
                                        }

                                        // if the object goes above the Maximum Y value
                                        if (go.transform.position.y > go.GetComponent<RedPossessable>().maxY)
                                        {
                                            // set the object's Y position to the Maximum Y value
                                            go.transform.position = new Vector3(currentXPosition, go.GetComponent<RedPossessable>().maxY, currentZPosition);
                                        }

                                        // if the object goes below the Minimum Y value
                                        if (go.transform.position.y < go.GetComponent<RedPossessable>().minY)
                                        {
                                            // set the object's Y position to the Minimum Y value
                                            go.transform.position = new Vector3(currentXPosition, go.GetComponent<RedPossessable>().minY, currentZPosition);
                                        }
                                    }
                                }

                                // if Z axis movement is allowed for this object
                                if (go.GetComponent<RedPossessable>().translateZ)
                                {
                                    // if the Left Thumbstick is not being moved along the Y axis
                                    if (primary2DAxisValue.y == 0)
                                    {
                                        // keep the object's current Z position
                                        go.transform.position = new Vector3(currentXPosition, currentYPosition, currentZPosition);
                                    }

                                    else
                                    {
                                        // if the object is between the Maximum and Minimum Z values
                                        if (go.transform.position.z <= go.GetComponent<RedPossessable>().maxZ && go.transform.position.z >= go.GetComponent<RedPossessable>().minZ)
                                        {
                                            // move the object along the Z axis
                                            go.transform.position = new Vector3(currentXPosition, currentYPosition, currentZPosition += moveVertical * go.GetComponent<RedPossessable>().translationSpeed);
                                        }

                                        // if the object is above the Maximum Z value
                                        if (go.transform.position.z > go.GetComponent<RedPossessable>().maxZ)
                                        {
                                            // set the object's Z position to the Maximum Z value
                                            go.transform.position = new Vector3(currentXPosition, currentYPosition, go.GetComponent<RedPossessable>().maxZ);
                                        }

                                        // if the object is below the Minimum Z value
                                        if (go.transform.position.z < go.GetComponent<RedPossessable>().minZ)
                                        {
                                            // set the object's Z position to the Minimum Z value
                                            go.transform.position = new Vector3(currentXPosition, currentYPosition, go.GetComponent<RedPossessable>().minZ);
                                        }
                                    }
                                }
                            }

                            break;

                        // if the object's type is Rotatable
                        case RedPossessable.ManipulationType.Rotatable:
                            break;

                        // if the object's type is Resizable
                        case RedPossessable.ManipulationType.Resizable:
                            break;
                    }
                }

                // if the object is being possessed by the Right Hand Device
                if (go.GetComponent<RedPossessable>().hand == RedPossessable.Hand.Right)
                {
                    switch (go.GetComponent<RedPossessable>().manipulationType)
                    {
                        // if the object's type is Ball
                        case RedPossessable.ManipulationType.Ball:

                            // if the Right Hand Device exists
                            if (rHandPresence.GetComponent<HandPresence>().handDevice != null)
                            {
                                // get the value of the Right Thumbstick
                                rHandPresence.GetComponent<HandPresence>().handDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 primary2DAxisValue);

                                float moveHorizontal = primary2DAxisValue.x;
                                float moveVertical = primary2DAxisValue.y;

                                movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

                                // rotate with camera
                                movement = RotateWithView();

                                // Move the object by adding Force in the direction of the Right Thumbstick
                                go.GetComponent<Rigidbody>().AddForce(movement * go.GetComponent<RedPossessable>().ballSpeed * Time.deltaTime);
                            }

                            break;

                        // if the object's type is Translatable
                        case RedPossessable.ManipulationType.Translatable:

                            // if the Right Hand Device exists
                            if (rHandPresence.GetComponent<HandPresence>().handDevice != null)
                            {
                                // get the value of the Right Thumbstick
                                rHandPresence.GetComponent<HandPresence>().handDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 primary2DAxisValue);

                                float moveHorizontal = primary2DAxisValue.x;
                                float moveVertical = primary2DAxisValue.y;

                                // get the object's current position
                                float currentXPosition = go.transform.position.x;
                                float currentYPosition = go.transform.position.y;
                                float currentZPosition = go.transform.position.z;

                                // if X axis movement is allowed for this object
                                if (go.GetComponent<RedPossessable>().translateX)
                                {
                                    // if the Right Thumbstick is not being moved along the X axis
                                    if (primary2DAxisValue.x == 0)
                                    {
                                        // keep the object's current position
                                        go.transform.position = new Vector3(currentXPosition, currentYPosition, currentZPosition);
                                    }

                                    else
                                    {
                                        // if the object is between the Minimum and Maximum X values
                                        if (go.transform.position.x <= go.GetComponent<RedPossessable>().maxX && go.transform.position.x >= go.GetComponent<RedPossessable>().minX)
                                        {
                                            // move the object along the X axis
                                            go.transform.position = new Vector3(currentXPosition += moveHorizontal * go.GetComponent<RedPossessable>().translationSpeed, currentYPosition, currentZPosition);
                                        }

                                        // if the object is above the Maximum X value
                                        if (go.transform.position.x > go.GetComponent<RedPossessable>().maxX)
                                        {
                                            // set the object's X position to the Maximum X value
                                            go.transform.position = new Vector3(go.GetComponent<RedPossessable>().maxX, currentYPosition, currentZPosition);
                                        }

                                        // if the object is below the Minimum X value
                                        if (go.transform.position.x < go.GetComponent<RedPossessable>().minX)
                                        {
                                            // set the object's X position to the Minimum X value
                                            go.transform.position = new Vector3(go.GetComponent<RedPossessable>().minX, currentYPosition, currentZPosition);
                                        }
                                    }
                                }

                                // if Y axis movement is allowed for this object
                                if (go.GetComponent<RedPossessable>().translateY)
                                {
                                    // if the Right Thumbstick is not being moved along the Y axis
                                    if (primary2DAxisValue.y == 0)
                                    {
                                        // keep the object's current position
                                        go.transform.position = new Vector3(currentXPosition, currentYPosition, currentZPosition);
                                    }

                                    else
                                    {
                                        // if the object is between the Maximum and Minimum Y values
                                        if (go.transform.position.y <= go.GetComponent<RedPossessable>().maxY && go.transform.position.y >= go.GetComponent<RedPossessable>().minY)
                                        {
                                            // move the object along the Y axis
                                            go.transform.position = new Vector3(currentXPosition, currentYPosition += moveVertical * go.GetComponent<RedPossessable>().translationSpeed, currentZPosition);
                                        }

                                        // if the object is above the Maximum Y value
                                        if (go.transform.position.y > go.GetComponent<RedPossessable>().maxY)
                                        {
                                            // set the object's Y position to the Maximum Y value
                                            go.transform.position = new Vector3(currentXPosition, go.GetComponent<RedPossessable>().maxY, currentZPosition);
                                        }

                                        // if the object is below the Minimum Y value
                                        if (go.transform.position.y < go.GetComponent<RedPossessable>().minY)
                                        {
                                            // set the object's Y position to the Minimum Y value
                                            go.transform.position = new Vector3(currentXPosition, go.GetComponent<RedPossessable>().minY, currentZPosition);
                                        }
                                    }
                                }

                                // if Z translation is allowed for this object
                                if (go.GetComponent<RedPossessable>().translateZ)
                                {
                                    // if the Left Thumbstick is not being moved along the Y axis
                                    if (primary2DAxisValue.y == 0)
                                    {
                                        // keep the object's current position
                                        go.transform.position = new Vector3(currentXPosition, currentYPosition, currentZPosition);
                                    }

                                    else
                                    {
                                        // if the object is between the Maximum and Minimum Z values
                                        if (go.transform.position.z <= go.GetComponent<RedPossessable>().maxZ && go.transform.position.z >= go.GetComponent<RedPossessable>().minZ)
                                        {
                                            // move the object along the Z axis
                                            go.transform.position = new Vector3(currentXPosition, currentYPosition, currentZPosition += moveVertical * go.GetComponent<RedPossessable>().translationSpeed);
                                        }

                                        // if the object is above the Maximum Z value
                                        if (go.transform.position.z > go.GetComponent<RedPossessable>().maxZ)
                                        {
                                            // set the object's Z position to the Maximum Z value
                                            go.transform.position = new Vector3(currentXPosition, currentYPosition, go.GetComponent<RedPossessable>().maxZ);
                                        }

                                        // if the object is below the Minimum Z value
                                        if (go.transform.position.z < go.GetComponent<RedPossessable>().minZ)
                                        {
                                            // set the object's posiiton to the Minimum Z value
                                            go.transform.position = new Vector3(currentXPosition, currentYPosition, go.GetComponent<RedPossessable>().minZ);
                                        }
                                    }
                                }
                            }

                            break;

                        // if the object's type is Rotatable
                        case RedPossessable.ManipulationType.Rotatable:
                            break;

                        // if the object's type is Resizable
                        case RedPossessable.ManipulationType.Resizable:
                            break;
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (GameObject go in redPossessableObjects)
        {
            // if a Red Possessable falls off the level
            if (go.transform.position.y <= -1)
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
            }
        }

        // get the object hit by the Left Hand Device's laser
        if (lRayGO.GetComponent<XRRayInteractor>().GetCurrentRaycastHit(out RaycastHit lRaycastHit))
        {
            // if the object is not the current Target of the Left Hand Device
            if (lRaycastHit.collider.gameObject != lTarget)
            {
                // if the Left Hand Device has a Target and the Target is a Blue Possessable
                if (lTarget && lTarget.tag == "Blue Possessable")
                {
                    // stop highlighting the Left Hand Device's Target
                    lTarget.GetComponent<BluePossessable>().isHighlighted = false;
                }

                // if the Left Hand Device has a Target and the Target is a Red Possessable
                else if (lTarget && lTarget.tag == "Red Possessable")
                {
                    // stop highlighting the Left Hand Device's Target
                    lTarget.GetComponent<RedPossessable>().isHighlighted = false;
                }
            }
        }

        // get the object hit by the Right Hand Device's laser
        if (rRayGO.GetComponent<XRRayInteractor>().GetCurrentRaycastHit(out RaycastHit rRaycastHit))
        {
            // if the object is not the current Target of the Right Hand Device
            if (rRaycastHit.collider.gameObject != rTarget)
            {
                // if the Right Hand Device has a Target and the Target is a Blue Possessable
                if (rTarget && rTarget.tag == "Blue Possessable")
                {
                    // stop highlighting the Right Hand Device's Target
                    rTarget.GetComponent<BluePossessable>().isHighlighted = false;
                }

                // if the Right Hand Device has a Target and the Target is a Red Possessable
                else if (rTarget && rTarget.tag == "Red Possessable")
                {
                    // stop highlighting the Right Hand Device's Target
                    rTarget.GetComponent<RedPossessable>().isHighlighted = false;
                }
            }
        }

        // if the Left Hand Device exists
        if (lHandPresence.GetComponent<HandPresence>().handDevice != null)
        {
            // get the value of the Left Trigger
            lHandPresence.GetComponent<HandPresence>().handDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue);
            
            // if the Left Trigger is being pulled
            if (triggerValue > 0.1f)
            {
                // activate the Left Hand laser
                lRayActive = true;
                lRayGO.SetActive(true);
            }

            else
            {
                // if the Left Trigger is not being pulled and the Left Hand Device has a Target
                if (lTarget)
                {
                    // if the Left Hand Target is a Blue Possessable
                    if (lTarget.tag == "Blue Possessable")
                    {
                        // stop highlighting the Left Hand Target
                        lTarget.GetComponent<BluePossessable>().isHighlighted = false;
                    }

                    // if the Left Hand Target is a Red Possessable
                    else if (lTarget.tag == "Red Possessable")
                    {
                        // stop highlighting the Left Hand Target
                        lTarget.GetComponent<RedPossessable>().isHighlighted = false;
                    }
                }

                // deactivate the Left Hand Laser
                lRayActive = false;
                lRayGO.GetComponent<XRInteractorLineVisual>().validColorGradient = whiteGradient;
                lRayGO.SetActive(false);
            }

            // if the Pause Menu exists
            if (pauseMenu)
            {
                // get the value of the Left Hand Device's Menu Button
                lHandPresence.GetComponent<HandPresence>().handDevice.IsPressed(InputHelpers.Button.MenuButton, out bool menuButtonPressed);

                // if the Left Hand Device's Menu Button has been pressed
                if (menuButtonPressed)
                {
                    // the Left Hand Device's Menu Button has not been released
                    menuButtonReleased = false;
                }

                // if the Left Hand Device's Menu Button has not been released and has not been pressed
                if (!menuButtonReleased && !menuButtonPressed)
                {
                    // if the Pause Menu is active
                    if (!pauseMenu.activeInHierarchy)
                    {
                        // deactivate the Pause Menu
                        pauseMenu.SetActive(true);
                    }

                    // if the Pause Menu is not active
                    else
                    {
                        // activate the Pause Menu
                        pauseMenu.SetActive(false);
                    }

                    // the Pause Menu Button has been released
                    menuButtonReleased = true;
                }
            }

            // if there is no Pause Menu and the current Scene is LevelOne
            else if (SceneManager.GetActiveScene().name == "LevelOne")
            {
                // get the value of the Left Hand Device's Menu Button
                lHandPresence.GetComponent<HandPresence>().handDevice.IsPressed(InputHelpers.Button.MenuButton, out bool menuButtonPressed);

                // if the Left Hand Device's Menu Button has been pressed
                if (menuButtonPressed)
                {
                    // the Left Hand Device's Menu Button has not been released
                    menuButtonReleased = false;
                }

                // if the Left Hand Device's Menu Button has not been released and has not been pressed
                if (!menuButtonReleased && !menuButtonPressed)
                {
                    // load Main Menu
                    StartCoroutine(ChangeScene());

                    // the Pause Menu Button has been released
                    menuButtonReleased = true;
                }
            }
        }

        // if the Right Hand Device exists
        if (rHandPresence.GetComponent<HandPresence>().handDevice != null)
        {
            // get the value of the Right Trigger
            rHandPresence.GetComponent<HandPresence>().handDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue);

            // if the Right Trigger is being pulled
            if (triggerValue > 0.1f)
            {
                // activate the Right Hand Laser
                rRayActive = true;
                rRayGO.SetActive(true);
            }

            else
            {
                // if the Right Trigger is not being pulled and the Right Hand Device has a Target
                if (rTarget)
                {
                    // if the Right Hand Target is a Blue Possessable
                    if (rTarget.tag == "Blue Possessable")
                    {
                        // stop highlighting the Right Hand Target
                        rTarget.GetComponent<BluePossessable>().isHighlighted = false;
                    }

                    // if the Right Hand Target is a Red Possessable
                    else if (rTarget.tag == "Red Possessable")
                    {
                        // stop highlighting the Right Hand Target
                        rTarget.GetComponent<RedPossessable>().isHighlighted = false;
                    }
                }

                // deactivate the Right Hand laser
                rRayActive = false;
                rRayGO.GetComponent<XRInteractorLineVisual>().validColorGradient = whiteGradient;
                rRayGO.SetActive(false);
            }
        }

        // if the Left Hand laser is active
        if (lRayActive)
        {
            switch (leftTargetColor)
            {
                // if the Target is Blue
                case "blue":
                    // make the laser blue
                    lRayGO.GetComponent<XRInteractorLineVisual>().validColorGradient = blueGradient;
                    break;

                // if the Target is Red
                case "red":
                    // make the laser red
                    lRayGO.GetComponent<XRInteractorLineVisual>().validColorGradient = redGradient;
                    break;

                // make the laser white by default
                default:
                    lRayGO.GetComponent<XRInteractorLineVisual>().validColorGradient = whiteGradient;
                    break;
            }
        }

        // if the Right Hand laser is active
        if (rRayActive)
        {
            switch (rightTargetColor)
            {
                // if the Target is Blue
                case "blue":
                    // make the laser blue
                    rRayGO.GetComponent<XRInteractorLineVisual>().validColorGradient = blueGradient;
                    break;

                // if the Target is Red
                case "red":
                    // make the laser red
                    rRayGO.GetComponent<XRInteractorLineVisual>().validColorGradient = redGradient;
                    break;

                // make the laser white by default
                default:
                    rRayGO.GetComponent<XRInteractorLineVisual>().validColorGradient = whiteGradient;
                    break;
            }
        }
    }

    /// <summary>
    /// Called when the Left Hand Device possesses an object
    /// </summary>
    public void LeftPossessObject()
    {
        // play possession sound
        sfx.Possess();

        // if the Left Hand Device has possessed an object
        if (lLastPossessed)
        {
            // if the current Target is a Red Possessable
            if (lTarget.GetComponent<RedPossessable>())
            {
                // if there is already a Red Possessable possessed by the Left Hand Device, unpossess it
                foreach (GameObject go in redPossessableObjects)
                {
                    if (go.GetComponent<RedPossessable>().isPossessed && go.GetComponent<RedPossessable>().hand == RedPossessable.Hand.Left)
                    {
                        go.GetComponent<RedPossessable>().Unpossess();
                    }
                }
            }
        }

        // Set the Left Hand Device's previously possessed object to the Target
        lLastPossessed = lTarget;

        Debug.Log("Left Target Possessed");

        // if the Left Hand Target is Red
        if (leftTargetColor == "red")
        {
            // Possess the Left Hand Target
            lTarget.GetComponent<RedPossessable>().Possess();
            lTarget.GetComponent<RedPossessable>().hand = RedPossessable.Hand.Left;
        }

        // if the Left Hand Target is Blue
        if (leftTargetColor == "blue")
        {
            // Possess the Left Hand Target
            lTarget.GetComponent<BluePossessable>().Possess();
        }

        // if the Left Hand Device's possessed object and the Right Hand Device's previously possessed object are the same object
        if (lLastPossessed == rLastPossessed)
        {
            // force the Right Hand Device to unpossess the object
            rLastPossessed = null;
        }
    }

    /// <summary>
    /// Called when the Right Hand Device possesses an object
    /// </summary>
    public void RightPossessObject()
    {
        // play possession sound
        sfx.Possess();

        // if the Right Hand Device has possessed an object
        if (rLastPossessed)
        {
            // if the current Target is a Red Possessable
            if (rTarget.GetComponent<RedPossessable>())
            {
                // if there is already a Red Possessable possessed by the Right Hand Device, unpossess it
                foreach (GameObject go in redPossessableObjects)
                {
                    if (go.GetComponent<RedPossessable>().isPossessed && go.GetComponent<RedPossessable>().hand == RedPossessable.Hand.Right)
                    {
                        go.GetComponent<RedPossessable>().Unpossess();
                    }
                }
            }
        }

        // Set the Right Hand's previously possessed object to the Target
        rLastPossessed = rTarget;

        Debug.Log("Right Target Possessed");

        // if the Right Hand Target is Red
        if (rightTargetColor == "red")
        {
            // Possess the Right Hand Target
            rTarget.GetComponent<RedPossessable>().Possess();
            rTarget.GetComponent<RedPossessable>().hand = RedPossessable.Hand.Right;
        }

        // if the Right Hand Target is Blue
        if (rightTargetColor == "blue")
        {
            // Possess the Right Hand Target
            rTarget.GetComponent<BluePossessable>().Possess();
        }

        // if the Right Hand Device's possessed object and the Left Hand Device's previously possessed object are the same object
        if (rLastPossessed == lLastPossessed)
        {
            // force the Left Hand Device to unpossess the object
            lLastPossessed = null;
        }
    }

    /// <summary>
    /// Rotates the Ball with the camera
    /// </summary>
    /// <returns></returns>
    public Vector3 RotateWithView()
    {
        if (camTransform != null)
        {
            Vector3 dir = camTransform.TransformDirection(movement);
            dir.Set(dir.x, 0, dir.z);
            return dir.normalized * movement.magnitude;
        }

        else
        {
            camTransform = Camera.main.transform;
            return movement;
        }
    }

    IEnumerator ChangeScene()
    {
        // wait for the music to fade out, then transition to the next Scene
        musicAnim.SetTrigger("fadeOut");
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene("MainMenu");
    }
}
