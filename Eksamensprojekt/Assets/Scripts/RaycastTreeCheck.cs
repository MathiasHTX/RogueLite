using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastTreeCheck : MonoBehaviour
{
    public float raycastDistance = 100f; // Max distance for the raycast
    public Color debugRayColor = Color.red; // Color for the debug ray
    public Transform playerOrientationObject; // Reference to the player orientation object

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Check for left mouse button click
        {
            if (playerOrientationObject == null)
            {
                Debug.LogError("Player orientation object not assigned!");
                return;
            }

            // Get the forward direction of the orientation object
            Vector3 playerForward = playerOrientationObject.forward;

            // Create a ray from the object's position in the direction the player is facing
            Ray ray = new Ray(transform.position, playerForward);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, raycastDistance)) // Check if the ray hits anything
            {
                // Check if the hit object is part of the terrain
                Terrain terrain = hit.collider.GetComponent<Terrain>();
                if (terrain != null)
                {
                    // Access the tree data of the terrain
                    TreeInstance[] treeInstances = terrain.terrainData.treeInstances;

                    foreach (TreeInstance treeInstance in treeInstances)
                    {
                        // Get the position of the tree in world space
                        Vector3 treeWorldPosition = Vector3.Scale(treeInstance.position, terrain.terrainData.size) + terrain.transform.position;

                        // Calculate the distance from the hit point to the tree position
                        float distanceToTree = Vector3.Distance(hit.point, treeWorldPosition);

                        // If the distance is within a threshold, consider it a hit on a tree
                        if (distanceToTree < 2f) // Adjust the threshold as needed
                        {
                            Debug.Log("Hit a tree!");
                            // Draw a debug ray to visualize the raycast
                            Debug.DrawRay(ray.origin, ray.direction * raycastDistance, debugRayColor);
                            return; // Exit the method after detecting a tree hit
                        }
                    }
                }

                Debug.Log("Did not hit a tree.");
            }
        }
    }
}
