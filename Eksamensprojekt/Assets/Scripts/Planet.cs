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

    public Transform cameraTransform;

    bool mouseOver;

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

        Vector3 directionToCamera = cameraTransform.position - titleCanvas.transform.position;

        // Project the direction onto the xz plane
        directionToCamera.y = 0;

        // Make the object rotate to face the camera (only rotate on y-axis)
        titleCanvas.transform.rotation = Quaternion.LookRotation(directionToCamera.normalized, Vector3.up);

        titleCanvas.transform.Rotate(Vector3.up, 180f);
    }

    private void OnMouseOver()
    {
        if (!planetSelector.PlanetSelected() && !mouseOver)
        {
            transform.DOScale(hoverSize, 0.2f);
            planetSelector.PlayHoverSound();
            mouseOver = true;
        }
    }

    private void OnMouseExit()
    {
        transform.DOScale(originalSize, 0.2f);
        mouseOver = false;
    }

    private void OnMouseUp()
    {
        if (!planetSelector.PlanetSelected())
        {
            planetSelector.ZoomToPlanet(this.transform, planetSO);
        }
    }
}
