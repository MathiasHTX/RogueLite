using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class CraftingUI : MonoBehaviour
{
    [SerializeField] WeaponSO[] weaponSOs;
    [SerializeField] CraftItem craftItemScript;
    [SerializeField] Toggle[] firstItemToggles;
    [SerializeField] Toggle firstTabsToggle;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] GameObject[] requiredItemsUI;
    [SerializeField] TextMeshProUGUI[] requiredItemsText;
    [SerializeField] Image[] requiredItemsIcons;
    [SerializeField] GameObject[] weaponCheckmarks;
    [SerializeField] TextMeshProUGUI powerLevelText;
    [SerializeField] Button craftBtn;
    [SerializeField] TextMeshProUGUI missingItemText;
    [SerializeField] GameObject ownedText;
    [SerializeField] GameObject[] grids;

    WeaponSO selectedWeapon;

    [SerializeField] WeaponSO firstWeapon;

    private void Start()
    {
        firstTabsToggle.isOn = true;
        InsideCraftingTable.onCraftingTable += InsideCraftingTable_onCraftingTable;
        UpdateCheckmarks();
        OpenGrid(0);

        selectedWeapon = firstWeapon;
    }

    private void InsideCraftingTable_onCraftingTable(bool obj)
    {
        firstTabsToggle.isOn = true;
        firstItemToggles[0].isOn = true;
        OpenWeapon(firstWeapon);
    }

    public void OpenWeapon(WeaponSO weaponSO)
    {
        bool hasAllRequiredItems = true;
        string missingItemsString = "";

        // Start by disabling every required item
        for(int i = 0; i < requiredItemsUI.Length; i++)
            requiredItemsUI[i].SetActive(false);

        if(PlayerPrefs.GetInt(weaponSO.weaponName + "Amount") >= 1)
        {
            missingItemText.gameObject.SetActive(false);
            craftBtn.gameObject.SetActive(false);
            ownedText.gameObject.SetActive(true);
        }
        else
        {
            missingItemText.gameObject.SetActive(true);
            craftBtn.gameObject.SetActive(true);
            ownedText.gameObject.SetActive(false);

            for (int i = 0; i < weaponSO.requiredItems.Length; i++)
            {
                // Enable the necessary items
                requiredItemsText[i].text = weaponSO.requiredItemAmounts[i] + "x";
                requiredItemsIcons[i].sprite = weaponSO.requiredItems[i].itemIcon;
                requiredItemsUI[i].SetActive(true);

                int requiredAmount = weaponSO.requiredItemAmounts[i];
                int amountOfItem = PlayerPrefs.GetInt(weaponSO.requiredItems[i].itemName + "Amount", 0);

                if (amountOfItem < requiredAmount)
                {
                    hasAllRequiredItems = false;
                    int missingAmount = requiredAmount - amountOfItem;

                    if (missingItemsString != "")
                        missingItemsString += " and ";
                    missingItemsString += missingAmount + " " + weaponSO.requiredItems[i].itemName;
                }
            }

            if (hasAllRequiredItems)
            {
                craftBtn.interactable = true;
                missingItemText.gameObject.SetActive(false);
            }
            else
            {
                craftBtn.interactable = false;
                missingItemText.text = "Missing " + missingItemsString;
                missingItemText.gameObject.SetActive(true);
            }
        }


        nameText.text = weaponSO.weaponName;
        descriptionText.text = weaponSO.description;
        powerLevelText.text = "Power level " + weaponSO.powerLevel;


        selectedWeapon = weaponSO;
    }

    public void CraftSelectedItem()
    {
        Debug.Log(selectedWeapon);
        craftItemScript.CraftItems(selectedWeapon);
        ItemCrafted();
        UpdateCheckmarks();

        //Update the UI;
        OpenWeapon(selectedWeapon);
    }

    public void ItemCrafted()
    {
        craftBtn.gameObject.SetActive(false);
        missingItemText.gameObject.SetActive(false);
        ownedText.transform.localScale = Vector3.zero;
        ownedText.SetActive(true);
        ownedText.transform.DOScale(1, 0.5f).SetEase(Ease.OutElastic);
    }

    public void UpdateCheckmarks()
    {
        for(int i = 0; i < weaponSOs.Length; i++)
        {
            if(PlayerPrefs.GetInt(weaponSOs[i].weaponName + "Amount") >= 1)
            {
                weaponCheckmarks[i].SetActive(true);
            }
            else
                weaponCheckmarks[i].SetActive(false);
        }
    }

    public void OpenGrid(int gridNumber)
    {
        firstItemToggles[gridNumber].isOn = true;

        for(int i = 0; i < grids.Length; i++)
        {
            if(i == gridNumber)
            {
                grids[i].SetActive(true);
            }
            else
                grids[i].SetActive(false);
        }
    }
}
