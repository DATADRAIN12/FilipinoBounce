using UnityEngine;

public class PlayerCollisionManager : MonoBehaviour
{
    private PlayerCorePhysics physics;
    private GroundPenaltySystem groundPenalty;
    private BouncePowerSystem bouncePower;

    void Start()
    {
        physics = GetComponent<PlayerCorePhysics>();
        groundPenalty = GetComponent<GroundPenaltySystem>();
        bouncePower = GetComponent<BouncePowerSystem>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            groundPenalty.HandleGroundCollision();
            bouncePower.ReduceConsecutiveBounces();
        }
        else if (collision.gameObject.CompareTag("Bounceable"))
        {
            bouncePower.HandleBounceableCollision();
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            physics.SetGrounded(false);
        }
    }
}