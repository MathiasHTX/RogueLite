using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowFlight : MonoBehaviour
{
    Transform thisTransform;
    public float decreasingValue = 20f;

    void Start()
    {
        thisTransform = transform;
    }

    void FixedUpdate()
    {
        float decreaseRate = 6.5f;
        decreasingValue -= decreaseRate * Time.deltaTime;

        // Target rotation based on new x-axis angle
        Quaternion targetRotation = Quaternion.Euler(decreasingValue, thisTransform.rotation.eulerAngles.y, thisTransform.rotation.eulerAngles.z);
    }
}
