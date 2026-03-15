using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float playerSpeed = 7;
    public float horizontalSpeed = 5;
    public float rightLimit = 5.5f;
    public float leftLimit = -5.5f;
    
    [Header("Jump Settings")]
    public float jumpForce = 8f;
    public float gravity = -20f;
    public float groundHeight = 0f; // <--- ALTURA PERSONALIZABLE DEL PISO
    private float verticalVelocity = 0;
    private bool isGrounded = true;

    [SerializeField] Animator playerAnim;
    
    void Update()
    {
        // Incrementar la velocidad poco a poco (dificultad progresiva)
        playerSpeed += Time.deltaTime * 0.2f; 

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
        if (isGrounded)
        {
            verticalVelocity = -1f; // Pequeña fuerza hacia abajo continua para mantenerlo en el piso
            if (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.upArrowKey.wasPressedThisFrame)
            {
                verticalVelocity = jumpForce;
                isGrounded = false;
                if (playerAnim != null) playerAnim.Play("Jump"); // Activar Animación
            }
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime; // Aplicar gravedad
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
