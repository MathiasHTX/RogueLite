using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item", order = 2)]
public class ItemSO : ScriptableObject
{
    public string itemName;
    public Sprite itemIcon;
}
