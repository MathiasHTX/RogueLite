using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InsideCraftingTable : MonoBehaviour
{
    [SerializeField] UIManager uIManager;

    bool playerInsideTrigger;
    public event Action<bool> onCraftingTable;
    bool isPaused;
    bool insideCraftingTable;

    private void Start()
    {
        uIManager.isPaused += UIManager_isPaused;
    }

    private void UIManager_isPaused(bool paused)
    {
        isPaused = paused;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInsideTrigger = true;
            uIManager.ShowPressEText("to interact");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInsideTrigger = false;
            uIManager.HidePressEText();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInsideTrigger && !isPaused)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                onCraftingTable?.Invoke(true);

                uIManager.OpenCraftingUI();
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                uIManager.HidePressEText();
                insideCraftingTable = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) && insideCraftingTable)
        {
            onCraftingTable?.Invoke(false);

            uIManager.CloseCraftingUI();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            uIManager.ShowPressEText("to interact");
            insideCraftingTable = false;
        }
    }
}
