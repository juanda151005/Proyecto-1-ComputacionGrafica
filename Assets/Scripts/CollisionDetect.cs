using UnityEngine;

public class CollisionDetect : MonoBehaviour
{
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] Animator playerAnim;
    [SerializeField] string obstacleTag = "Obstacle";

    bool  dead            = false;
    float invincibleUntil = 0f;   // tiempo hasta el que el jugador es invencible

    void OnTriggerEnter(Collider other)
    {
        if (dead) return;
        if (Time.time < invincibleUntil) return;   // frames de invencibilidad post-escudo
        if (!other.CompareTag(obstacleTag)) return;

        // Si el jugador tiene escudo, absorbe el golpe y no muere
        PlayerShield shield = playerMovement != null
            ? playerMovement.GetComponent<PlayerShield>()
            : GetComponentInParent<PlayerShield>();
        if (shield != null && shield.IsShielded)
        {
            shield.AbsorbHit();
            invincibleUntil = Time.time + 0.5f;   // 0.5s sin poder morir tras absorber
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
