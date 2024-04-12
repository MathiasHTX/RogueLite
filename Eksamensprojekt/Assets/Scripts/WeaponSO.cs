using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Scriptable Objects/Weapon", order = 1)]
public class WeaponSO : ScriptableObject
{
    public string weaponName;
    public int damage;
    public int price;
    public ItemSO requiredItem;
}
