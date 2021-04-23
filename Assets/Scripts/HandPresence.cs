using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HandPresence : MonoBehaviour
{
    public InputDeviceCharacteristics controllerCharacteristics;
    public GameObject handModelPrefab;
    public InputDevice handDevice;

    private InputDevice handDeviceTarget;
    private GameObject spawnedHandModel;
    private Animator handAnimator;

    // Start is called before the first frame update
    void Start()
    {
        TryInitialize();
    }

    /// <summary>
    /// Called when the devices are trying to initialize
    /// </summary>
    void TryInitialize()
    {
        // get the Left and Right Hand Devices
        List<InputDevice> handDevice = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, handDevice);

        foreach (var item in handDevice)
        {
            Debug.Log(item.name + item.characteristics);
        }

        // if one or more Hand Devices exist
        if (handDevice.Count > 0)
        {
            // set the first Hand Device to the Target
            handDeviceTarget = handDevice[0];

            // add the Hand model
            spawnedHandModel = Instantiate(handModelPrefab, transform);
            handAnimator = spawnedHandModel.GetComponent<Animator>();
        }
    }

    /// <summary>
    /// Called every frame
    /// </summary>
    void UpdateHandAnimation()
    {
        // if the Trigger is pulled
        if (handDeviceTarget.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
        {
            // update the hand animation
            handAnimator.SetFloat("Trigger", triggerValue);
        }

        // if the Trigger is not being pulled
        else
        {
            // update the hand animation
            handAnimator.SetFloat("Trigger", 0);
        }

        // if the Grip is being held
        if (handDeviceTarget.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
        {
            // update the hand animation
            handAnimator.SetFloat("Grip", gripValue);
        }

        // if the Grip is not being held
        else
        {
            // update the hand animation
            handAnimator.SetFloat("Grip", 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // if the Hand Device was found
        if (handDeviceTarget != null)
        {
            // set the Hand Device to the Target
            handDevice = handDeviceTarget;
        }

        // if the Hand Device Target is invalid
        if (!handDeviceTarget.isValid)
        {
            // Try to initialize once more
            TryInitialize();
        }

        // Update the hand animation every frame
        else
        {
            UpdateHandAnimation();
        }
    }
}
