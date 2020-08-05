using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]

public class PlayerController : MonoBehaviour
{
    public float maxSpeed = 6f;
    public float jumpHeight = 22f;
    public float dashDistance = 3f;
    public float gravityScale = 4f;
    public GameObject gameLogics;
    public bool isDashing = false;
    public bool isSmashing = false;
    public static GameObject LocalPlayerInstance;
    public GameObject dashAnimation;

    PlayerInput playerInput;
    InputAction moveAction;
    InputAction jumpAction;
    InputAction dashAction;
    InputAction chargeJumpAction;
    InputAction smashAction;
    InputAction startAction;
    float moveDirection = 0;
    bool isGrounded = false;
    Rigidbody2D r2d;
    Collider2D mainCollider;
    // Check every collider except Player and Ignore Raycast
    LayerMask layerMask = ~(1 << 2 | 1 << 8);
    float jumpSpeed = 0;
    bool chargingJump = false;
    bool needsSparks = true;


    // Use this for initialization
    void Start()
    {
        r2d = GetComponent<Rigidbody2D>();
        mainCollider = GetComponent<Collider2D>();
        r2d.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        r2d.gravityScale = gravityScale;
        gameObject.layer = 8;
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        dashAction = playerInput.actions["Dash"];
        chargeJumpAction = playerInput.actions["Charge Jump"];
        smashAction = playerInput.actions["Smash"];
        startAction = playerInput.actions["Start"];

        startAction.started += ctx =>
        {
            Debug.Log("starting game");
            gameLogics.GetComponent<GameLogics>().SendStartRoundMessage();
        };

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

        // Smashing
        smashAction.started += ctx =>
        {
            if (isGrounded || isDashing || !gameLogics.GetComponent<GameLogics>().isPlaying)
            {
                return;
            }
            isSmashing = true;
            StartCoroutine(Smash(0.5f));
            GetComponent<CapsuleCollider2D>().offset = new Vector2(1, 0);

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
        if(gameLogics == null)
        {
            return;
        }
        if (!gameLogics.GetComponent<GameLogics>().isPlaying)
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
        if (dashAction.triggered && !isDashing && isGrounded)
        {
            if (moveDirectionVector.x != 0 || moveDirectionVector.y != 0)
            {
                dashAnimation.GetComponent<Animation>().Play();
                gameLogics.GetComponent<GameLogics>().ResetVelocity(gameObject);
                isDashing = true;
                r2d.AddForce(new Vector3(moveDirectionVector.x * dashDistance * 5000, 0, transform.position.z));
                StartCoroutine(DisableDash());
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
        isGrounded = transform.position.y < -3.6;

        // Apply movement velocity
        if(!isDashing)
        {
            r2d.velocity = new Vector2((moveDirection) * maxSpeed, r2d.velocity.y);
        }

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
        yield return new WaitForSeconds(0.7f);
        isDashing = false;
    }

    IEnumerator Smash(float duration)
    {
        float startRotation = transform.eulerAngles.y;
        float endRotation = startRotation + 360.0f;
        float t = 0.0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float zRotation = Mathf.Lerp(startRotation, endRotation, t / duration) % 360.0f;
            Debug.Log(zRotation);
            r2d.rotation = zRotation;
            yield return null;
        }
    }
}