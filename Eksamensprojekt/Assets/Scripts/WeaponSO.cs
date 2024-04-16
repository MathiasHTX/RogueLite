using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Scriptable Objects/Weapon", order = 1)]
public class WeaponSO : ScriptableObject
{
    public string weaponName;
    public string description;
    public int damage;
    public int[] requiredItemAmounts;
    public ItemSO[] requiredItems;
    public int powerLevel;
    public Vector3 boxSize;
    public float animationLength;
}
