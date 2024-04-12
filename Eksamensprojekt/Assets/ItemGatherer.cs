using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGatherer : MonoBehaviour
{
    public ItemSO item;  // This should be the name of the item, set in the Unity Inspector
    public float GatherChance;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && PlayerIsInTrigger)
        {
            if (Random.value <= GatherChance)
            {
                PlayerPrefs.SetInt(item.itemName + "Amount", PlayerPrefs.GetInt(item.itemName + "Amount") + 1);
                PlayerPrefs.Save();
                PlayerPrefsKeysManager.RegisterKey(item.itemName + "Amount"); // Register the item key
                Debug.Log("Gathered Item");
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