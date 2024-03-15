using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WeaponController : MonoBehaviour
{
    public static WeaponController Instance;
    public static event Action OnSwordSwing;

    public GameObject sword;
    bool canAttack = true;
    public float swingCooldown = 0.1f;
    Animator anim;
    [SerializeField] Collider swordTrigger;
    int swing = 0;

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
        swordTrigger.enabled = false;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (canAttack)
            {
                swordTrigger.enabled = true;
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
        OnSwordSwing?.Invoke();

        canAttack = false;

        switch (swing)
        {
            case 0:
                anim.Play("SwordSwing", 0, 0f); swing = 1; break;
            case 1:
                anim.Play("SwordSwing2", 0, 0f); swing = 0; break;
        }

        StartCoroutine(ResetSwingCooldown());
    }

    IEnumerator ResetSwingCooldown()
    {
        yield return new WaitForSeconds(swingCooldown);
        canAttack = true;
        yield return new WaitForSeconds(0.3f);
        swing = 0;
    }

    public void DisableTrigger()
    {
        swordTrigger.enabled = false;
    }

    public void EnableTrigger()
    {
        swordTrigger.enabled = true;
    }
}
