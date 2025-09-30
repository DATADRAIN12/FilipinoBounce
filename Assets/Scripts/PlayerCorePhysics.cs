using UnityEngine;

public class PlayerCorePhysics : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D rb;

    [Header("Base Gravity Settings")]
    public float baseGravity = 2f;
    public float tapGravity = 6f;
    public float maxFallSpeed = -25f;

    [Header("BounceMaster-style Height Scaling")]
    public float minFallSpeed = -8f;           // Slowest possible fall speed
    public float maxFallSpeedAtHeight = -30f;  // Fastest possible fall speed
    public float heightForMaxSpeed = 15f;      // Height where max speed is reached
    public AnimationCurve heightFallCurve = new AnimationCurve(
        new Keyframe(0f, 0f),
        new Keyframe(0.3f, 0.1f),
        new Keyframe(0.7f, 0.5f),
        new Keyframe(1f, 1f)
    );

    private bool isGrounded = false;
    private bool isStartingPhase = true;
    private float highestReachedY;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = baseGravity;
        highestReachedY = transform.position.y;
    }

    void Update()
    {
        UpdateHighestHeight();
        HandleGravity();
        ClampFallSpeed();
    }

    void UpdateHighestHeight()
    {
        if (transform.position.y > highestReachedY)
        {
            highestReachedY = transform.position.y;
        }
    }

    public void HandleGravity()
    {
        if (isGrounded)
        {
            rb.gravityScale = baseGravity;
            return;
        }

        // BounceMaster-style: Tap increases gravity, release uses base gravity
        if (Input.GetMouseButton(0))
        {
            rb.gravityScale = tapGravity;
        }
        else
        {
            rb.gravityScale = baseGravity;
        }
    }

    public void ClampFallSpeed()
    {
        float currentMaxFallSpeed = GetCurrentMaxFallSpeed();

        if (rb.velocity.y < currentMaxFallSpeed)
        {
            rb.velocity = new Vector2(rb.velocity.x, currentMaxFallSpeed);
        }
    }

    private float GetCurrentMaxFallSpeed()
    {
        if (isGrounded) return maxFallSpeed;

        // Calculate how high the player has been in this jump
        float currentHeight = highestReachedY - transform.position.y;
        currentHeight = Mathf.Max(0f, currentHeight);

        // Use curve to determine fall speed based on drop height
        float heightRatio = Mathf.Clamp01(currentHeight / heightForMaxSpeed);
        float curveValue = heightFallCurve.Evaluate(heightRatio);

        // Scale between min and max fall speeds
        float scaledFallSpeed = Mathf.Lerp(minFallSpeed, maxFallSpeedAtHeight, curveValue);

        return scaledFallSpeed;
    }

    public void ResetJumpHeight()
    {
        highestReachedY = transform.position.y;
    }

    // Getters and Setters
    public bool IsGrounded() => isGrounded;
    public void SetGrounded(bool grounded)
    {
        isGrounded = grounded;
        if (grounded)
        {
            ResetJumpHeight();
        }
    }

    public bool IsStartingPhase() => isStartingPhase;
    public void SetStartingPhase(bool startingPhase) => isStartingPhase = startingPhase;
}