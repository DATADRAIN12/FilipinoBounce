using UnityEngine;
using System.Collections;

public class GroundPenaltySystem : MonoBehaviour
{
    [Header("Ground Settings")]
    public float groundStickForce = 5f;
    public float groundDrag = 2f;
    public float startingBouncyPhaseTime = 5f;
    public float startingGroundBounceForce = 10f;

    [Header("Step-based Penalty System")]
    public int maxGroundTouches = 3;
    public float[] groundBounceForces = new float[] { 8f, 4f, 2f, 0f };

    private PlayerCorePhysics playerPhysics;
    private Rigidbody2D rb;
    private int groundTouchCount = 0;
    private bool hasStartedBouncing = false;

    void Start()
    {
        playerPhysics = GetComponent<PlayerCorePhysics>();
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(StartingBouncyPhase());
    }

    public void HandleGroundCollision()
    {
        playerPhysics.SetGrounded(true);

        if (playerPhysics.IsStartingPhase())
        {
            HandleStartingPhaseBounce();
        }
        else if (hasStartedBouncing)
        {
            HandleStepBasedGroundPenalty();
        }
        else if (!playerPhysics.IsStartingPhase())
        {
            HandleStepBasedGroundPenalty();
        }
    }

    private void HandleStartingPhaseBounce()
    {
        playerPhysics.SetGrounded(false);
        hasStartedBouncing = true;

        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.AddForce(Vector2.up * startingGroundBounceForce, ForceMode2D.Impulse);
    }

    private void HandleStepBasedGroundPenalty()
    {
        groundTouchCount = Mathf.Min(groundTouchCount + 1, maxGroundTouches);

        int forceIndex = Mathf.Clamp(groundTouchCount - 1, 0, groundBounceForces.Length - 1);
        float bounceForce = groundBounceForces[forceIndex];

        if (bounceForce > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            rb.AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse);
        }
        else
        {
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0f;
        }

        Debug.Log($"Ground Touch {groundTouchCount}/{maxGroundTouches}! Bounce Force: {bounceForce}");
    }

    // FIXED: This method now resets ground penalty completely
    public void ResetGroundPenalty()
    {
        if (!playerPhysics.IsStartingPhase())
        {
            groundTouchCount = 0; // Reset to 0 instead of just reducing by 1
            Debug.Log($"Bounceable reset ground penalty! Ground touches now: {groundTouchCount}");
        }
    }

    public void ResetGroundCounter() => groundTouchCount = 0;
    public int GetGroundTouchCount() => groundTouchCount;

    private IEnumerator StartingBouncyPhase()
    {
        yield return new WaitForSeconds(startingBouncyPhaseTime);
        playerPhysics.SetStartingPhase(false);
    }
}