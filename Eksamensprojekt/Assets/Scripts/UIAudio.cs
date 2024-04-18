using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAudio : MonoBehaviour
{
    [SerializeField] AudioClip openSound, closeSound, clickSound, craftingSound, eatMushroomSound;
    AudioSource audioSrc;

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

    public void PlayClickSound()
    {
        audioSrc.PlayOneShot(clickSound);
    }

    public void PlayCraftingSound()
    {
        audioSrc.PlayOneShot(craftingSound);
    }

    public void PlayEatMushroomSound()
    {
        audioSrc.PlayOneShot(eatMushroomSound);
    }
}
