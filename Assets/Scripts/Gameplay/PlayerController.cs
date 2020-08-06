using System;
using System.Collections;
using Boo.Lang;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]

public class PlayerController : MonoBehaviour
{
    public float maxSpeed = 6f;
    public float jumpHeight = 22f;
    public float dashDistance = 3f;
    public float gravityScale = 4f;
    public float smashRadius = 15f;
    public GameObject gameLogics;
    public bool isDashing = false;
    public bool isSmashing = false;
    public static GameObject LocalPlayerInstance;
    public GameObject dashRightAnimation;
    public GameObject dashLefttAnimation;
    public GameObject jumpAnimation;
    public GameObject smashCollider;

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

    bool IsPlaying()
    {
        return gameLogics == null || gameLogics.GetComponent<GameLogics>().isPlaying;
    }

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
            if (gameLogics != null && gameLogics.GetComponent<GameLogics>().isStarting)
            {
                gameLogics.GetComponent<GameLogics>().SendStartRoundMessage();
            }
        };

        // Jumping
        jumpAction.started += ctx =>
        {
            if (isGrounded && !isDashing && IsPlaying() && jumpAnimation != null)
            {
                jumpAnimation.SetActive(true);
                r2d.velocity = new Vector2(r2d.velocity.x, jumpHeight / 1.5f);

            }

        };

        // Charging jump
        chargeJumpAction.started += ctx =>
        {
            if (!isDashing && IsPlaying())
            {
                chargingJump = true;
            }
        };

        // Smashing
        smashAction.started += ctx =>
        {
            if (this == null || isGrounded || isDashing || isSmashing || !IsPlaying())
            {
                return;
            }
            isSmashing = true;
            StartCoroutine(Smash(0.5f));
        };

        // Releasing charged jump
        chargeJumpAction.canceled += ctx =>
        {
            if (!isGrounded || isDashing || !IsPlaying() || r2d == null)
            {
                return;
            }
            r2d.velocity = new Vector2(r2d.velocity.x, jumpSpeed);
            jumpSpeed = 0;
            chargingJump = false;
            transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().color = Color.black;
            transform.GetChild(1).GetComponent<ParticleSystem>().Play();
        };
    }


    // Update is called once per frame
    void Update()
    {
        if (gameLogics != null && !IsPlaying())
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
            if (moveDirectionVector.x != 0)
            {
                if(gameLogics != null)
                {
                    gameLogics.GetComponent<GameLogics>().ResetVelocity(gameObject);
                }
                isDashing = true;
                r2d.AddForce(new Vector3(moveDirectionVector.x * dashDistance * 5000, 0, transform.position.z));
                if (moveDirectionVector.x > 0)
                {
                    dashLefttAnimation.SetActive(true);
                    StartCoroutine(DisableDash(0.4f, -70f));
                }
                else
                {
                    dashRightAnimation.SetActive(true);
                    StartCoroutine(DisableDash(0.4f, 70f));
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (!IsPlaying())
        {
            return;
        }
        Bounds colliderBounds = mainCollider.bounds;
        Vector3 groundCheckPos = colliderBounds.min + new Vector3(colliderBounds.size.x * 0.5f, 0.1f, 0);
        // Check if player is grounded
        isGrounded = transform.position.y < -3.89;

        // Apply movement velocity
        if (!isDashing)
        {
            if (moveDirection == 0)
            {
                r2d.rotation = 0;
            }
            else if (moveDirection > 0)
            {
                r2d.rotation = -3;
            }
            else
            {
                r2d.rotation = 3;

            }
            r2d.velocity = new Vector2((moveDirection) * maxSpeed, r2d.velocity.y);
        }

        if (chargingJump)
        {
            if (jumpSpeed < jumpHeight)
            {
                jumpSpeed += 1f;
                transform.GetChild(0).GetChild(0).GetComponent<EyeLogics>().ChangeEyeColor(jumpSpeed / jumpHeight, Color.black, Color.cyan);
            }
        }

    }

    IEnumerator DisableDash(float duration, float rotationX)
    {
        float startRotation = transform.eulerAngles.y;
        float endRotation = startRotation + rotationX;
        float t = 0.0f;
        float zRotation;
        while (t < duration)
        {
            t += Time.deltaTime;
            if (t < duration / 3)
            {
                zRotation = Mathf.Lerp(startRotation, endRotation, 3 * t / duration);
            }
            else if (t > 2 * duration / 3)
            {
                zRotation = Mathf.Lerp(endRotation, startRotation, 3 * t / duration - 2);
            }
            else
            {
                zRotation = rotationX;
            }
            r2d.rotation = zRotation;
            yield return null;
        }
        isDashing = false;
    }

    IEnumerator Smash(float duration)
    {
        float t = 0.0f;
        while (t < duration)
        {
            if (t < duration / 2)
            {
                smashCollider.GetComponent<CapsuleCollider2D>().size = new Vector2(Mathf.Lerp(0.0001f, smashRadius, 2 * t / duration), smashCollider.GetComponent<CapsuleCollider2D>().size.y);
            }
            else
            {
                smashCollider.GetComponent<CapsuleCollider2D>().size = new Vector2(Mathf.Lerp(smashRadius, 0.0001f, 2 * t / duration - 1), smashCollider.GetComponent<CapsuleCollider2D>().size.y);
            }
            t += Time.deltaTime;
            yield return null;
        }

        isSmashing = false;
    }
}