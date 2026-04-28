using UnityEngine;

// Empuja al jugador lateralmente con fuerza creciente mientras esta dentro del trigger.
// Usa LateUpdate para correr DESPUES de PlayerMovement.Update y no ser sobreescrito.
public class WindPushZone : MonoBehaviour
{
    [Tooltip("Velocidad maxima lateral del viento. Positivo = derecha, negativo = izquierda.")]
    [SerializeField] float windStrength = 3.5f;

    [Tooltip("Segundos que tarda el viento en alcanzar su fuerza maxima al entrar.")]
    [SerializeField] float rampTime = 2.5f;

    [Tooltip("Limite X derecho del carril.")]
    [SerializeField] float rightLimit = 5.5f;
    [Tooltip("Limite X izquierdo del carril.")]
    [SerializeField] float leftLimit = -5.5f;

    Transform playerTransform;
    float     timeInside;   // acumula tiempo dentro del trigger para el ramp-up

    void OnTriggerEnter(Collider other)
    {
        if (!IsPlayer(other)) return;
        playerTransform = FindPlayerRoot(other);
        timeInside = 0f;
    }

    void OnTriggerExit(Collider other)
    {
        if (!IsPlayer(other)) return;
        playerTransform = null;
        timeInside = 0f;
    }

    // LateUpdate corre despues de todos los Update: el viento gana sobre el movimiento normal
    void LateUpdate()
    {
        if (playerTransform == null) return;

        // Ramp-up: la fuerza crece de 0 a windStrength en 'rampTime' segundos
        timeInside += Time.deltaTime;
        float t         = Mathf.Clamp01(timeInside / rampTime);
        float curForce  = Mathf.Lerp(0f, windStrength, t);

        float nextX = playerTransform.position.x + curForce * Time.deltaTime;
        nextX = Mathf.Clamp(nextX, leftLimit, rightLimit);

        Vector3 p = playerTransform.position;
        p.x = nextX;
        playerTransform.position = p;
    }

    bool IsPlayer(Collider other)
    {
        if (other.CompareTag("Player")) return true;
        return other.GetComponentInParent<PlayerMovement>() != null;
    }

    // Sube por la jerarquia hasta encontrar el Transform que tiene PlayerMovement
    Transform FindPlayerRoot(Collider other)
    {
        if (other.CompareTag("Player")) return other.transform;
        var pm = other.GetComponentInParent<PlayerMovement>();
        return pm != null ? pm.transform : other.transform;
    }
}
