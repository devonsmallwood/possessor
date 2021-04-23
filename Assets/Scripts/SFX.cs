using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX : MonoBehaviour
{
    public AudioClip possessSound;
    public AudioClip pressurePadDownSound;
    public AudioClip pressurePadUpSound;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
    }

    public void Possess()
    {
        // play the possess sound
        audioSource.PlayOneShot(possessSound);
    }

    public void PressurePadDown()
    {
        // play the pressure pad down sound
        audioSource.PlayOneShot(pressurePadDownSound);
    }

    public void PressurePadUp()
    {
        // play the pressure pad up sound
        audioSource.PlayOneShot(pressurePadUpSound);
    }
}
