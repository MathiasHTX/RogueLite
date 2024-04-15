using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InsideCraftingTable : MonoBehaviour
{
    bool playerInsideTrigger;
    public static event Action<bool> onCraftingTable;
    bool isPaused;

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
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInsideTrigger = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInsideTrigger && !isPaused)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                onCraftingTable?.Invoke(false);

                UIManager.instance.OpenCraftingUI();
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                onCraftingTable?.Invoke(true);

                UIManager.instance.CloseCraftingUI();
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}
