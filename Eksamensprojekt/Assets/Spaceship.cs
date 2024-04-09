using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Spaceship : MonoBehaviour
{
    bool hasExitedSpaceship;
    bool canOpenSpaceShip;

    bool sceneIsHome;

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
                    OpenSpaceship();
                }
                else
                {
                    UIManager.instance.ShowCantEnterShipUI();
                }
            }

        }
    }

    void OpenSpaceship()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        PlayerPrefs.SetInt("SceneThatOpenedShip", SceneManager.GetActiveScene().buildIndex);
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
