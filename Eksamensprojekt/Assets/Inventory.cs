using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inventory : MonoBehaviour
{
    bool isOpen;

    [SerializeField] Color emptySlotColor;

    [Header("Items")]
    [SerializeField] Image[] itemImages;
    [SerializeField] Image[] itemIcons;
    [SerializeField] TextMeshProUGUI[] itemAmountTexts;
    [SerializeField] ItemSO[] itemSOs;
    List<ItemSO> ownedItems = new List<ItemSO>();

    [Header("Weapons")]
    [SerializeField] Image[] weaponImages;
    [SerializeField] Image[] weaponIcons;
    [SerializeField] WeaponSO[] weaponSOs;
    List<WeaponSO> ownedWeapons = new List<WeaponSO>();

    private void OnEnable()
    {
        UpdateInventory();
    }

    void UpdateInventory()
    {
        for(int i = 0; i < itemImages.Length; i++)
        {
            itemImages[i].color = emptySlotColor;
            itemIcons[i].gameObject.SetActive(false);
            itemAmountTexts[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < weaponImages.Length; i++)
        {
            weaponImages[i].color = emptySlotColor;
            weaponIcons[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < itemSOs.Length; i++)
        {
            int amount = PlayerPrefs.GetInt(itemSOs[i].itemName + "Amount");
            if (amount > 0 && !ownedItems.Contains(itemSOs[i]))
            {
                ownedItems.Add(itemSOs[i]);
            }
            else if(amount <= 0 && ownedItems.Contains(itemSOs[i]))
            {
                ownedItems.Remove(itemSOs[i]);
            }
        }

        for (int i = 0; i < weaponSOs.Length; i++)
        {
            int amount = PlayerPrefs.GetInt(weaponSOs[i].weaponName + "Amount");
            if (amount > 0 && !ownedWeapons.Contains(weaponSOs[i]))
            {
                ownedWeapons.Add(weaponSOs[i]);
            }
            else if (amount <= 0 && ownedItems.Contains(itemSOs[i]))
            {
                ownedWeapons.Remove(weaponSOs[i]);
            }
        }

        if (ownedItems.Count > 0)
        {
            for (int i = 0; i < ownedItems.Count; i++)
            {
                int amount = PlayerPrefs.GetInt(ownedItems[i].itemName + "Amount");
                itemImages[i].color = Color.white;
                itemIcons[i].sprite = ownedItems[i].itemIcon;
                itemIcons[i].gameObject.SetActive(true);
                itemAmountTexts[i].gameObject.SetActive(true);
                itemAmountTexts[i].text = amount.ToString();
            }
        }

        if (ownedWeapons.Count > 0)
        {
            for (int i = 0; i < ownedWeapons.Count; i++)
            {
                int amount = PlayerPrefs.GetInt(ownedWeapons[i].weaponName + "Amount");
                weaponImages[i].color = Color.white;
                weaponIcons[i].sprite = ownedWeapons[i].weaponIcon;
                weaponIcons[i].gameObject.SetActive(true);
            }
        }
    }
}
