using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckGatheredItems : MonoBehaviour
{
    public string[] itemNames;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && PlayerIsInTrigger)
        {
            foreach (string key in PlayerPrefsKeysManager.GetAllKeys())
            {
                int value = PlayerPrefs.GetInt(key, 0);  // Default to 0 if key not found
                Debug.Log(key + ": " + value);
            }
        }
    }


    private bool PlayerIsInTrigger = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerIsInTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerIsInTrigger = false;
        }
    }
}
