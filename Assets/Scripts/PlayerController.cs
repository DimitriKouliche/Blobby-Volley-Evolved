using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]

public class PlayerController : MonoBehaviour
{
    // Move player in 2D space
    public float maxSpeed = 3.4f;
    public float jumpHeight = 6.5f;
    public float dashDistance = 6.5f;
    public float gravityScale = 1.5f;
    public KeyCode UserKeyUpPrimary = KeyCode.W;
    public KeyCode UserKeyDownPrimary = KeyCode.S;
    public KeyCode UserKeyLeftPrimary = KeyCode.A;
    public KeyCode UserKeyRightPrimary = KeyCode.D;
    public KeyCode UserKeyDashPrimary = KeyCode.E;
    public bool isFacingRight = true;
    public GameObject gameLogics;

    float moveDirection = 0;
    bool isGrounded = false;
    Rigidbody2D r2d;
    Collider2D mainCollider;
    // Check every collider except Player and Ignore Raycast
    LayerMask layerMask = ~(1 << 2 | 1 << 8);
    Transform t;
    float jumpSpeed = 0;
    bool chargingJump = false;
    bool isDashing = false;


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

    }

    // Update is called once per frame
    void Update()
    {
        if(!gameLogics.GetComponent<GameLogics>().isPlaying)
        {
            return;
        }
        // Movement controls
        if ((Input.GetKey(UserKeyLeftPrimary) || Input.GetKey(UserKeyRightPrimary)))
        {
            moveDirection = Input.GetKey(UserKeyLeftPrimary) ? -1 : 1;
        }
        else
        {
            if (isGrounded || r2d.velocity.magnitude < 0.01f)
            {
                moveDirection = 0;
            }
        }

        //Dash
        if (Input.GetKeyDown(UserKeyDashPrimary) && !isDashing)
        {
            float positionX = 0;
            float positionY = 0;
            if (Input.GetKey(UserKeyLeftPrimary))
            {
                positionX -= dashDistance;
            }
            if (Input.GetKey(UserKeyRightPrimary))
            {
                positionX += dashDistance;
            }
            if (Input.GetKey(UserKeyUpPrimary))
            {
                positionY += dashDistance;
            }
            if (Input.GetKey(UserKeyDownPrimary))
            {
                positionY -= dashDistance;
            }
            if(positionX != 0 || positionY != 0)
            {
                StartCoroutine(DisableDash());
                isDashing = true;
                r2d.AddForce(new Vector3(positionX * 1000, positionY * 1000, transform.position.z));
            }
        }

        // Jumping
        if (Input.GetKeyDown(UserKeyUpPrimary) && isGrounded)
        {
            r2d.velocity = new Vector2(r2d.velocity.x, jumpHeight / 1.5f);
        }

        // Releasing charged jump
        if (Input.GetKeyUp(UserKeyDownPrimary) && isGrounded)
        {
            r2d.velocity = new Vector2(r2d.velocity.x, jumpSpeed);
            jumpSpeed = 0;
            chargingJump = false;
        }

        // Charging jump
        if (Input.GetKeyDown(UserKeyDownPrimary) && isGrounded)
        {
            chargingJump = true;
        }

        // Fast falling
        if (Input.GetKeyDown(UserKeyDownPrimary) && !isGrounded)
        {
            r2d.velocity = new Vector2(r2d.velocity.x, r2d.velocity.y);
        }
    }

    IEnumerator DisableDash()
    {
        yield return new WaitForSeconds(0.3f);
        isDashing = false;
    }

    void FixedUpdate()
    {
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
            }
        }

    }
}