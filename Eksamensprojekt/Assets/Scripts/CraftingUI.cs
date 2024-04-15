using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingUI : MonoBehaviour
{
    [SerializeField] CraftItem craftItemScript;
    [SerializeField] Toggle firstToggle;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] TextMeshProUGUI priceText;
    [SerializeField] TextMeshProUGUI powerLevelText;
    [SerializeField] Button craftBtn;
    [SerializeField] TextMeshProUGUI missingItemText;
    [SerializeField] Image itemIcon;
    WeaponSO selectedWeapon;

    private void Start()
    {
        firstToggle.isOn = true;
    }

    public void OpenWeapon(WeaponSO weaponSO)
    {
        int requiredAmount = weaponSO.requiredItemAmounts[0];
        int amountOfItem = PlayerPrefs.GetInt(weaponSO.requiredItems[0].itemName + "Amount", 0);
        if(amountOfItem >= requiredAmount)
        {
            craftBtn.interactable = true;
            missingItemText.gameObject.SetActive(false);
        }
        else
        {
            int missingAmount = requiredAmount - amountOfItem;
            craftBtn.interactable = false;
            missingItemText.text = "Missing " + missingAmount + " " + weaponSO.requiredItems[0].itemName;
            missingItemText.gameObject.SetActive(true);
        }


        nameText.text = weaponSO.weaponName;
        descriptionText.text = weaponSO.description;
        priceText.text = requiredAmount + "x";
        powerLevelText.text = "Power level " + weaponSO.powerLevel;
        itemIcon.sprite = weaponSO.requiredItems[0].itemIcon;

        selectedWeapon = weaponSO;
    }

    public void CraftSelectedItem()
    {
        craftItemScript.CraftItems(selectedWeapon);
    }
}
