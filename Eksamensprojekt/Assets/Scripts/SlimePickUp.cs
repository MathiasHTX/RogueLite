using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using static UnityEditor.Progress;

public class SlimePickUp : MonoBehaviour
{
    AudioSource audioSource;
    bool pickedUp;
    public ItemSO slimePickUpSO;

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
                PlayerPrefs.SetInt(slimePickUpSO.itemName + "Amount", PlayerPrefs.GetInt(slimePickUpSO.itemName + "Amount") + 1);
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
