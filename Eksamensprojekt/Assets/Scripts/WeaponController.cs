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
    RuntimeAnimatorController ac;
    float animationLength;

    [SerializeField] Transform cameraHolder;
    [SerializeField] LayerMask enemyLayerMask;
    [SerializeField] Vector3 boxSize;

    [SerializeField] WeaponSO[] weaponSOs;
    [SerializeField] GameObject[] weapons;
    int usingWeapon = 0;
    bool canChangeWeapon = true;

    AudioSource audioSrc;
    [SerializeField] AudioClip[] swordSounds;

    bool isPaused;

    float changeWeaponTimer;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        audioSrc = GetComponent<AudioSource>();
        PlayerMovementAdvanced.onDeath += PlayerMovementAdvanced_onDeath;
        UIManager.isPaused += UIManager_isPaused;
        InsideCraftingTable.onCraftingTable += InsideCraftingTable_onCraftingTable;
        ChangeWeapon(0);
    }

    private void InsideCraftingTable_onCraftingTable(bool openClose)
    {
        isPaused = openClose;
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
                WeaponAttack();
            }
        }

        if(Input.GetAxisRaw("Mouse ScrollWheel") > 0f && usingWeapon < weapons.Length - 1)
        {
            ChangeWeapon(usingWeapon + 1);
        }
        else if(Input.GetAxisRaw("Mouse ScrollWheel") < 0f && usingWeapon > 0)
        {
            ChangeWeapon(usingWeapon - 1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeWeapon(0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeWeapon(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChangeWeapon(2);
        }

        // Makes sure you can't change weapon while anim is playing
        if (canChangeWeapon == false)
        {
            changeWeaponTimer += Time.deltaTime;
            if (changeWeaponTimer >= animationLength)
            {
                canChangeWeapon = true;
            }
        }
    }

    void ChangeWeapon(int number)
    {
        Debug.Log(canChangeWeapon);
        if (canChangeWeapon && !isPaused)
        {
            usingWeapon = number;

            for (int i = 0; i < weapons.Length; i++)
            {
                if (i == usingWeapon)
                {
                    weapons[i].SetActive(true);
                }
                else
                    weapons[i].SetActive(false);
            }

            anim = weapons[usingWeapon].GetComponent<Animator>();
            ac = anim.runtimeAnimatorController;
            animationLength = weaponSOs[usingWeapon].animationLength / 0.6f;
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

    public void WeaponAttack()
    {
        //OnSwordSwing?.Invoke();
        canAttack = false;

        int randomIndex = UnityEngine.Random.Range(0, ac.animationClips.Length);
        string randomClipName = ac.animationClips[randomIndex].name;
        anim.Play(randomClipName, 0, 0);

        // Sound
        int randomSound = UnityEngine.Random.Range(0, swordSounds.Length);
        float randomPitch = UnityEngine.Random.Range(0.9f, 1.5f);
        audioSrc.pitch = randomPitch;
        audioSrc.clip = swordSounds[randomSound];
        audioSrc.Play();

        Vector3 boxSize = weaponSOs[usingWeapon].boxSize;
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

        changeWeaponTimer = 0;
        canChangeWeapon = false;
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
