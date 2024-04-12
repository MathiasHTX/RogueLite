using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMusicLoop : MonoBehaviour
{
    public AudioSource loopingSoundtrack;

    void Start()
    {
        // Start the coroutine to delay audio playback
        StartCoroutine(DelayedPlay(10.126f));
    }

    IEnumerator DelayedPlay(float delay)
    {
        // Wait for the specified delay time
        yield return new WaitForSeconds(delay);

        // Play the audio if loopingSoundtrack is not null
        if (loopingSoundtrack != null)
        {
            loopingSoundtrack.Play();
        }
    }
}