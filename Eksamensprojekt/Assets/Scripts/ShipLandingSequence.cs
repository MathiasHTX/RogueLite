using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShipLandingSequence : MonoBehaviour
{
    public static ShipLandingSequence instance;

    public static event Action OnExitShip;

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

    public float waitTime;

    public bool canLookAround;


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
        StartCoroutine(changeCam());
    }

    void Update()
    {
        if (!canLookAround)
            transform.LookAt(ship.transform.position);
        else
        {
            MyInput();

            transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);

            if (Input.GetKeyDown(KeyCode.E))
            {
                ExitShip();
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
            UIManager.instance.ShowInsideShipUI();
        }
    }

    void ExitShip()
    {
        player.SetActive(true);
        thisCam.SetActive(false);
        OnExitShip?.Invoke();
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
