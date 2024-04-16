using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InsideCraftingTable : MonoBehaviour
{
    bool playerInsideTrigger;
    public static event Action<bool> onCraftingTable;
    bool isPaused;
    bool insideCraftingTable;

    private void Start()
    {
        UIManager.isPaused += UIManager_isPaused;
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
            UIManager.instance.ShowPressEText("to interact");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInsideTrigger = false;
            UIManager.instance.HidePressEText();
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

                UIManager.instance.OpenCraftingUI();
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                UIManager.instance.HidePressEText();
                insideCraftingTable = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) && insideCraftingTable)
        {
            onCraftingTable?.Invoke(false);

            UIManager.instance.CloseCraftingUI();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            UIManager.instance.ShowPressEText("to interact");
            insideCraftingTable = false;
        }
    }
}
