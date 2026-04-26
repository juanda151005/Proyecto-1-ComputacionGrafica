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

    [SerializeField] Animator playerAnim;
    
    void Awake()
    {
        doubleJump = GetComponent<PlayerDoubleJump>();
    }

    void Update()
    {
        // Incrementar la velocidad poco a poco, pero solo hasta el máximo permitido
        playerSpeed += Time.deltaTime * 0.2f; 
        playerSpeed = Mathf.Clamp(playerSpeed, 0, maxPlayerSpeed); 

        // Incrementar la velocidad horizontal poco a poco hasta el límite de 10
        horizontalSpeed += Time.deltaTime * 0.05f; 
        horizontalSpeed = Mathf.Clamp(horizontalSpeed, 0, maxHorizontalSpeed); 

        transform.Translate(Vector3.forward * Time.deltaTime * playerSpeed, Space.World);
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
        {
            if (this.gameObject.transform.position.x > leftLimit){
                transform.Translate(Vector3.left * Time.deltaTime * horizontalSpeed);
            }
        }
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
        {
            if (this.gameObject.transform.position.x < rightLimit){
                transform.Translate(Vector3.left * Time.deltaTime * horizontalSpeed * -1);
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

        // Chequeo muy básico de piso usando la altura configurable
        if (transform.position.y <= groundHeight)
        {
            transform.position = new Vector3(transform.position.x, groundHeight, transform.position.z);
            isGrounded = true;
        }
    }
}
