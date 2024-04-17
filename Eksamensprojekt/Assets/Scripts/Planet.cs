using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class Planet : MonoBehaviour
{
    public PlanetSelector planetSelector;

    public PlanetSO planetSO;

    public float originalSize;
    public float hoverSize = 2.2f;

    // UI
    public TextMeshProUGUI planetText;
    public Image lockImage;
    public Sprite unlockIcon;
    public Canvas titleCanvas;

    public Transform cameraTransform;

    int planetLockedState;
    public Material lockedPlanetMat;
    public Material unlockedPlanetMat;

    bool mouseOver;

    private void Start()
    {
        originalSize = transform.localScale.x;
        planetText.text = planetSO.title;

        // Unlock home planet
        if (!PlayerPrefs.HasKey("0LockedState"))
            PlayerPrefs.SetInt("0LockedState", 2);

        CheckLockedState();
    }

    private void Update()
    {
        Vector3 planetPos = gameObject.transform.position;
        titleCanvas.transform.position = new Vector3(planetPos.x, planetPos.y, planetPos.z);

        Vector3 directionToCamera = cameraTransform.position - titleCanvas.transform.position;

        // Project the direction onto the xz plane
        directionToCamera.y = 0;

        // Make the object rotate to face the camera (only rotate on y-axis)
        titleCanvas.transform.rotation = Quaternion.LookRotation(directionToCamera.normalized, Vector3.up);

        titleCanvas.transform.Rotate(Vector3.up, 180f);
    }

    private void OnMouseOver()
    {
        if (!planetSelector.PlanetSelected() && !mouseOver && planetLockedState != 0)
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
            switch (planetLockedState)
            {
                case 0:
                    break; //Deny sound
                case 1:
                    break; // unlock
                case 2:
                    planetSelector.ZoomToPlanet(this.transform, planetSO); break;

            }
        }
    }

    void CheckLockedState()
    {
        planetLockedState = PlayerPrefs.GetInt(planetSO.planetNumber + "LockedState");
        int highestPowerLevel = PlayerPrefs.GetInt("HighestPowerLevel");

        if (highestPowerLevel >= planetSO.powerLevel && planetLockedState == 0)
        {
            planetLockedState = 1; // Can be unlocked
        }

        Debug.Log(planetSO + " " + highestPowerLevel);
        Debug.Log(planetSO + " " + planetLockedState);

        MeshRenderer planetMeshRenderer = GetComponent<MeshRenderer>();

        switch (planetLockedState)
        {
            case 0: // Locked
                planetMeshRenderer.material = lockedPlanetMat;
                planetText.text = "UNKNOWN";
                lockImage.gameObject.SetActive(true); break;
            case 1: // Can be unlocked
                planetMeshRenderer.material = unlockedPlanetMat;
                planetText.text = "UNLOCK!";
                lockImage.sprite = unlockIcon;
                lockImage.gameObject.SetActive(true); break;
            case 2: // Unlocked
                planetMeshRenderer.material = planetSO.material;
                planetText.text = planetSO.title;
                lockImage.gameObject.SetActive(false); break;
        }
    }
}
