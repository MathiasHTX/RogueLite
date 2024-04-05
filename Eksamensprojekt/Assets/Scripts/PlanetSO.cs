using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Planet", menuName = "Scriptable Objects/Planet", order = 2)]
public class PlanetSO : ScriptableObject
{
    public string title;
    public string description;
    public Material material;
    public bool unlocked = true;
}
