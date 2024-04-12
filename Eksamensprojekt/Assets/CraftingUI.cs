using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingUI : MonoBehaviour
{
    [SerializeField] Image[] buttonBackgrounds;
    [SerializeField] Color32 enabledColor;
    [SerializeField] Color32 disabledColor;

    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] TextMeshProUGUI priceText;

    public void OpenWeapon(WeaponSO weaponSO)
    {
        nameText.text = weaponSO.weaponName;
        descriptionText.text = weaponSO.description;
        priceText.text = weaponSO.price + "x";
    }

    public void ChangeButtonBackground(Image background)
    {
        for(int i = 0; i < buttonBackgrounds.Length; i++)
        {
            if(buttonBackgrounds[i] == background)
            {
                background.color = enabledColor;
            }
            else
            {
                background.color = disabledColor;
            }
        }
    }
}
