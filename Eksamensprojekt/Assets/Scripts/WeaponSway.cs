using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    public float intensity;
    public float smoothReturnSpeed;

    // Bobbing effect variables
    public float walkingBobSpeed = 2f;
    public float walkingBobAmount = 0.007f;
    public float jumpingBobSpeed = 2f;
    public float jumpingBobAmount = 0.007f;
    public float bobbingSideIntensity = 0.0001f;
    private float defaultPosY = 0f;
    private float timer = 0f;
    private float verticalSpeed;

    private Quaternion origin_rotation;

    // Reference to get player's moving speed and check if grounded
    public Transform playerTransform;
    private Vector3 lastPosition;
    private float playerSpeed;

    private void Start()
    {
        origin_rotation = transform.localRotation;
        defaultPosY = transform.localPosition.y;
        lastPosition = playerTransform.position;
    }

    private void Update()
    {
        UpdateSway();
        UpdateBob();

        verticalSpeed = (playerTransform.position.y - lastPosition.y) / Time.deltaTime;
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

        bool isGrounded = PlayerMovementAdvanced.instance.IsGrounded();

        if (playerSpeed > 0.1f && isGrounded)
        {
            timer += Time.deltaTime * walkingBobSpeed * playerSpeed;
            float bobSine = Mathf.Sin(timer) * walkingBobAmount * playerSpeed;
            float bobCosine = Mathf.Cos(timer * 2) * bobbingSideIntensity * playerSpeed;
            SmoothMoveWeapon(new Vector3(transform.localPosition.x + bobCosine, defaultPosY + bobSine, transform.localPosition.z), walkingBobSpeed);
        }
        else if (playerSpeed > 0.1f && !isGrounded)
        {
            // Calculate the vertical component of the player's velocity
            float verticalVelocity = Vector3.Dot(playerTransform.GetComponent<Rigidbody>().velocity, playerTransform.up);

            // Use the vertical velocity for bobbing instead of playerSpeed
            timer += Time.deltaTime * jumpingBobSpeed * Mathf.Abs(verticalVelocity);
            float bobSine = Mathf.Sin(timer) * jumpingBobAmount * Mathf.Abs(verticalVelocity);
            float bobCosine = Mathf.Cos(timer * 2) * bobbingSideIntensity * Mathf.Abs(verticalVelocity);

            // Apply the bobbing effect based on the vertical velocity
            SmoothMoveWeapon(new Vector3(transform.localPosition.x + bobCosine, defaultPosY + bobSine, transform.localPosition.z), jumpingBobSpeed);
        }
        else if (playerSpeed < 0.1f)
        {
            // Smoothly return to the default position
            SmoothMoveWeapon(new Vector3(transform.localPosition.x, defaultPosY, transform.localPosition.z), smoothReturnSpeed);
        }
    }

    private void SmoothMoveWeapon(Vector3 targetPosition, float speed)
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * speed);
    }
}