using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 2f;
    public float jumpForce = 5f;
    public float playerHeight = 1f;

    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody rb;
    private bool isGrounded;
    private float moveX;
    private float moveZ;
    private bool jump;
    private bool jumpEnabled = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        isGrounded = CheckGround();
        MoveAndRotatePlayer();
    }

    private void Update()
    {
        GetInput();
    }

    private void GetInput()
    {
        moveX = Input.GetAxis("Horizontal");
        moveZ = Input.GetAxis("Vertical");
        jump = Input.GetButtonDown("Jump");

        if (jump && isGrounded && jumpEnabled)
        {
            jumpEnabled = false;
            JumpPlayer();
        }
    }

    private bool CheckGround()
    {
        if (Physics.Raycast(transform.position, Vector3.down,  playerHeight + groundCheckRadius, groundLayer))
        {
            return true;
        }
        return false;
    }

    private void MoveAndRotatePlayer()
    {
        Vector3 movement = transform.forward * moveZ * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);

        if (moveX != 0f)
        {
            Quaternion toRotation = Quaternion.Euler(0f, moveX * rotationSpeed, 0f);
            rb.MoveRotation(rb.rotation * toRotation);
        }
    }

    private void JumpPlayer()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        NoiseManager.Instance.MakeNoise(transform.position);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        Invoke("ResetJump", 0.5f);
    }

    private void ResetJump()
    {
        jumpEnabled = true;
    }
}
