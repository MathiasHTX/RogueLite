using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public GameObject sword;
    bool canAttack = true;
    public float swingCooldown = 0.2f;
    Animator anim;
    [SerializeField] Collider swordTrigger;

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
        canAttack = false;
        anim.Play("SwordSwing", 0, 0);
        StartCoroutine(ResetSwingCooldown());
    }

    IEnumerator ResetSwingCooldown()
    {
        yield return new WaitForSeconds(swingCooldown);
        canAttack = true;
    }

    public void DisableTrigger()
    {
        swordTrigger.enabled = false;
    }
}
