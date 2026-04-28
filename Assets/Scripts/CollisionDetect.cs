using UnityEngine;

public class CollisionDetect : MonoBehaviour
{
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] Animator playerAnim;
    [SerializeField] string obstacleTag = "Obstacle";

    bool  dead            = false;
    float invincibleUntil = 0f;

    void OnTriggerEnter(Collider other)
    {
        if (dead) return;
        if (Time.time < invincibleUntil) return;
        if (!other.CompareTag(obstacleTag)) return;

        PlayerShield shield = playerMovement != null
            ? playerMovement.GetComponent<PlayerShield>()
            : GetComponentInParent<PlayerShield>();

        if (shield != null && shield.IsShielded)
        {
            shield.AbsorbHit();
            invincibleUntil = Time.time + 0.5f;
            return;
        }

        dead = true;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartDeathSequence(playerMovement, playerAnim);
        }
        else
        {
            Debug.LogWarning("[CollisionDetect] No hay GameManager en la escena.");
        }
    }
}
