using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item", order = 2)]
public class ItemSO : ScriptableObject
{
    public string itemName;
    public string description;
    public Sprite itemIcon;
    public int[] requiredItemAmounts;
    public ItemSO[] requiredItems;
}
