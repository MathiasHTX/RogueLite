using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public class PlayerMovementAdvanced : MonoBehaviour
{
    public static PlayerMovementAdvanced instance { get; private set; }

    // Health
    [SerializeField] int startHealth = 200;
    [SerializeField] int healthDamage = 10;
    int health;
    bool isHit;
    [SerializeField] int hitCooldown = 2;
    public static event Action<int> onPlayerHit;
    bool isDead;

    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;

    public float groundDrag;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;
    int jumpCount;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    [Header("Dashing")]
    public float dashSpeed;
    public float dashDuration = 0.2f; // Duration of the dash in seconds
    public float dashCooldown = 2f; // Cooldown duration in seconds
    private float doubleClickTime = 0.2f; // Window for double click in seconds
    private Dictionary<KeyCode, float> lastKeyPressTime = new Dictionary<KeyCode, float> {
        {KeyCode.W, -1f},
        {KeyCode.A, -1f},
        {KeyCode.S, -1f},
        {KeyCode.D, -1f}
    };
    private bool isDashing;
    private Vector3 dashDirection;
    private float lastDashTime = -1f; // Track the last time dash was used

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;


    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    float slopeAngle;

    public MovementState state;
    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;

        startYScale = transform.localScale.y;

        health = startHealth;
    }

    private void Update()
    {
        // Adjusted ground check
        float extraHeight = grounded ? 0.2f : 0.5f; // Extend the check further if not grounded
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + extraHeight, whatIsGround);

        MyInput();
        SpeedControl();
        StateHandler();

        // handle drag
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 1.2f;

        // Check for double-click and initiate dash if detected
        CheckForDashInput();
    }

    private void FixedUpdate()
    {
        MovePlayer();

        // Apply dash force if dashing
        if (isDashing)
        {
            rb.AddForce(dashDirection * dashSpeed, ForceMode.VelocityChange);
        }
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // when to jump
        if (Input.GetKey(jumpKey) && readyToJump && grounded && jumpCount < 2)
        {
            readyToJump = false;

            jumpCount += 1;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (Input.GetKeyUp(jumpKey))
        {
            jumpCount = 0;
        }

        // start crouch
        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        // stop crouch
        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    private void StateHandler()
    {
        // Mode - Crouching
        if (Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }

        // Mode - Sprinting
        else if (grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }

        // Mode - Walking
        else if (grounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }

        // Mode - Air
        else
        {
            state = MovementState.air;
        }
    }

    private void MovePlayer()
    {
        // Calculate movement direction based on player input
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (OnSlope() && !exitingSlope)
        {
            // Apply movement force adjusted for slope
            Vector3 adjustedMoveDirection = AdjustMoveDirectionOnSlope(moveDirection, slopeHit.normal);
            rb.AddForce(adjustedMoveDirection * moveSpeed * 10f, ForceMode.Force);

            // Directly manage velocity to prevent unintended movement
            ManageSlopeVelocity();

            // Apply braking force if there is no input to prevent sliding
            if (moveDirection == Vector3.zero)
            {
                ApplyBrakingForce();
            }
        }
        else
        {
            // Apply standard movement force
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * (grounded ? 1f : airMultiplier), ForceMode.Force);
        }

        rb.useGravity = !OnSlope();
    }

    private void ApplyBrakingForce()
    {
        // Check if the player is grounded to apply braking forces
        if (grounded)
        {
            // Calculate the direction of the slope by projecting the global up vector onto the plane of the slope.
            Vector3 slopeDirection = Vector3.ProjectOnPlane(Vector3.up, slopeHit.normal).normalized;

            // Calculate the right vector relative to the slope and player orientation.
            Vector3 slopeRight = Vector3.Cross(slopeDirection, slopeHit.normal).normalized;

            // Project the current velocity onto the slope's right and forward vectors to get the sideways and upward/downward velocity components.
            Vector3 sidewaysVelocity = Vector3.Project(rb.velocity, slopeRight);
            Vector3 upDownVelocity = Vector3.Project(rb.velocity, slopeDirection);

            // Apply braking force against the sideways velocity to prevent sliding sideways.
            rb.AddForce(-sidewaysVelocity * 10f, ForceMode.Force);

            // If there is no vertical input and the player is on a slope, apply a braking force to counteract sliding up or down the slope.
            if (verticalInput == 0 && OnSlope())
            {
                // Apply a gentle force against the slope's direction based on the player's down slope velocity
                Vector3 counteractingForce = -upDownVelocity * 10f * rb.mass;
                rb.AddForce(counteractingForce, ForceMode.Force);
            }
        }
    }

    private Vector3 AdjustMoveDirectionOnSlope(Vector3 moveDirection, Vector3 slopeNormal)
    {
        // Project the move direction onto the plane defined by the slope normal
        return Vector3.ProjectOnPlane(moveDirection, slopeNormal).normalized;
    }

    private void ManageSlopeVelocity()
    {
        Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        // Check if the player's horizontal velocity exceeds the intended move speed
        if (horizontalVelocity.magnitude > moveSpeed)
        {
            rb.velocity = new Vector3(horizontalVelocity.normalized.x * moveSpeed, rb.velocity.y, horizontalVelocity.normalized.z * moveSpeed);
        }

        // Slightly adjust the player's vertical velocity to counteract downward drift
        if (!grounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, Mathf.Max(rb.velocity.y, -0.5f), rb.velocity.z); // Limit the downward velocity
        }
    }

    private float CalculateSlopeSpeedAdjustment(Vector3 moveDirection)
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
            // Adjust the formula to make the player move faster on the slope, but less aggressively
            float speedAdjustmentFactor = Mathf.Cos(slopeAngle * Mathf.Deg2Rad);

            // Adjust the factor to increase speed on slopes, but ensure it's not overly aggressive
            // Here, instead of reducing the speed, we slightly increase it, but ensure it's within a reasonable range
            speedAdjustmentFactor = Mathf.Clamp(speedAdjustmentFactor, 1.3f, 1.7f); // Adjust these values as needed

            return speedAdjustmentFactor;
        }

        return 1f; // No adjustment needed if not on a slope
    }


    private void SpeedControl()
    {
        // limiting speed on slope
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }

        // limiting speed on ground or in air
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // limit velocity if needed
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    private void Jump()
    {
        exitingSlope = true;

        // Calculate the adjustment factor based on the slope angle
        float slopeAdjustmentFactor = CalculateJumpForceAdjustmentOnSlope();

        Debug.Log(slopeAngle);
        if (slopeAngle < 45)
        {
            // reset y velocity
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // Apply adjusted jump force
            rb.AddForce(transform.up * jumpForce * slopeAdjustmentFactor, ForceMode.Impulse);
        }
    }

    private float CalculateJumpForceAdjustmentOnSlope()
    {
        if (OnSlope())
        {
            float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);

            // Apply a more aggressive reduction to the jump force as the slope angle increases
            // Adjust the exponent or coefficients in this formula to change how much the slope affects the jump
            float adjustmentFactor = Mathf.Clamp01(1 - Mathf.Pow(slopeAngle / 90f, 2)); // Using a quadratic adjustment for a steeper decrease

            // Ensure the jump force is slightly reduced even on small slopes
            // This factor can be adjusted to ensure even on slight slopes the jump is noticeably smaller
            adjustmentFactor *= 0.9f; // Further reduce the jump force on any slope

            return adjustmentFactor;
        }

        return 1f; // No adjustment needed on flat ground
    }

    private void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }

    private void CheckForDashInput()
    {
        if (Time.time - lastDashTime < dashCooldown)
        {
            // Dash is on cooldown, do not initiate another dash
            return;
        }

        foreach (var key in lastKeyPressTime.Keys.ToList())
        {
            if (Input.GetKeyDown(key))
            {
                if (Time.time - lastKeyPressTime[key] <= doubleClickTime)
                {
                    // Double click detected, initiate dash
                    InitiateDash(key);
                }

                // Update last key press time
                lastKeyPressTime[key] = Time.time;
            }
        }
    }

    private void InitiateDash(KeyCode key)
    {
        if (Time.time - lastDashTime < dashCooldown)
        {
            // Dash is still on cooldown
            return;
        }

        Vector3 direction = Vector3.zero;
        switch (key)
        {
            case KeyCode.W: direction = orientation.forward; break;
            case KeyCode.A: direction = -orientation.right; break;
            case KeyCode.S: direction = -orientation.forward; break;
            case KeyCode.D: direction = orientation.right; break;
        }

        isDashing = true;
        dashDirection = direction;
        lastDashTime = Time.time; // Update the last dash time to the current time

        Invoke("StopDashing", dashDuration);
    }

    private void StopDashing()
    {
        isDashing = false;
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.5f))
        {
            slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return slopeAngle < maxSlopeAngle && slopeAngle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    public bool IsGrounded()
    {
        return grounded;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (!isHit && !isDead)
            {
                health -= healthDamage;
                isHit = true;
                onPlayerHit?.Invoke(health);
                StartCoroutine(HitCooldown());

                if (health <= 0)
                {
                    Death();
                }
            }
        }
    }

    IEnumerator HitCooldown()
    {
        yield return new WaitForSeconds(hitCooldown);
        isHit = false;
    }

    void Death()
    {
        isDead = true;
        UIManager.instance.GameOver();
    }

    public bool IsDead()
    {
        return isDead;
    }

    public int GetStartHealth()
    {
        return startHealth;
    }
}