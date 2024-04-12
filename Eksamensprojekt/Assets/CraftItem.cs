using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftItem : MonoBehaviour
{

    public WeaponSO woodSwordSO;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && PlayerIsInTrigger)
        {
            CraftItems(woodSwordSO);
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

    public void CraftItems(WeaponSO weaponSO)
    {
        string materialAmountKey = weaponSO.requiredItem.itemName + "Amount";
        int amount = PlayerPrefs.GetInt(materialAmountKey, 0);
        int price = weaponSO.price;

        if (amount >= price)
        {
            PlayerPrefs.SetInt(materialAmountKey, amount - price);
            PlayerPrefsKeysManager.RegisterKey(materialAmountKey);  // Ensure 'Wood' is updated

            int craftedItemCount = PlayerPrefs.GetInt(weaponSO.weaponName + "Amount");
            PlayerPrefs.SetInt(weaponSO.weaponName + "Amount", craftedItemCount + 1);  // Add 1 WoodSword to inventory
            PlayerPrefs.Save();
            PlayerPrefsKeysManager.RegisterKey(weaponSO.weaponName + "Amount"); // Register 'WoodSword'
            Debug.Log("New " + weaponSO.weaponName + " count: " + PlayerPrefs.GetInt(weaponSO.weaponName + "Amount"));
        }
        else
        {
            Debug.Log("Not enough " + weaponSO.requiredItem.itemName + " to craft a " + weaponSO.weaponName + ". You need at least " + price);
        }
    }
}
