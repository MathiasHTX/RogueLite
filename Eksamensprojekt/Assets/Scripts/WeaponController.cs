using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WeaponController : MonoBehaviour
{
    [SerializeField] UIManager uIManager;

    bool isDead;

    bool canAttack = true;
    public float swingCooldown = 0.3f;

    Animator anim;
    RuntimeAnimatorController ac;
    float animationLength;

    [Header("General Settings")]
    [SerializeField] Transform cameraHolder;
    [SerializeField] LayerMask enemyLayerMask;
    private Vector3 boxSize;

    [Header("")]
    [SerializeField] WeaponSO[] weaponSOs;
    [SerializeField] GameObject[] allWeapons;
    public List<GameObject> availableWeapons = new List<GameObject>();
    int usingWeapon = 0;
    bool canChangeWeapon = true;
    bool hasNoWeapons;
    bool isUsingBow = true;

    AudioSource audioSrc;

    [Header("Sword Settings")]
    [SerializeField] AudioClip[] swordSounds;

    [Header("Crossbow Settings")]
    public Transform arrowInstantiatePoint;
    public GameObject arrowPrefab;
    public float arrowSpeed;
    public GameObject crossbow;
    [SerializeField] AudioClip crossbowSound;

    bool isPaused;

    bool insideCraftingTable;

    float changeWeaponTimer;

    private void Start()
    {
        audioSrc = GetComponent<AudioSource>();
        PlayerMovementAdvanced.onDeath += PlayerMovementAdvanced_onDeath;
        uIManager.isPaused += UIManager_isPaused;
        InsideCraftingTable.onCraftingTable += InsideCraftingTable_onCraftingTable;
        FindAvailableWeapons();
    }

    public void FindAvailableWeapons()
    {
        for(int i = 0; i < weaponSOs.Length; i++)
        {
            if (PlayerPrefs.GetInt(weaponSOs[i].weaponName + "Amount") >= 1)
            {
                if (!availableWeapons.Contains(allWeapons[i]))
                {
                    availableWeapons.Add(allWeapons[i]);
                }
            }
        }

        hasNoWeapons = availableWeapons.Count == 0 ? true : false;

        if (!hasNoWeapons)
        {
            ChangeWeapon(availableWeapons.Count - 1);
        }
    }

    private void InsideCraftingTable_onCraftingTable(bool openClose)
    {
        insideCraftingTable = openClose;
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
            if (canAttack && !isDead && !insideCraftingTable && !hasNoWeapons)
            {
                if (isUsingBow)
                {
                    Crossbow();
                }
                else
                {
                    WeaponAttack();
                }
            }
        }

        if(Input.GetAxisRaw("Mouse ScrollWheel") > 0f && usingWeapon < availableWeapons.Count - 1)
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

        if(!hasNoWeapons && number < availableWeapons.Count)
        {
            if (canChangeWeapon && !isPaused)
            {
                usingWeapon = number;

                for (int i = 0; i < availableWeapons.Count; i++)
                {
                    if (i == usingWeapon)
                    {
                        availableWeapons[i].SetActive(true);
                    }
                    else
                        availableWeapons[i].SetActive(false);
                }

                anim = availableWeapons[usingWeapon].GetComponent<Animator>();
                ac = anim.runtimeAnimatorController;
                Debug.Log(weaponSOs[usingWeapon]);
                animationLength = weaponSOs[usingWeapon].animationLength / 0.6f;

                if (weaponSOs[usingWeapon].isBow)
                {
                    isUsingBow = true;
                }
                else
                    isUsingBow = false;
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

    public void Crossbow()
    {
        GameObject spawnedArrow = Instantiate(arrowPrefab);
        Animator crossbowAnim = crossbow.GetComponent<Animator>();
        crossbowAnim.Play("CrossbowShoot");
        Debug.Log("Played animation!");
        spawnedArrow.transform.position = arrowInstantiatePoint.transform.position;
        spawnedArrow.transform.rotation = arrowInstantiatePoint.transform.rotation;
        Rigidbody arrowRb = spawnedArrow.GetComponent<Rigidbody>();

        Vector3 direction = new Vector3(arrowInstantiatePoint.transform.up.x, arrowInstantiatePoint.transform.up.y, arrowInstantiatePoint.up.z);
        arrowRb.AddForce(direction * arrowSpeed, ForceMode.Impulse);
        audioSrc.PlayOneShot(crossbowSound);
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
