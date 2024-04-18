using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Snowball : MonoBehaviour
{
    PlayerMovementAdvanced playerMovementAdvanced;

    [Header("Snowball Properties")]
    public Rigidbody snowballRb;
    private Vector3 originForce;
    public float rollForce;
    private Animator animator;
    public ParticleSystem dissolve;
    private Vector3 force;
    private bool playerHit;
    private float decreasingValue = 20f;
    private Rigidbody playerRb;
    public int groundLayer;

    [Header("Player Forces")]
    public float pushForceY;
    public float playerPushForce;

    [Header("Sounds")]
    public AudioSource rollingAudioSrc;
    public AudioSource hitPlayerAudioSrc;

    void Start()
    {
        playerMovementAdvanced = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovementAdvanced>();
        animator = GetComponent<Animator>();
        animator.Play("SpawnAnimation");
        snowballRb = GetComponent<Rigidbody>();
        StartCoroutine(PlayAnimationAfterDelay("DespawnAnimation", 13f));
    }
    IEnumerator PlayAnimationAfterDelay(string animationName, float delay)
    {
        yield return new WaitForSeconds(delay);
        animator.Play(animationName);
        dissolve.Play();
        yield return new WaitForSeconds(0.7f);
        dissolve.Stop();
    }

    void FixedUpdate()
    {
        // Apply continous force to snowball, ensuring it moves
        if (snowballRb != null)
        {
            snowballRb.AddForce(originForce * rollForce, ForceMode.Force);
        }

        // Airborne player force
        if (playerHit && decreasingValue > 0)
        {
            float decreaseRate = 6.5f;
            decreasingValue -= decreaseRate * Time.deltaTime;
            playerRb.AddForce(force * decreasingValue, ForceMode.Impulse);
            if (playerMovementAdvanced.IsGrounded())
            {
                playerHit = false;
            }
        }
    }

    // Check if snowball is grounded & play rolling sound
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == groundLayer)
            rollingAudioSrc.DOFade(0.5f, 1f);
            rollingAudioSrc.Play();
    }

    IEnumerator dragAdjustment(float delayUntilNotHit)
    {
        yield return new WaitForSeconds(delayUntilNotHit);
        playerRb.drag = 0.125f;
    }

    public void Initialize(Vector3 force, float size, float rollForce)
    {
        this.force = force;
        originForce = force;
        Debug.Log(force);
        snowballRb.AddForce(force, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerRb = other.GetComponent<Rigidbody>();
            playerHitBySnowball();
            hitPlayerAudioSrc.Play();
        }
    }

    void playerHitBySnowball()
    {
        Vector3 yForce = new Vector3(0, pushForceY, 0);
        playerRb.AddForce(yForce * playerPushForce, ForceMode.Force);
        playerHit = true;
        StartCoroutine(dragAdjustment(2f));
    }
}
