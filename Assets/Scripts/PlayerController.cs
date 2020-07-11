using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]

public class PlayerController : MonoBehaviour
{
    // Move player in 2D space
    public float maxSpeed = 3.4f;
    public float jumpHeight = 6.5f;
    public float gravityScale = 1.5f;
    public KeyCode UserKeyUpPrimary = KeyCode.W;
    public KeyCode UserKeyDownPrimary = KeyCode.S;
    public KeyCode UserKeyLeftPrimary = KeyCode.A;
    public KeyCode UserKeyRightPrimary = KeyCode.D;
    public bool isFacingRight = true;

    float moveDirection = 0;
    bool isGrounded = false;
    Rigidbody2D r2d;
    Collider2D mainCollider;
    // Check every collider except Player and Ignore Raycast
    LayerMask layerMask = ~(1 << 2 | 1 << 8);
    Transform t;
    float jumpSpeed = 0;
    bool chargingJump = false;


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


        if (Input.GetKeyDown(UserKeyUpPrimary) && isGrounded)
        {
            r2d.velocity = new Vector2(r2d.velocity.x, jumpHeight/1.5f);
        }

        if (Input.GetKeyUp(UserKeyDownPrimary) && isGrounded)
        {
            r2d.velocity = new Vector2(r2d.velocity.x, jumpSpeed);
            jumpSpeed = 0;
            chargingJump = false;
        }

        // Jumping
        if (Input.GetKeyDown(UserKeyDownPrimary))
        {
            chargingJump = true;
        }
    }

    void FixedUpdate()
    {
        Bounds colliderBounds = mainCollider.bounds;
        Vector3 groundCheckPos = colliderBounds.min + new Vector3(colliderBounds.size.x * 0.5f, 0.1f, 0);
        // Check if player is grounded
        isGrounded = Physics2D.OverlapCircle(groundCheckPos, 0.23f, layerMask);

        // Apply movement velocity
        r2d.velocity = new Vector2((moveDirection) * maxSpeed, r2d.velocity.y);

        // Simple debug
        Debug.DrawLine(groundCheckPos, groundCheckPos - new Vector3(0, 0.23f, 0), isGrounded ? Color.green : Color.red);

        if(chargingJump)
        {
            if (jumpSpeed < jumpHeight)
            {
                jumpSpeed += 1f;
            }
        }

    }
}