using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    [SerializeField] UIManager uiManager;
    [SerializeField] PlayerMovementAdvanced playerMovementAdvanced;
    [SerializeField] InsideCraftingTable insideCraftingTableScript;

    [SerializeField] private float sensX;
    [SerializeField] private float sensY;

    Camera cam;

    float mouseX;
    float mouseY;

    float multiplier = 0.01f;

    float xRotation;
    float yRotation;

    public GameObject camObj;
    public Transform orientation;

    bool camEnabled = true;
    bool isDead;

    private void Start()
    {
        cam = camObj.GetComponent<Camera>();

        if(PlayerPrefs.GetInt("Tutorial") != 0)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            camEnabled = false;
        }

        uiManager.isPaused += UIManager_isPaused;
        playerMovementAdvanced.onDeath += PlayerMovementAdvanced_onDeath;

        if(insideCraftingTableScript != null)
            insideCraftingTableScript.onCraftingTable += InsideCraftingTable_onCraftingTable;
    }

    private void InsideCraftingTable_onCraftingTable(bool openClose)
    {
        camEnabled = !openClose;
    }

    private void PlayerMovementAdvanced_onDeath()
    {
        isDead = true;
    }

    private void UIManager_isPaused(bool paused)
    {
        camEnabled = !paused;
    }

    private void Update()
    {
        if (camEnabled && !isDead)
        {
            MyInput();

            cam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
            transform.rotation = Quaternion.Euler(0, yRotation, 0);
            orientation.rotation = Quaternion.Euler(0, yRotation, 0);
        }
    }

    void MyInput()
    {
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");

        yRotation += mouseX * sensX * multiplier;
        xRotation -= mouseY * sensY * multiplier;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
    }
}