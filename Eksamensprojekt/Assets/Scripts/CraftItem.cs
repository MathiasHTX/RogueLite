using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftItem : MonoBehaviour
{

    void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.E) && PlayerIsInTrigger)
        {
            CraftItems(woodSwordSO);
        }
        */
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

    public void CraftItems(WeaponSO weaponSO)
    {
        for (int i = 0; i < weaponSO.requiredItems.Length; i++)
        {
            string itemAmountKey = weaponSO.requiredItems[i].itemName + "Amount";
            int amount = PlayerPrefs.GetInt(itemAmountKey, 0);
            int requiredAmount = weaponSO.requiredItemAmounts[i];

            PlayerPrefs.SetInt(itemAmountKey, amount - requiredAmount);
            PlayerPrefsKeysManager.RegisterKey(itemAmountKey);
        }

        int craftedItemCount = PlayerPrefs.GetInt(weaponSO.weaponName + "Amount");
        PlayerPrefs.SetInt(weaponSO.weaponName + "Amount", craftedItemCount + 1);  // Add 1 crafted item to inventory
        PlayerPrefs.Save();
        PlayerPrefsKeysManager.RegisterKey(weaponSO.weaponName + "Amount"); // Register crafted item;
        Debug.Log("New " + weaponSO.weaponName + " count: " + PlayerPrefs.GetInt(weaponSO.weaponName + "Amount"));
    }
}
