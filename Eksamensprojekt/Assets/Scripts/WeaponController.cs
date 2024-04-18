using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class WeaponController : MonoBehaviour
{
    [SerializeField] UIManager uIManager;
    [SerializeField] PlayerMovementAdvanced playerMovementAdvanced;
    [SerializeField] InsideCraftingTable insideCraftingTableScript;

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

    [Header("Weapons")]
    [SerializeField] WeaponSO[] weaponSOs;
    [SerializeField] GameObject[] allWeapons;
    public List<GameObject> availableWeapons = new List<GameObject>();
    public List<WeaponSO> availableWeaponsSO = new List<WeaponSO>();
    int usingWeapon = 0;
    bool canChangeWeapon = true;
    bool hasNoWeapons;
    bool isUsingBow;

    AudioSource audioSrc;

    [Header("Sword Settings")]
    [SerializeField] AudioClip[] swordSounds;

    [Header("Crossbow Settings")]
    public Transform arrowInstantiatePoint;
    public GameObject arrowPrefab;
    public float arrowSpeed;
    public GameObject crossbow;
    [SerializeField] AudioClip crossbowSound;
    [SerializeField] GameObject visualArrow;
    int arrowAmount;
    [SerializeField] ItemSO arrowSO;

    bool isPaused;

    bool insideCraftingTable;

    float changeWeaponTimer;

    bool sceneIsHome;

    private void Start()
    {
        audioSrc = GetComponent<AudioSource>();

        playerMovementAdvanced.onDeath += PlayerMovementAdvanced_onDeath;
        uIManager.isPaused += UIManager_isPaused;

        if (insideCraftingTableScript != null)
            insideCraftingTableScript.onCraftingTable += InsideCraftingTable_onCraftingTable;

        UpdateArrowAmount();

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

                if (!availableWeaponsSO.Contains(weaponSOs[i]))
                {
                    availableWeaponsSO.Add(weaponSOs[i]);
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
                Debug.Log(isUsingBow);
                if (isUsingBow)
                {
                    CrossBowAttack();
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
                animationLength = availableWeaponsSO[usingWeapon].animationLength / 0.6f;
                boxSize = availableWeaponsSO[usingWeapon].boxSize;
                swingCooldown = availableWeaponsSO[usingWeapon].swingCooldown;

                if (availableWeaponsSO[usingWeapon].isBow)
                {
                    isUsingBow = true;
                }
                else
                    isUsingBow = false;
            }
        }

    }

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

    /*
    public void Crossbow()
    {
        Vector3 startPosition = arrowInstantiatePoint.transform.position;
        Quaternion startRotation = arrowInstantiatePoint.transform.rotation;
        GameObject spawnedArrow = Instantiate(arrowPrefab, arrowInstantiatePoint.transform.position, arrowInstantiatePoint.transform.rotation);
        Animator crossbowAnim = crossbow.GetComponent<Animator>();
        crossbowAnim.Play("CrossbowShoot");
        Debug.Log("Played animation!");
        Rigidbody arrowRb = spawnedArrow.GetComponent<Rigidbody>();

        Vector3 direction = new Vector3(arrowInstantiatePoint.transform.up.x, arrowInstantiatePoint.transform.up.y, arrowInstantiatePoint.up.z);
        arrowRb.AddForce(direction * arrowSpeed, ForceMode.Impulse);
        audioSrc.PlayOneShot(crossbowSound);

        Destroy(spawnedArrow, 5.0f);
    }
    */

    public void CrossBowAttack()
    {
        if(arrowAmount > 0)
        {
            arrowAmount--;
            PlayerPrefs.SetInt(arrowSO.itemName + "Amount", arrowAmount);

            canAttack = false;

            anim.Play("CrossbowShoot");

            // Shoot arrow
            GameObject arrow = Instantiate(arrowPrefab, arrowInstantiatePoint.position, arrowInstantiatePoint.rotation);
            Rigidbody arrowRb = arrow.GetComponent<Rigidbody>();
            Vector3 shootDirection = arrowInstantiatePoint.forward;
            arrowRb.AddForce(shootDirection * arrowSpeed, ForceMode.Impulse);

            // Hide and show fake arrow
            visualArrow.transform.DOKill();
            visualArrow.transform.DOScale(0, 0);

            if(arrowAmount > 0)
            {
                visualArrow.transform.DOScale(2.2f, 0.2f).SetDelay(0.3f);
            }

            audioSrc.PlayOneShot(crossbowSound);

            StartCoroutine(SwingCooldown());

            Destroy(arrow, 5);
        }
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

    public void UpdateArrowAmount()
    {
        arrowAmount = PlayerPrefs.GetInt(arrowSO.itemName + "Amount");
        if (arrowAmount > 0)
        {
            visualArrow.SetActive(true);
        }
        else
        {
            visualArrow.SetActive(false);
        }
    }
}
