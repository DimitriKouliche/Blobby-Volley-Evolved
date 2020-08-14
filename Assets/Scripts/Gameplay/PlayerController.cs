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
    public float smashRadius = 20f;
    public GameObject gameLogics;
    public bool isDashing = false;
    public bool isSmashing = false;
    public static GameObject LocalPlayerInstance;
    public GameObject dashRightAnimation;
    public GameObject dashLefttAnimation;
    public GameObject jumpAnimation;
    public GameObject smashAnimation;
    public GameObject smashCollider;
    public Color eyeChargeColor = new Color(233, 208, 118);

    PlayerInput playerInput;
    InputAction moveAction;
    InputAction dashAction;
    InputAction chargeJumpAction;
    InputAction smashAction;
    InputAction startAction;
    float moveDirection = 0;
    bool isGrounded = false;
    Rigidbody2D r2d;
    Collider2D mainCollider;
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
        dashAction = playerInput.actions["Dash"];
        chargeJumpAction = playerInput.actions["Charge Jump"];
        smashAction = playerInput.actions["Smash"];
        startAction = playerInput.actions["Start"];


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

            FindChild(FindChild(gameObject, "SpriteBlob"), "EyesWhite").SetActive(false);
            FindChild(FindChild(gameObject, "SpriteBlob"), "ClosedEyes").SetActive(true);
            smashAnimation.SetActive(true);
            isSmashing = true;
            smashCollider.SetActive(true);
            StartCoroutine(Smash(0.7f));
        };

        // Releasing charged jump
        chargeJumpAction.canceled += ctx =>
        {
            if ((!isGrounded && !IsTouchingPlayer()) || isDashing || !IsPlaying() || r2d == null)
            {
                return;
            }
            r2d.velocity = new Vector2(r2d.velocity.x, jumpSpeed);
            CancelCharge();
        };
    }

    void CancelCharge()
    {
        jumpSpeed = jumpHeight / 2f;
        chargingJump = false;
        FindChild(FindChild(FindChild(gameObject, "SpriteBlob"), "EyesWhite"), "eyes").GetComponent<SpriteRenderer>().color = Color.black;
        FindChild(gameObject, "Charge").SetActive(false);

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
                FindChild(FindChild(gameObject, "SpriteBlob"), "EyesWhite").SetActive(false);
                FindChild(FindChild(gameObject, "SpriteBlob"), "ClosedEyes").SetActive(true);
                isDashing = true;
                CancelCharge();
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

    bool IsTouchingWall()
    {
        return gameObject.GetComponent<Collider2D>().IsTouching(GameObject.Find("Left Wall").GetComponent<Collider2D>()) || gameObject.GetComponent<Collider2D>().IsTouching(GameObject.Find("Right Wall").GetComponent<Collider2D>());
    }
    

    bool IsTouchingPlayer()
    {
        if(transform.position.y > 0)
        {
            return false;
        }
        if (GameObject.Find("Blob 1(Clone)") && gameObject.name != "Blob 1(Clone)" && gameObject.GetComponent<Collider2D>().IsTouching(GameObject.Find("Blob 1(Clone)").GetComponent<Collider2D>()))
        {
            return true;
        }
        if (GameObject.Find("Blob 2(Clone)") && gameObject.name != "Blob 2(Clone)" && gameObject.GetComponent<Collider2D>().IsTouching(GameObject.Find("Blob 2(Clone)").GetComponent<Collider2D>()))
        {
            return true;
        }
        if (GameObject.Find("Blob 3(Clone)") && gameObject.name != "Blob 3(Clone)" && gameObject.GetComponent<Collider2D>().IsTouching(GameObject.Find("Blob 3(Clone)").GetComponent<Collider2D>()))
        {
            return true;
        }
        if (GameObject.Find("Blob 4(Clone)") && gameObject.name != "Blob 4(Clone)" && gameObject.GetComponent<Collider2D>().IsTouching(GameObject.Find("Blob 4(Clone)").GetComponent<Collider2D>()))
        {
            return true;
        }
        return false;
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
        isGrounded = transform.position.y < -6.3;

        // Apply movement velocity
        if (!isDashing && !isSmashing)
        {
            if (moveDirection == 0)
            {
                FindChild(gameObject, "SpriteBlob").transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else if (moveDirection > 0)
            {
                FindChild(gameObject, "SpriteBlob").transform.rotation = Quaternion.Euler(0, 0, -3);
            }
            else
            {
                FindChild(gameObject, "SpriteBlob").transform.rotation = Quaternion.Euler(0, 0, 3);
            }
            r2d.velocity = new Vector2((moveDirection) * maxSpeed, r2d.velocity.y);
        }

        if (chargingJump)
        {
            if (jumpSpeed < jumpHeight)
            {
                jumpSpeed += 0.5f;
                FindChild(FindChild(FindChild(gameObject, "SpriteBlob"), "EyesWhite"), "eyes").GetComponent<EyeLogics>().ChangeEyeColor(jumpSpeed / jumpHeight, Color.black, eyeChargeColor);
            } else
            {
                FindChild(gameObject, "Charge").SetActive(true);
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
                zRotation = Mathf.SmoothStep(startRotation, endRotation, 3 * t / duration);
            }
            else if (t > 2 * duration / 3)
            {
                zRotation = Mathf.SmoothStep(endRotation, startRotation, 3 * t / duration - 2);
            }
            else
            {
                zRotation = rotationX;
            }
            FindChild(gameObject, "SpriteBlob").transform.rotation = Quaternion.Euler(0, 0, zRotation);
            yield return null;
        }
        FindChild(FindChild(gameObject, "SpriteBlob"), "EyesWhite").SetActive(true);
        FindChild(FindChild(gameObject, "SpriteBlob"), "ClosedEyes").SetActive(false);
        isDashing = false;
    }

    IEnumerator Smash(float duration)
    {
        float t = 0.0f;
        int angle;
        if(transform.position.x > 0)
        {
            angle = -30;
        } else
        {
            angle = 30;
        }
        while (t < duration)
        {
            if (t < duration / 2)
            {
                r2d.rotation = Mathf.Lerp(0, angle, 2 * t / duration);
                smashCollider.GetComponent<CapsuleCollider2D>().size = new Vector2(Mathf.SmoothStep(0.0001f, smashRadius, 2 * t / duration), smashCollider.GetComponent<CapsuleCollider2D>().size.y);
            }
            else
            {
                r2d.rotation = Mathf.Lerp(angle, 0, 2 * t / duration - 1);
                smashCollider.GetComponent<CapsuleCollider2D>().size = new Vector2(Mathf.SmoothStep(smashRadius, 0.0001f, 2 * t / duration - 1), smashCollider.GetComponent<CapsuleCollider2D>().size.y);
            }
            t += Time.deltaTime;
            yield return null;
        }
        FindChild(FindChild(gameObject, "SpriteBlob"), "EyesWhite").SetActive(true);
        FindChild(FindChild(gameObject, "SpriteBlob"), "ClosedEyes").SetActive(false);
        smashCollider.SetActive(false);
        isSmashing = false;
    }
    GameObject FindChild(GameObject parent, string name)
    {
        foreach (Transform t in parent.transform)
        {
            if (t.name == name)
            {
                return t.gameObject;
            }
        }
        return null;
    }
}