using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Planet : MonoBehaviour
{
    public PlanetSelector planetSelector;

    public PlanetSO planetSO;

    public float originalSize;
    public float hoverSize = 2.2f;

    private void Start()
    {
        originalSize = transform.localScale.x;
    }

    private void OnMouseOver()
    {
        if (!planetSelector.PlanetSelected())
        {
            transform.DOScale(hoverSize, 0.2f);
        }
    }

    private void OnMouseExit()
    {
        transform.DOScale(originalSize, 0.2f);
    }

    private void OnMouseUp()
    {
        if (!planetSelector.PlanetSelected())
        {
            planetSelector.ZoomToPlanet(this.transform, planetSO);
        }
    }
}
