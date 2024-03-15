using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using DG.Tweening;

public class EnemyAI : MonoBehaviour
{
    GameObject player;

    NavMeshAgent agent;

    [SerializeField] LayerMask groundLayer, playerLayer, swordLayer;

    [SerializeField] Animator animator;

    Rigidbody rb;
    Collider trigger;

    [SerializeField] float hitForce;
    [SerializeField] float hitYForce = 0;

    GameObject playerOrientation;

    bool isDead = false;

    // Patrol
    Vector3 destinationPoint;
    bool walkPointSet;
    [SerializeField] float walkingRange;

    //State change
    [SerializeField] float sightRange, attackRange;
    bool playerInSight, playerInAttackRange;

    //Health and UI
    int startHealth = 3;
    int health;
    [SerializeField] Image healthBar;
    [SerializeField] Gradient colorGradient;
    bool isHit;

    [SerializeField] ParticleSystem slimeParticle;

    // Start is called before the first frame update
    void Start()
    {
        WeaponController.OnSwordSwing += WeaponController_OnSwordSwing;

        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player");
        playerOrientation = GameObject.FindWithTag("PlayerOrientation");
        rb = GetComponent<Rigidbody>();
        trigger = GetComponent<BoxCollider>();

        FindHealth();
        UpdateHealthBar();
    }

    private void WeaponController_OnSwordSwing()
    {
        float radius = 3f;
        if(Physics.CheckSphere(transform.position, radius, swordLayer))
        {
            EnemyHit();
        }
    }

    // Update is called once per frame
    void Update()
    {
        playerInSight = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

        if (!playerInSight && !playerInAttackRange)
            Patrol();

        if (playerInSight && !playerInAttackRange)
            Chase();

        if (playerInSight && playerInAttackRange)
            Attack();

    }

    void FindHealth()
    {
        int healthMultiplier = Random.Range(1, 4);

        // StartHealth can be 3, 6 or 9
        startHealth *= healthMultiplier;

        float scaleFactor = 1f + (healthMultiplier - 1f) * 0.5f;
        transform.localScale = Vector3.one * scaleFactor;
        health = startHealth;

        Debug.Log("dpoawjdo" + health);
    }

    void Chase()
    {
        if (agent.enabled)
            agent.SetDestination(player.transform.position);

    }

    void Attack()
    {
        /*
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("SlimeBlobAttack"))
        {
            animator.SetTrigger("Attack");
            agent.SetDestination(transform.position);
        }
        */

        if (agent.enabled)
            agent.SetDestination(transform.position);

    }

    void Patrol()
    {
        if (!walkPointSet)
        {
            SearchForDestination();
        }

        if (walkPointSet)
        {
            if (agent.enabled)
                agent.SetDestination(destinationPoint);
        }

        if (Vector3.Distance(transform.position, destinationPoint) < 10)
            walkPointSet = false;
    }

    void SearchForDestination()
    {
        float z = Random.Range(-walkingRange, walkingRange);
        float x = Random.Range(-walkingRange, walkingRange);

        destinationPoint = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z);

        if (Physics.Raycast(destinationPoint, Vector3.down, groundLayer))
        {
            walkPointSet = true;
        }
    }

    /*
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sword"))
        {
            //trigger.enabled = false;
            EnemyHit();
        }
    }
    */


    public void EnemyHit()
    {
        Vector3 playerDirection = playerOrientation.transform.forward;
        Vector3 forceDirection = new Vector3(playerDirection.x * hitForce, hitYForce, playerDirection.z * hitForce);
        if (!isHit && !isDead)
        {
            health--;

            if (health == 0)
            {
                Death();
            }
            else
            {
                StartCoroutine(ApplyKnockback(forceDirection));
            }
        }

    }

    IEnumerator ApplyKnockback(Vector3 force)
    {
        Debug.Log("Hit");
        slimeParticle.Play();
        animator.Play("SlimeBlobHit", 0, 0f);

        Debug.Log(health);
        UpdateHealthBar();

        //isHit = true;
        yield return null;
        agent.enabled = false;
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.AddForce(force, ForceMode.Impulse);

        yield return new WaitForFixedUpdate();
        //yield return new WaitUntil(() => rb.velocity.magnitude < 0.05f);
        yield return new WaitForSeconds(1f);


        rb.useGravity = false;
        rb.isKinematic = true;
        agent.Warp(transform.position);
        agent.enabled = true;

        yield return null;

        trigger.enabled = true;
        isHit = false;
    }

    void Death()
    {
        isDead = true;
        Instantiate(slimeParticle, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }

    void UpdateHealthBar()
    {
        float fillAmount = (float)health / (float)startHealth;
        healthBar.DOFillAmount(fillAmount, 0.2f);
        healthBar.color = colorGradient.Evaluate(fillAmount);
    }

    private void OnDestroy()
    {
        WeaponController.OnSwordSwing -= WeaponController_OnSwordSwing;
    }
}
