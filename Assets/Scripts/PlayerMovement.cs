using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float playerSpeed = 8.5f;
    public float maxPlayerSpeed = 22; // Límite máximo de velocidad
    public float horizontalSpeed = 6;
    public float maxHorizontalSpeed = 10f; // Límite de velocidad horizontal
    public float rightLimit = 5.5f;
    public float leftLimit = -5.5f;

    [Header("Jump Settings")]
    public float jumpForce = 8f;
    public float gravity = -20f;
    public float groundHeight = 0f; // <--- ALTURA PERSONALIZABLE DEL PISO
    private float verticalVelocity = 0;
    private bool  isGrounded   = true;
    private int   airJumpCount = 0;   // saltos usados en el aire

    PlayerDoubleJump doubleJump;

    [Header("Slow Zones")]
    [SerializeField] string slowZoneTag = "Swamp";
    [SerializeField] float swampForwardSpeed = 4f;     // velocidad fija hacia adelante dentro del pantano
    [SerializeField] float swampHorizontalSpeed = 3f;  // velocidad fija lateral dentro del pantano
    [SerializeField] float speedRecoverPerSecond = 6f; // unidades/segundo que sube al salir hasta alcanzar la velocidad real
    private float effSpeed;   // velocidad efectiva hacia adelante
    private float effHoriz;   // velocidad efectiva lateral
    private int slowZoneCount = 0; // soporta superposición de zonas

    [SerializeField] Animator playerAnim;

    void Awake()
    {
        doubleJump = GetComponent<PlayerDoubleJump>();
        // Inicializar para que antes de tocar un pantano corra a su velocidad real.
        effSpeed = playerSpeed;
        effHoriz = horizontalSpeed;
    }

    void Update()
    {
        // Incrementar la velocidad poco a poco, pero solo hasta el máximo permitido
        playerSpeed += Time.deltaTime * 0.1f;
        playerSpeed = Mathf.Clamp(playerSpeed, 0, maxPlayerSpeed);

        // Incrementar la velocidad horizontal poco a poco hasta el límite de 10
        horizontalSpeed += Time.deltaTime * 0.03f;
        horizontalSpeed = Mathf.Clamp(horizontalSpeed, 0, maxHorizontalSpeed);

        // --- Velocidad efectiva por zona lenta (pantano) ---
        // Dentro: velocidad fija (no depende de la velocidad real acumulada).
        // Fuera: sube gradualmente hasta alcanzar la velocidad real actual.
        if (slowZoneCount > 0)
        {
            effSpeed = swampForwardSpeed;
            effHoriz = swampHorizontalSpeed;
        }
        else
        {
            effSpeed = Mathf.MoveTowards(effSpeed, playerSpeed, speedRecoverPerSecond * Time.deltaTime);
            effHoriz = Mathf.MoveTowards(effHoriz, horizontalSpeed, speedRecoverPerSecond * Time.deltaTime);
        }

        transform.Translate(Vector3.forward * Time.deltaTime * effSpeed, Space.World);
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
        {
            if (this.gameObject.transform.position.x > leftLimit){
                transform.Translate(Vector3.left * Time.deltaTime * effHoriz);
            }
        }
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
        {
            if (this.gameObject.transform.position.x < rightLimit){
                transform.Translate(Vector3.left * Time.deltaTime * effHoriz * -1);
            }
        }

        // --- Lógica de Salto y Gravedad ---
        bool jumpPressed = Keyboard.current.spaceKey.wasPressedThisFrame ||
                           Keyboard.current.upArrowKey.wasPressedThisFrame;

        if (isGrounded)
        {
            airJumpCount     = 0;
            verticalVelocity = -1f;
            if (jumpPressed)
            {
                verticalVelocity = jumpForce;
                isGrounded       = false;
                if (playerAnim != null) playerAnim.Play("Jump");
            }
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;

            // Doble salto en el aire si el poder está activo
            if (jumpPressed && airJumpCount < 1 && doubleJump != null && doubleJump.IsActive)
            {
                verticalVelocity = jumpForce;
                airJumpCount++;
                if (playerAnim != null) playerAnim.Play("Jump");
            }
        }

        // Aplicar movimiento vertical
        transform.Translate(new Vector3(0, verticalVelocity * Time.deltaTime, 0), Space.World);

        // Chequeo de piso
        if (transform.position.y <= groundHeight)
        {
            transform.position = new Vector3(transform.position.x, groundHeight, transform.position.z);
            isGrounded = true;
        }
    }

    // --- Detección de zonas lentas (pantano, etc.) ---
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(slowZoneTag)) slowZoneCount++;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(slowZoneTag))
            slowZoneCount = Mathf.Max(0, slowZoneCount - 1);
    }
}
