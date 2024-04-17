using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Snowball : MonoBehaviour
{
    public Rigidbody snowballRb;
    private Vector3 originForce;
    public float rollForce;
    private Animator animator;
    public ParticleSystem dissolve;
    private Vector3 force;
    private bool playerHit;
    private float decreasingValue = 15f;
    Rigidbody playerRb;

    public float pushForceY;
    public float playerPushForce;

    void Start()
    {
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

    void Update()
    {
        if (snowballRb != null)
        {
            snowballRb.AddForce(originForce * rollForce, ForceMode.Force);
        }

        if (playerHit && decreasingValue > 0)
        {
            float decreaseRate = 6.5f;
            decreasingValue -= decreaseRate * Time.deltaTime;
            playerRb.AddForce(force * decreasingValue, ForceMode.Impulse);
            if (PlayerMovementAdvanced.instance.IsGrounded())
            {
                playerHit = false;
            }
        }
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
            Vector3 yForce = new Vector3(0, pushForceY, 0);
            playerRb.AddForce(yForce * playerPushForce, ForceMode.Force);
            playerHit = true;
            StartCoroutine(dragAdjustment(2f));
        }
    }
}
