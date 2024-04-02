using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCircle : MonoBehaviour
{
    [SerializeField] LineRenderer[] lineRenderers;

    [SerializeField] int subdivisions = 10;
    [SerializeField] float[] radiuses;

    // Start is called before the first frame update
    void Start()
    {
        for(int j = 0; j < lineRenderers.Length; j++)
        {
            float angleStep = 2f * Mathf.PI / subdivisions;

            lineRenderers[j].positionCount = subdivisions;

            for (int i = 0; i < subdivisions; i++)
            {
                float xPosition = radiuses[j] * Mathf.Cos(angleStep * i);
                float zPosition = radiuses[j] * Mathf.Sin(angleStep * i);

                Vector3 pointInCircle = new Vector3(xPosition, 0f, zPosition);

                lineRenderers[j].SetPosition(i, pointInCircle);
            }
        }
    }
}
