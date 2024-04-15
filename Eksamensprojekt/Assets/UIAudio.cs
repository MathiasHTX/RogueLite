using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAudio : MonoBehaviour
{
    public static UIAudio instance;

    [SerializeField] AudioClip openSound, closeSound;
    AudioSource audioSrc;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        audioSrc = GetComponent<AudioSource>();
    }

    public void PlayOpenSound()
    {
        audioSrc.PlayOneShot(openSound);
    }

    public void PlayCloseSound()
    {
        audioSrc.PlayOneShot(closeSound);
    }

}
