using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System;

public class WeaponSway : MonoBehaviour
{
    public float intensity;
    public float smoothReturnSpeed;

    // Bobbing effect variables
    public float bobSpeed = 2f;
    public float bobAmount = 0.007f;
    public float bobbingSideIntensity = 0.0001f;
    private float defaultPosY = 0f;
    private float timer = 0f;
    private float verticalSpeed;

    private Quaternion origin_rotation;

    // Reference to get player's moving speed and check if grounded
    public Transform playerTransform;
    private Vector3 lastPosition;
    private float playerSpeed;


    bool isPaused;

    private void Start()
    {
        origin_rotation = transform.localRotation;
        defaultPosY = transform.localPosition.y;
        lastPosition = playerTransform.position;

        UIManager.isPaused += UIManager_isPaused;
    }

    private void UIManager_isPaused(bool paused)
    {
        isPaused = paused;
    }

    private void Update()
    {
        if (!isPaused)
        {
            UpdateSway();
            UpdateBob();

            float verticalSpeed = (playerTransform.position.y - lastPosition.y) / Time.deltaTime;
        }
    }

    private void UpdateSway()
    {
        float t_x_mouse = Input.GetAxis("Mouse X");
        float t_y_mouse = Input.GetAxis("Mouse Y");

        Quaternion t_x_adj = Quaternion.AngleAxis(-intensity * t_x_mouse * 5f, Vector3.up);
        Quaternion t_y_adj = Quaternion.AngleAxis(intensity * t_y_mouse, Vector3.right);
        Quaternion target_rotation = origin_rotation * t_x_adj * t_y_adj;

        transform.localRotation = Quaternion.Lerp(transform.localRotation, target_rotation, Time.deltaTime * smoothReturnSpeed);
    }

    private void UpdateBob()
    {
        playerSpeed = (playerTransform.position - lastPosition).magnitude / Time.deltaTime;
        lastPosition = playerTransform.position;

        if (playerSpeed > 0.1f && PlayerMovementAdvanced.instance.IsGrounded())
        {
            timer += Time.deltaTime * bobSpeed * playerSpeed;
            float bobSine = Mathf.Sin(timer) * bobAmount * playerSpeed;
            float bobCosine = Mathf.Cos(timer * 2) * bobbingSideIntensity * playerSpeed;
            transform.localPosition = new Vector3(transform.localPosition.x + bobCosine, defaultPosY + bobSine, transform.localPosition.z);
        }
        else if (playerSpeed > 0.1f && !PlayerMovementAdvanced.instance.IsGrounded())
        {
            if (verticalSpeed > 0) // Going up
            {
                float upwardsLag = -0.5f * verticalSpeed;
                transform.localPosition = new Vector3(transform.localPosition.x, defaultPosY + upwardsLag, transform.localPosition.z);
            }
            else if (verticalSpeed < 0) // Falling down
            {
                float fallingLag = 0.5f * Mathf.Abs(verticalSpeed);
                transform.localPosition = new Vector3(transform.localPosition.x, defaultPosY + fallingLag, transform.localPosition.z);
            }
        }
        else if (playerSpeed < 0.1f)
        {
            timer = 0f;
            // Smoothly return to the default position
            transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(transform.localPosition.x, defaultPosY, transform.localPosition.z), Time.deltaTime * smoothReturnSpeed);
        }
    }
}