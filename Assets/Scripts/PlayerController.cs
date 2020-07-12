using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]

public class PlayerController : MonoBehaviour
{
    public InputAction moveAction;
    public InputAction jumpAction;
    public InputAction dashAction;
    public InputAction chargeJumpAction;
    public float maxSpeed = 3.4f;
    public float jumpHeight = 6.5f;
    public float dashDistance = 6.5f;
    public float gravityScale = 1.5f;
    public bool isFacingRight = true;
    public GameObject gameLogics;
    public bool isDashing = false;

    float moveDirection = 0;
    bool isGrounded = false;
    Rigidbody2D r2d;
    Collider2D mainCollider;
    // Check every collider except Player and Ignore Raycast
    LayerMask layerMask = ~(1 << 2 | 1 << 8);
    Transform t;
    float jumpSpeed = 0;
    bool chargingJump = false;
    bool needsSparks = true;


    // Use this for initialization
    void Start()
    {
        t = transform;
        r2d = GetComponent<Rigidbody2D>();
        mainCollider = GetComponent<Collider2D>();
        r2d.freezeRotation = true;
        r2d.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        r2d.gravityScale = gravityScale;
        gameObject.layer = 8;
        moveAction.Enable();
        jumpAction.Enable();
        dashAction.Enable();
        chargeJumpAction.Enable();

        // Jumping
        jumpAction.started += ctx =>
        {
            if(isGrounded && !isDashing && gameLogics.GetComponent<GameLogics>().isPlaying)
            {
                r2d.velocity = new Vector2(r2d.velocity.x, jumpHeight / 1.5f);
            }
        };

        // Charging jump
        chargeJumpAction.started += ctx =>
        {
            if(!isDashing && gameLogics.GetComponent<GameLogics>().isPlaying)
            {
                chargingJump = true;
            }
        };

        // Releasing charged jump
        chargeJumpAction.canceled += ctx =>
        {
            if (!isGrounded || isDashing || !gameLogics.GetComponent<GameLogics>().isPlaying)
            {
                return;
            }
            r2d.velocity = new Vector2(r2d.velocity.x, jumpSpeed);
            jumpSpeed = 0;
            chargingJump = false;
            transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.black;
            transform.GetChild(1).GetComponent<ParticleSystem>().Stop();
            needsSparks = true;
        };
    }

    // Update is called once per frame
    void Update()
    {
        if(!gameLogics.GetComponent<GameLogics>().isPlaying)
        {
            return;
        }

        // Moving
        var moveDirectionVector = moveAction.ReadValue<Vector2>();
        moveDirection = moveDirectionVector.x;

        // Fast falling
        if (moveDirectionVector.y < 0 && !isGrounded)
        {
            r2d.velocity = new Vector2(r2d.velocity.x, r2d.velocity.y);
        }

        // Dashing
        if (dashAction.triggered && !isDashing)
        {
            if (moveDirectionVector.x != 0 || moveDirectionVector.y != 0)
            {
                gameLogics.GetComponent<GameLogics>().ResetVelocity(gameObject);
                StartCoroutine(DisableDash());
                isDashing = true;
                r2d.AddForce(new Vector3(moveDirectionVector.x * dashDistance * 1000, moveDirectionVector.y * dashDistance * 1000, transform.position.z));
            }
        }
    }

    void FixedUpdate()
    {
        if (!gameLogics.GetComponent<GameLogics>().isPlaying)
        {
            return;
        }
        Bounds colliderBounds = mainCollider.bounds;
        Vector3 groundCheckPos = colliderBounds.min + new Vector3(colliderBounds.size.x * 0.5f, 0.1f, 0);
        // Check if player is grounded
        isGrounded = Physics2D.OverlapCircle(groundCheckPos, 0.23f, layerMask);

        // Apply movement velocity
        if(!isDashing)
        {
            r2d.velocity = new Vector2((moveDirection) * maxSpeed, r2d.velocity.y);
        }

        // Simple debug
        Debug.DrawLine(groundCheckPos, groundCheckPos - new Vector3(0, 0.23f, 0), isGrounded ? Color.green : Color.red);

        if (chargingJump)
        {
            if (jumpSpeed < jumpHeight)
            {
                jumpSpeed += 1f;
                transform.GetChild(0).GetComponent<EyeLogics>().ChangeEyeColor(jumpSpeed / jumpHeight, Color.black, Color.cyan);
            } else if (needsSparks)
            {
                transform.GetChild(1).GetComponent<ParticleSystem>().Play();
                needsSparks = false;
            }
        }

    }

    IEnumerator DisableDash()
    {
        yield return new WaitForSeconds(1f);
        isDashing = false;
    }
}