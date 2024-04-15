using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class ShipLandingSequence : MonoBehaviour
{
    public static ShipLandingSequence instance;

    public static event Action OnExitShip;

    [Header("Ship animator")]
    [SerializeField] Animator spaceshipAnim;

    [Header("Sensitivity for cockpit Camera")]
    [SerializeField] private float sensX;
    [SerializeField] private float sensY;

    float mouseX;
    float mouseY;

    float multiplier = 0.01f;

    float xRotation;
    float yRotation;

    [Header("")]

    public GameObject thisCam;
    public GameObject player;

    public GameObject ship;
    public Transform cameraShipPosition;

    public AudioSource loopingSoundtrack;

    public float waitTime;
    public float waitTimeSound;

    public bool canLookAround;

    bool hasExitedSpaceShip;

    [Header("Thruster particles")]
    public ParticleSystem btFireL;
    public ParticleSystem btSmokeL;

    public ParticleSystem btFireR;
    public ParticleSystem btSmokeR;

    public ParticleSystem uLtFire1;
    public ParticleSystem uLtSmoke1;

    public ParticleSystem uRtFire1;
    public ParticleSystem uRtSmoke1;

    ParticleSystem.EmissionModule btFireLem;
    ParticleSystem.EmissionModule btSmokeLem;
    ParticleSystem.EmissionModule btFireRem;
    ParticleSystem.EmissionModule btSmokeRem;
    ParticleSystem.EmissionModule uLtFireem;
    ParticleSystem.EmissionModule uLtSmokeem;
    ParticleSystem.EmissionModule uRtFireem;
    ParticleSystem.EmissionModule uRtSmokeem;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;

        }
    }

    private void Start()
    {
        btFireLem = btFireL.GetComponent<ParticleSystem>().emission;
        btSmokeLem = btSmokeL.GetComponent<ParticleSystem>().emission;
        btFireRem = btFireR.GetComponent<ParticleSystem>().emission;
        btSmokeRem = btSmokeR.GetComponent<ParticleSystem>().emission;
        uLtFireem = uLtFire1.GetComponent<ParticleSystem>().emission;
        uLtFireem.enabled = false;
        uLtSmokeem = uLtSmoke1.GetComponent<ParticleSystem>().emission;
        uLtSmokeem.enabled = false;
        uRtFireem = uRtFire1.GetComponent<ParticleSystem>().emission;
        uRtFireem.enabled = false;
        uRtSmokeem = uRtSmoke1.GetComponent<ParticleSystem>().emission;
        uRtSmokeem.enabled = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        canLookAround = false;

        player.SetActive(false);
        thisCam.SetActive(true);
        spaceshipAnim.Play("SpaceShipLandingEG");
        StartCoroutine(changeCam());
        StartCoroutine(playMusic());
        StartCoroutine(thrusterParticles());
    }

    void Update()
    {
        if (!canLookAround)
        {
            transform.LookAt(ship.transform.position);
        }
        else
        {
            MyInput();

            transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!hasExitedSpaceShip)
                {
                    ExitShip();
                }
            }
        }
    }
    private IEnumerator thrusterParticles()
    {
        yield return new WaitForSeconds(4);
        uRtFireem.enabled = true;
        uRtSmokeem.enabled = true;
        yield return new WaitForSeconds(0.4f);
        thisCam.transform.DOShakePosition(5.6f, 0.06f, 15, 50, false, false);
        uLtFireem.enabled = true;
        uLtSmokeem.enabled = true;
        yield return new WaitForSeconds(0.7f);
        uRtFireem.enabled = false;
        uRtSmokeem.enabled = false;
        yield return new WaitForSeconds(0.9f);
        // Main thrusters off
        btFireLem.enabled = false;
        btSmokeLem.enabled = false;
        btFireRem.enabled = false;
        btSmokeRem.enabled = false;

        // Underside thrusters
        uRtFireem.enabled = true;
        uRtSmokeem.enabled = true;
        uLtFireem.enabled = false;
        uLtSmokeem.enabled = false;
        yield return new WaitForSeconds(0.2f);
        uLtFireem.enabled = true;
        uLtSmokeem.enabled = true;
        uRtFireem.enabled = false;
        uRtSmokeem.enabled = false;
        yield return new WaitForSeconds(0.2f);
        uRtFireem.enabled = true;
        uRtSmokeem.enabled = true;
        yield return new WaitForSeconds(3.6f);
        uLtFireem.enabled = false;
        uLtSmokeem.enabled = false;
        uRtFireem.enabled = false;
        uRtSmokeem.enabled = false;
    }

    private IEnumerator changeCam()
    {
        yield return new WaitForSeconds(waitTime);
        transform.position = cameraShipPosition.position;
        canLookAround = true;
        UIManager.instance.ShowPressEText("to exit");
    }

    private IEnumerator playMusic()
    {
        yield return new WaitForSeconds(waitTimeSound);
        if (loopingSoundtrack != null)
        {
            loopingSoundtrack.Play();
        }
    }

    void ExitShip()
    {
        player.SetActive(true);
        thisCam.SetActive(false);
        OnExitShip?.Invoke();
        hasExitedSpaceShip = true;
    }

    void MyInput()
    {
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");

        yRotation += mouseX * sensX * multiplier;
        xRotation -= mouseY * sensY * multiplier;

        yRotation = Mathf.Clamp(yRotation, -70f, 75f);
        xRotation = Mathf.Clamp(xRotation, -70f, 75f);
    }

    public bool HasExitedShip()
    {
        return hasExitedSpaceShip;
    }
}
