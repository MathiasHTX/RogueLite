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
    WeaponSO selectedWeapon;

    private void Start()
    {
        firstToggle.isOn = true;
    }

    public void OpenWeapon(WeaponSO weaponSO)
    {
        int price = weaponSO.price;
        int amountOfItem = PlayerPrefs.GetInt(weaponSO.requiredItem.itemName + "Amount", 0);
        if(amountOfItem >= price)
        {
            craftBtn.interactable = true;
            missingItemText.gameObject.SetActive(false);
        }
        else
        {
            int missingAmount = price - amountOfItem;
            craftBtn.interactable = false;
            missingItemText.text = "Missing " + missingAmount + " " + weaponSO.requiredItem.itemName;
            missingItemText.gameObject.SetActive(true);
        }


        nameText.text = weaponSO.weaponName;
        descriptionText.text = weaponSO.description;
        priceText.text = price + "x";
        powerLevelText.text = "Power level " + weaponSO.powerLevel;

        selectedWeapon = weaponSO;
    }

    public void CraftSelectedItem()
    {
        craftItemScript.CraftItems(selectedWeapon);
    }
}
