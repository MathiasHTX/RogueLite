using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShipLandingSequence : MonoBehaviour
{
    public static ShipLandingSequence instance;

    public static event Action OnExitShip;

    [SerializeField] Animator spaceshipAnim;

    [SerializeField] private float sensX;
    [SerializeField] private float sensY;

    float mouseX;
    float mouseY;

    float multiplier = 0.01f;

    float xRotation;
    float yRotation;

    public GameObject thisCam;
    public GameObject player;

    public GameObject ship;
    public Transform cameraShipPosition;

    public AudioSource loopingSoundtrack;

    public float waitTime;
    public float waitTimeSound;

    public bool canLookAround;

    bool hasExitedSpaceShip;


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        canLookAround = false;

        player.SetActive(false);
        thisCam.SetActive(true);
        spaceshipAnim.Play("SpaceShipLandingEG");
        StartCoroutine(changeCam());
        StartCoroutine(playMusic());
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

    private IEnumerator changeCam()
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime);
            transform.position = cameraShipPosition.position;
            canLookAround = true;
            UIManager.instance.ShowSpaceshipText(false);
        }
    }

    private IEnumerator playMusic()
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTimeSound);
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
}
