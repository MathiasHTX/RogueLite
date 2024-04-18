using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Spaceship : MonoBehaviour
{
    [SerializeField] UIManager uIManager;
    [SerializeField] WaveManager waveManager;
    [SerializeField] ShipLandingSequence shipLandingSequence;

    bool hasExitedSpaceship;
    bool canOpenSpaceShip;

    bool sceneIsHome;

    private void Start()
    {
        shipLandingSequence.OnExitShip += ShipLandingSequence_OnExitShip;
        sceneIsHome = SceneManager.GetActiveScene().buildIndex == 2;
    }

    private void ShipLandingSequence_OnExitShip()
    {
        //hasExitedSpaceship = true;
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
                if (waveManager.IsWaveComplete())
                {
                    uIManager.ShowHoldToLeave();
                }
                else
                {
                    uIManager.ShowCantEnterShipUI();
                }
            }

        }

        if (Input.GetKeyUp(KeyCode.E) && !sceneIsHome)
        {
            uIManager.HideHoldToLeave();
        }
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
            uIManager.ShowPressEText("to enter");
            canOpenSpaceShip = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            uIManager.HidePressEText();
            hasExitedSpaceship = true;
            canOpenSpaceShip = false;
        }
    }
}
