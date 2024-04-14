using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Spaceship : MonoBehaviour
{
    public static Spaceship instance;

    bool hasExitedSpaceship;
    bool canOpenSpaceShip;

    bool sceneIsHome;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        ShipLandingSequence.OnExitShip += ShipLandingSequence_OnExitShip;
        sceneIsHome = SceneManager.GetActiveScene().buildIndex == 2;
    }

    private void ShipLandingSequence_OnExitShip()
    {
        hasExitedSpaceship = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && canOpenSpaceShip)
        {
            if (sceneIsHome)
            {
                OpenSpaceship();
            }
            else
            {
                if (WaveManager.instance.IsWaveComplete())
                {
                    UIManager.instance.ShowHoldToLeave();
                }
                else
                {
                    UIManager.instance.ShowCantEnterShipUI();
                }
            }

        }

        // if (Input.GetKeyUp(KeyCode.E))
        //{
          //  UIManager.instance.HideHoldToLeave();
        //}
    }

    public void OpenSpaceship()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene(1);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && hasExitedSpaceship)
        {
            UIManager.instance.ShowSpaceshipText(true);
            canOpenSpaceShip = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UIManager.instance.HideSpaceshipText();
            hasExitedSpaceship = true;
            canOpenSpaceShip = false;
        }
    }
}
