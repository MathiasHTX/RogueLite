using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inventory : MonoBehaviour
{
    bool isOpen;



    [Header("Items")]
    [SerializeField] GameObject[] itemUI;
    [SerializeField] Image[] itemIcons;
    [SerializeField] TextMeshProUGUI[] itemAmountTexts;
    [SerializeField] ItemSO[] itemSOs;

    [Header("Weapons")]
    [SerializeField] GameObject[] weaponUI;
    [SerializeField] Image[] weaponIcons;
    [SerializeField] WeaponSO[] weaponSOs;

    private void OnEnable()
    {
        UpdateInventory();
    }

    void UpdateInventory()
    {
        for(int i = 0; i < itemSOs.Length; i++)
        {
            int amount = PlayerPrefs.GetInt(itemSOs[i].itemName + "Amount");

            if (amount > 0)
            {
                itemIcons[i].sprite = itemSOs[i].itemIcon;
                itemAmountTexts[i].text = amount.ToString();
                itemUI[i].SetActive(true);
            }
            else
            {
                itemUI[i].SetActive(false);
            }
        }

        for (int i = 0; i < weaponSOs.Length; i++)
        {
            int amount = PlayerPrefs.GetInt(weaponSOs[i].weaponName + "Amount");

            if (amount > 0)
            {
                weaponIcons[i].sprite = weaponSOs[i].weaponIcon;
                weaponUI[i].SetActive(true);
            }
            else
            {
                weaponUI[i].SetActive(false);
            }
        }
    }
}
