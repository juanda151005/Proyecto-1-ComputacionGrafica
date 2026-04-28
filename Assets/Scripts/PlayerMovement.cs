using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float playerSpeed    = 8.5f;
    public float maxPlayerSpeed = 22;
    public float horizontalSpeed    = 6;
    public float maxHorizontalSpeed = 10f;
    public float rightLimit = 5.5f;
    public float leftLimit  = -5.5f;

    [Header("Jump Settings")]
    public float jumpForce    = 8f;
    public float gravity      = -20f;
    public float groundHeight = 0f;
    private float verticalVelocity = 0;
    private bool  isGrounded       = true;
    private int   airJumpCount     = 0;

    PlayerDoubleJump doubleJump;

    [Header("Slow Zones")]
    [SerializeField] string slowZoneTag         = "Swamp";
    [SerializeField] float swampForwardSpeed    = 4f;
    [SerializeField] float swampHorizontalSpeed = 3f;
    [SerializeField] float speedRecoverPerSecond = 6f;
    private float effSpeed;
    private float effHoriz;
    private int slowZoneCount = 0;

    [SerializeField] Animator playerAnim;

    void Awake()
    {
        doubleJump = GetComponent<PlayerDoubleJump>();
        effSpeed = playerSpeed;
        effHoriz = horizontalSpeed;
    }

    void Update()
    {
        playerSpeed    += Time.deltaTime * 0.1f;
        playerSpeed     = Mathf.Clamp(playerSpeed, 0, maxPlayerSpeed);

        horizontalSpeed += Time.deltaTime * 0.03f;
        horizontalSpeed  = Mathf.Clamp(horizontalSpeed, 0, maxHorizontalSpeed);

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
            if (this.gameObject.transform.position.x > leftLimit)
                transform.Translate(Vector3.left * Time.deltaTime * effHoriz);
        }
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
        {
            if (this.gameObject.transform.position.x < rightLimit)
                transform.Translate(Vector3.left * Time.deltaTime * effHoriz * -1);
        }

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

            if (jumpPressed && airJumpCount < 1 && doubleJump != null && doubleJump.IsActive)
            {
                verticalVelocity = jumpForce;
                airJumpCount++;
                if (playerAnim != null) playerAnim.Play("Jump");
            }
        }

        transform.Translate(new Vector3(0, verticalVelocity * Time.deltaTime, 0), Space.World);

        if (transform.position.y <= groundHeight)
        {
            transform.position = new Vector3(transform.position.x, groundHeight, transform.position.z);
            isGrounded = true;
        }
    }

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
