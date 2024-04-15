using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WeaponController : MonoBehaviour
{
    public static WeaponController Instance;

    bool isDead;

    public GameObject sword;
    bool canAttack = true;
    public float swingCooldown = 0.3f;
    Animator anim;

    [SerializeField] Transform cameraHolder;
    [SerializeField] LayerMask enemyLayerMask;
    [SerializeField] Vector3 boxSize;

    [SerializeField] WeaponSO[] weaponSOs;
    int usingWeapon = 0;

    AudioSource audioSrc;
    [SerializeField] AudioClip[] swordSounds;

    bool isPaused;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        anim = sword.GetComponent<Animator>();
        audioSrc = GetComponent<AudioSource>();
        PlayerMovementAdvanced.onDeath += PlayerMovementAdvanced_onDeath;
        UIManager.isPaused += UIManager_isPaused;
        InsideCraftingTable.onCraftingTable += InsideCraftingTable_onCraftingTable;
    }

    private void InsideCraftingTable_onCraftingTable(bool openClose)
    {
        isPaused = !openClose;
    }

    private void UIManager_isPaused(bool paused)
    {
        isPaused = paused;
    }

    private void PlayerMovementAdvanced_onDeath()
    {
        isDead = true;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (canAttack && !isDead && !isPaused)
            {
                SwordAttack();
            }
        }
    }

    /*
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            trigger.enabled = false;
            //other.GetComponent<EnemyAI>().EnemyHit();
            if(other.TryGetComponent<EnemyAI>(out EnemyAI enemyAIscript))
            {
                enemyAIscript.EnemyHit();
            }
        }
    }
    */

    public void SwordAttack()
    {
        //OnSwordSwing?.Invoke();
        canAttack = false;
        anim.Play("SwordSwing", 0, 0);

        // Sound
        int randomSound = UnityEngine.Random.Range(0, swordSounds.Length);
        float randomPitch = UnityEngine.Random.Range(0.9f, 1.5f);
        audioSrc.pitch = randomPitch;
        audioSrc.clip = swordSounds[randomSound];
        audioSrc.Play();

        Vector3 boxCenter = cameraHolder.position + cameraHolder.forward * (boxSize.z / 2f);

        Collider[] colliders = Physics.OverlapBox(boxCenter, boxSize / 2f, cameraHolder.rotation, enemyLayerMask);

        // Iterate through the colliders found
        foreach (Collider collider in colliders)
        {
            if (!collider.isTrigger)
            {
                // Get the EnemyAI component attached to the collider
                EnemyAI enemyAI = collider.GetComponent<EnemyAI>();

                // Check if the component exists
                if (enemyAI != null)
                {
                    // Call the EnemyHit() function on the EnemyAI component
                    enemyAI.EnemyHit(weaponSOs[usingWeapon].damage);
                }
            }

        }

        StartCoroutine(SwingCooldown());
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.forward * (boxSize.z / 2f), boxSize);
    }

    IEnumerator SwingCooldown()
    {
        yield return new WaitForSeconds(swingCooldown);
        canAttack = true;
    }
}
