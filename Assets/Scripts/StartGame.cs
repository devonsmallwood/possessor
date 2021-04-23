using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip audioClip;
    public float waitTime;
    public Animator musicAnim;

    public void StartLevel()
    {
        StartCoroutine(ChangeScene());
    }

    IEnumerator ChangeScene()
    {
        audioSource.PlayOneShot(audioClip);
        // wait for the music to fade out, then transition to the next Scene
        musicAnim.SetTrigger("fadeOut");
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene("LevelOne");
    }
}
