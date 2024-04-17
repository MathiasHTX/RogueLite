using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ItemGatherer : MonoBehaviour
{
    [Header("Gathering Settings")]
    public ItemSO item; // Name of the item the player is gonna gather
    public float GatherChance;

    [Header("Hit Detection")]
    public float rayLength = 10.0f; // Length of the ray
    public LayerMask hitLayers; // Layer mask to filter which objects to hit
    public Transform rayOrigin; // Starting point of the ray
    public float forwardOffset = 1.0f; // Define the extra forward offset amount

    [Header("Particles")]
    public GameObject hitParticles; // Particle system to play on hit
    public GameObject gatherParticles; // Particle system to play on hit

    private List<Vector3> treeLocations = new List<Vector3>(); // List to store positions of "Tree" tagged trees
    private bool PlayerIsInTrigger = false;

    [Header("Sounds")]
    public AudioSource audioSrc;
    public AudioClip[] hitSounds;
    public AudioClip gatherSound;

    void Start()
    {
        // Populate the treeLocations list with positions of all "Tree" tagged trees
        foreach (Terrain terrain in Terrain.activeTerrains)
        {
            foreach (TreeInstance tree in terrain.terrainData.treeInstances)
            {
                GameObject treePrefab = terrain.terrainData.treePrototypes[tree.prototypeIndex].prefab;
                if (treePrefab.tag == "Tree")
                {
                    Vector3 treeWorldPosition = Vector3.Scale(tree.position, terrain.terrainData.size) + terrain.transform.position;
                    treeLocations.Add(treeWorldPosition);
                }
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && PlayerIsInTrigger)
        {
            RaycastHit hit;
            // Shoot a ray forward from the rayOrigin
            if (Physics.Raycast(rayOrigin.position, rayOrigin.forward, out hit, rayLength, hitLayers))
            {
                // Adjust hit position with the forward offset
                Vector3 adjustedHitPosition = hit.point + rayOrigin.forward * forwardOffset;

                // Check if the hit position is near any "Tree" tree
                if (IsNearTree(hit.point)) // check proximity using the original hit point
                {
                    if (Random.value <= GatherChance)
                    {
                        // Increment item count in PlayerPrefs
                        PlayerPrefs.SetInt(item.itemName + "Amount", PlayerPrefs.GetInt(item.itemName + "Amount") + 1);
                        PlayerPrefs.Save();
                        PlayerPrefsKeysManager.RegisterKey(item.itemName + "Amount");

                        // Play the particle system at the adjusted hit position
                        Destroy(Instantiate(hitParticles, adjustedHitPosition, Quaternion.identity), 1);
                        Destroy(Instantiate(gatherParticles, adjustedHitPosition, Quaternion.identity), 1);
                        audioSrc.PlayOneShot(gatherSound);
                    }
                    else
                    // Play the particle system at the adjusted hit position
                    Destroy(Instantiate(hitParticles, adjustedHitPosition, Quaternion.identity), 1);
                    int randomSound = Random.Range(0, hitSounds.Length);
                    float randomPitch = Random.Range(0.9f, 1.5f);
                    audioSrc.pitch = randomPitch;
                    audioSrc.PlayOneShot(hitSounds[randomSound]);
                }
            } 
        }
    }

    private bool IsNearTree(Vector3 hitPosition)
    {
        foreach (Vector3 treePosition in treeLocations)
        {
            if (Vector3.Distance(hitPosition, treePosition) < 3.0f) // Consider 3 meters as near for example
            {
                return true;
            }
        }
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerIsInTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerIsInTrigger = false;
        }
    }
}