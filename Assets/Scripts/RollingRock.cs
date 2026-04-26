using UnityEngine;

/// Mueve la roca de izquierda a derecha con movimiento sinusoidal.
/// Usa rb.MovePosition en FixedUpdate — único método que garantiza
/// detección correcta de triggers en Rigidbodies cinemáticos.
[RequireComponent(typeof(Rigidbody))]
public class RollingRock : MonoBehaviour
{
    [SerializeField] float speed     = 3f;
    [SerializeField] float amplitude = 4.5f;

    float     phase;
    float     baseY;
    Rigidbody rb;
    Transform visual;

    void Start()
    {
        rb    = GetComponent<Rigidbody>();
        phase = Random.Range(0f, Mathf.PI * 2f);
        baseY = transform.position.y;

        // Primer hijo que no sea el trigger "T"
        foreach (Transform child in transform)
        {
            if (child.gameObject.name != "T") { visual = child; break; }
        }
    }

    // Movimiento en FixedUpdate con MovePosition → triggers siempre activos
    void FixedUpdate()
    {
        float x = Mathf.Sin(Time.fixedTime * speed + phase) * amplitude;
        rb.MovePosition(new Vector3(x, baseY, transform.position.z));
    }

    // Rotación visual en Update (solo cosmética, no afecta colisión)
    void Update()
    {
        if (visual == null) return;
        float vel = Mathf.Cos(Time.time * speed + phase) * amplitude * speed;
        visual.Rotate(0f, 0f, -vel * Time.deltaTime * 35f, Space.World);
    }
}
