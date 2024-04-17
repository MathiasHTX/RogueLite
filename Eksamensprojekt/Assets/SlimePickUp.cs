using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SlimePickUp : MonoBehaviour
{
    AudioSource audioSource;
    bool pickedUp;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!pickedUp)
            {
                transform.DOScale(0, 0.2f);
                audioSource.Play();
                pickedUp = true;
                StartCoroutine(DeletePickUp());
            }
        }
    }

    IEnumerator DeletePickUp()
    {
        yield return new WaitForSeconds(1);
        Destroy(this.gameObject);
    }
}
