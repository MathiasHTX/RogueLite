using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snowball : MonoBehaviour
{
    public Rigidbody rb;
    public Vector3 originForce;
    public float rollForce;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (rb != null)
        {
            rb.AddForce(originForce * rollForce, ForceMode.Force);
        }
    }

    public void Initialize(Vector3 force, float size, float rollForce)
    {
        transform.localScale = new Vector3(size, size, size);
        originForce = force;
        this.rollForce = rollForce;
        rb.AddForce(force, ForceMode.Impulse);
    }
}
