using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class Planet : MonoBehaviour
{
    public PlanetSelector planetSelector;

    public PlanetSO planetSO;

    public float originalSize;
    public float hoverSize = 2.2f;

    public TextMeshProUGUI planetText;
    public Canvas titleCanvas;

    private void Start()
    {
        originalSize = transform.localScale.x;
        GetComponent<MeshRenderer>().material = planetSO.material;
        planetText.text = planetSO.title;
    }

    private void Update()
    {
        float offset = 1.5f;
        Vector3 planetPos = gameObject.transform.position;
        titleCanvas.transform.position = new Vector3(planetPos.x, planetPos.y + offset, planetPos.z);
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
