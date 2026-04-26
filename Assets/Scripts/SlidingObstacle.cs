using UnityEngine;

/// Mueve el obstáculo de izquierda a derecha con movimiento sinusoidal.
/// No rota — pensado para árboles u objetos que no deben girar.
[RequireComponent(typeof(Rigidbody))]
public class SlidingObstacle : MonoBehaviour
{
    [SerializeField] float speed     = 1.8f;
    [SerializeField] float amplitude = 4.5f;

    float     phase;
    float     baseY;
    Rigidbody rb;

    void Start()
    {
        rb    = GetComponent<Rigidbody>();
        phase = Random.Range(0f, Mathf.PI * 2f);
        baseY = transform.position.y;
    }

    void FixedUpdate()
    {
        float x = Mathf.Sin(Time.fixedTime * speed + phase) * amplitude;
        rb.MovePosition(new Vector3(x, baseY, transform.position.z));
    }
}
