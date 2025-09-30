using UnityEngine;

public class BouncePowerSystem : MonoBehaviour
{
    [Header("Bounce Settings")]
    public float baseBounceForce = 14f;
    public float currentBounceForce;
    public float maxBounceForce = 20f;

    [Header("Progressive Bounceable Boost")]
    public float bounceableBoostAmount = 3f;
    public int maxConsecutiveBounces = 5;

    private PlayerCorePhysics playerPhysics;
    private GroundPenaltySystem groundPenalty;
    private Rigidbody2D rb;
    private int consecutiveBounces = 0;
    private float lastBounceVelocity;

    void Start()
    {
        playerPhysics = GetComponent<PlayerCorePhysics>();
        groundPenalty = GetComponent<GroundPenaltySystem>();
        rb = GetComponent<Rigidbody2D>();
        currentBounceForce = baseBounceForce;
    }

    public void HandleBounceableCollision()
    {
        playerPhysics.SetGrounded(false);

        if (!playerPhysics.IsStartingPhase())
        {
            consecutiveBounces++;
            groundPenalty.ResetGroundPenalty(); // CHANGED: Now calls ResetGroundPenalty instead of ReduceGroundPenalty
        }
        else
        {
            consecutiveBounces = 0;
        }

        lastBounceVelocity = Mathf.Abs(rb.velocity.y);
        CalculateDynamicBounce();
        ApplyBounce();
    }

    private void CalculateDynamicBounce()
    {
        float calculatedBounceForce = baseBounceForce;

        if (!playerPhysics.IsStartingPhase() && consecutiveBounces > 1)
        {
            int effectiveBounces = Mathf.Min(consecutiveBounces - 1, maxConsecutiveBounces);
            float bounceBoost = bounceableBoostAmount * effectiveBounces;
            calculatedBounceForce += bounceBoost;
        }

        calculatedBounceForce = Mathf.Min(calculatedBounceForce, maxBounceForce);
        currentBounceForce = calculatedBounceForce;

        float velocityBonus = Mathf.Clamp(lastBounceVelocity * 0.1f, 0f, maxBounceForce - currentBounceForce);
        currentBounceForce += velocityBonus;
        currentBounceForce = Mathf.Min(currentBounceForce, maxBounceForce);

        Debug.Log($"Bounceable Hit! Consecutive: {consecutiveBounces}, Force: {currentBounceForce}");
    }

    private void ApplyBounce()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.AddForce(Vector2.up * currentBounceForce, ForceMode2D.Impulse);

        if (Mathf.Abs(rb.velocity.x) > 3f)
        {
            rb.velocity = new Vector2(rb.velocity.x * 0.7f, rb.velocity.y);
        }
    }

    public void ReduceConsecutiveBounces()
    {
        if (!playerPhysics.IsStartingPhase() && consecutiveBounces > 0)
        {
            consecutiveBounces = Mathf.Max(0, consecutiveBounces - 1);
        }
    }

    public void ResetConsecutiveBounces() => consecutiveBounces = 0;
    public int GetConsecutiveBounces() => consecutiveBounces;

    public void BoostBounceForce(float boostAmount, float duration)
    {
        StartCoroutine(TemporaryBoost(boostAmount, duration));
    }

    private System.Collections.IEnumerator TemporaryBoost(float boostAmount, float duration)
    {
        float originalBounce = currentBounceForce;
        currentBounceForce = Mathf.Min(currentBounceForce + boostAmount, maxBounceForce);
        yield return new WaitForSeconds(duration);
        currentBounceForce = originalBounce;
    }
}