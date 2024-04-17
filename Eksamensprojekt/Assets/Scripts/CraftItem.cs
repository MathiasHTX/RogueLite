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

    public void CraftWeapon(WeaponSO weaponSO)
    {
        for (int i = 0; i < weaponSO.requiredItems.Length; i++)
        {
            string itemAmountKey = weaponSO.requiredItems[i].itemName + "Amount";
            int amount = PlayerPrefs.GetInt(itemAmountKey, 0);
            int requiredAmount = weaponSO.requiredItemAmounts[i];

            PlayerPrefs.SetInt(itemAmountKey, amount - requiredAmount);
            PlayerPrefsKeysManager.RegisterKey(itemAmountKey);
        }

        int craftedWeaponCount = PlayerPrefs.GetInt(weaponSO.weaponName + "Amount");
        PlayerPrefs.SetInt(weaponSO.weaponName + "Amount", craftedWeaponCount + 1);  // Add 1 crafted item to inventory
        PlayerPrefs.Save();
        PlayerPrefsKeysManager.RegisterKey(weaponSO.weaponName + "Amount"); // Register crafted item;
        Debug.Log("New " + weaponSO.weaponName + " count: " + PlayerPrefs.GetInt(weaponSO.weaponName + "Amount"));

        int powerLevel = weaponSO.powerLevel;
        if(powerLevel > PlayerPrefs.GetInt("HighestPowerLevel"))
        {
            PlayerPrefs.SetInt("HighestPowerLevel", powerLevel);
        }

        WeaponController.Instance.FindAvailableWeapons();
    }

    public void CraftItems(ItemSO itemSO)
    {
        for (int i = 0; i < itemSO.requiredItems.Length; i++)
        {
            string itemAmountKey = itemSO.requiredItems[i].itemName + "Amount";
            int amount = PlayerPrefs.GetInt(itemAmountKey, 0);
            int requiredAmount = itemSO.requiredItemAmounts[i];

            PlayerPrefs.SetInt(itemAmountKey, amount - requiredAmount);
            PlayerPrefsKeysManager.RegisterKey(itemAmountKey);
        }

        int craftedItemCount = PlayerPrefs.GetInt(itemSO.itemName + "Amount");
        PlayerPrefs.SetInt(itemSO.itemName + "Amount", craftedItemCount + 1);  // Add 1 crafted item to inventory
        PlayerPrefs.Save();
        PlayerPrefsKeysManager.RegisterKey(itemSO.itemName + "Amount");
    }
}
