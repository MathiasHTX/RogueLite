using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomCameraRotation : MonoBehaviour
{
    public float maxRotationChange = 1f; // Maximum rotation change per frame
    public float minAngleChange = -45f; // Minimum angle change for new target
    public float maxAngleChange = 45f; // Maximum angle change for new target

    private Quaternion targetRotation;
    private Quaternion currentRotation;

    void Start()
    {
        // Set initial target rotation
        SetNewTargetRotation();
    }

    void Update()
    {
        // Rotate towards the target rotation
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxRotationChange * Time.deltaTime);

        // Check if reached the target rotation
        if (Quaternion.Angle(transform.rotation, targetRotation) < 0.01f)
        {
            // Set new target rotation
            SetNewTargetRotation();
        }
    }

    void SetNewTargetRotation()
    {
        // Generate a new random rotation within the specified angle range
        float newX = Mathf.Clamp(currentRotation.eulerAngles.x + Random.Range(minAngleChange, maxAngleChange), 0f, 360f);
        float newY = Mathf.Clamp(currentRotation.eulerAngles.y + Random.Range(minAngleChange, maxAngleChange), 0f, 360f);
        float newZ = Mathf.Clamp(currentRotation.eulerAngles.z + Random.Range(minAngleChange, maxAngleChange), 0f, 360f);
        targetRotation = Quaternion.Euler(newX, newY, newZ);

        // Update current rotation for next target calculation
        currentRotation = transform.rotation;
    }
}
