using UnityEngine;

public class CollisionDetect : MonoBehaviour
{
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] Animator playerAnim;
    [SerializeField] string obstacleTag = "Obstacle";

    bool dead = false;

    void OnTriggerEnter(Collider other)
    {
        if (dead) return;
        if (!other.CompareTag(obstacleTag)) return;

        // Si el jugador tiene escudo, absorbe el golpe y no muere
        PlayerShield shield = playerMovement != null
            ? playerMovement.GetComponent<PlayerShield>()
            : GetComponentInParent<PlayerShield>();
        if (shield != null && shield.IsShielded)
        {
            shield.AbsorbHit();
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
