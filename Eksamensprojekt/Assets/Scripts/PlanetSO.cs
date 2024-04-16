using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Planet", menuName = "Scriptable Objects/Planet", order = 2)]
public class PlanetSO : ScriptableObject
{
    public int sceneBuildIndex;
    public string title;
    public int planetNumber;
    public string description;
    public Material material;
    public bool unlocked = true;
}