using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetManager : MonoBehaviour
{
    private void Start()
    {
        Debug.Log(RenderSettings.ambientIntensity);
        RenderSettings.ambientIntensity = 1f;
    }
}
