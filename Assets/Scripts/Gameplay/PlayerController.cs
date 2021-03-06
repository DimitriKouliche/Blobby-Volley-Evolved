﻿using System;
using System.Collections;
using System.IO.IsolatedStorage;
using Steamworks;
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
    public float bumpRadius = 20f;
    public GameObject gameLogics;
    public bool isDashing = false;
    public bool isSmashing = false;
    public bool isBumping = false;
    public static GameObject LocalPlayerInstance;
    public GameObject dashRightAnimation;
    public GameObject dashLefttAnimation;
    public GameObject jumpAnimation;
    public GameObject wallJumpAnimation;
    public GameObject smashAnimation;
    public GameObject bumpAnimation;
    public GameObject smashAnimationWhite;
    public GameObject bumpAnimationWhite;
    public GameObject smashCollider;
    public GameObject bumpCollider;
    public Color eyeChargeColor = new Color(233, 208, 118);

    PlayerInput playerInput;
    InputAction moveAction;
    InputAction dashAction;
    InputAction chargeJumpAction;
    InputAction smashAction;
    InputAction bumpAction;
    InputAction startAction;
    float moveDirection = 0;
    bool isGrounded = false;
    Rigidbody2D r2d;
    Collider2D mainCollider;
    float jumpSpeed = 0;
    bool chargingJump = false;
    PlayerSounds playerSounds;
    Vector2 moveDirectionVector;
    bool isJumpDashing = false;
    bool hasWallJumped = false;
    bool isWallJumping = false;

    bool IsPlaying()
    {
        if (this == null || !gameObject.activeSelf)
        {
            return false;
        }
        return (gameLogics == null || gameLogics.GetComponent<GameLogics>().isPlaying) && !FindChild(GameObject.Find("UI"), "MenuContent").activeSelf;
    }

    public void PauseGame()
    {
        if (isSmashing || !IsPlaying() || this == null || !gameLogics.GetComponent<GameLogics>().CanPauseGame())
        {
            return;
        }
        gameLogics.GetComponent<GameLogics>().isPaused = true;
        Time.timeScale = 0;
        FindChild(GameObject.Find("UI"), "MenuContent").GetComponent<PauseMenuController>().playerInput = playerInput;
        FindChild(GameObject.Find("UI"), "MenuContent").SetActive(true);
    }

    // Use this for initialization
    void Start()
    {
        playerSounds = GetComponent<PlayerSounds>();
        jumpSpeed = jumpHeight / 1.8f;
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
        bumpAction = playerInput.actions["Bump"];
        startAction = playerInput.actions["Start"];


        startAction.started += ctx =>
        {
            PauseGame();
        };


        // Charging jump
        chargeJumpAction.started += ctx =>
        {
            if (this == null)
            {
                return;
            }
            if (IsTouchingWall() && !isGrounded && !hasWallJumped)
            {
                if(transform.position.x > 0)
                {
                    r2d.velocity = new Vector2(-jumpHeight * 1.2f, jumpHeight * 0.8f);
                }
                if(transform.position.x < 0)
                {
                    r2d.velocity = new Vector2(jumpHeight * 1.2f, jumpHeight * 0.8f);
                }
                hasWallJumped = true;
                isWallJumping = true;
                StartCoroutine(DisableWallJump());
                FindChild(gameObject, "WallJump").SetActive(true);
                return;
            }
            if (IsPlaying() && Time.timeScale != 0)
            {
                chargingJump = true;
            }
        };

        // Smashing
        smashAction.started += ctx =>
        {
            if (this == null || isGrounded || isDashing || isSmashing || !IsPlaying() || Time.timeScale == 0)
            {
                return;
            }
            playerSounds.SmashSound();
            if(FindChild(FindChild(gameObject, "SpriteBlob"), "EyesWhite").activeSelf)
            {
                FindChild(FindChild(gameObject, "SpriteBlob"), "EyesWhite").SetActive(false);
                FindChild(FindChild(gameObject, "SpriteBlob"), "ClosedEyes").SetActive(true);
            }
            smashAnimation.SetActive(true);
            smashAnimationWhite.SetActive(true);
            isSmashing = true;
            smashCollider.SetActive(true);
            StartCoroutine(Smash(0.7f));
            PlayerPrefs.SetInt("hasSmashed", 1);
            SkillAchievement();
        };

        // Bumping
        bumpAction.started += ctx =>
        {
            int invert = 1;
            if (this == null || isDashing || isBumping || !IsPlaying() || Time.timeScale == 0)
            {
                return;
            }
            var moveDirectionVector = moveAction.ReadValue<Vector2>();
            moveDirection = moveDirectionVector.x;
            playerSounds.BumpSound();
            bumpAnimation.SetActive(true);
            bumpAnimationWhite.SetActive(true);
            bumpCollider.SetActive(true);
            isBumping = true;
            StartCoroutine(Bump(0.5f, invert));
            PlayerPrefs.SetInt("hasBumped", 1);
            SkillAchievement();
        };

        // Releasing charged jump
        chargeJumpAction.canceled += ctx =>
        {
            Jump();
            PlayerPrefs.SetInt("hasJumped", 1);
            SkillAchievement();
        };

        // Dashing
        dashAction.started += ctx =>
    {
        if (this == null || !isGrounded || isDashing || !IsPlaying() || Time.timeScale == 0)
        {
            return;
        }
        if (moveDirectionVector.x != 0)
        {
            if (gameLogics != null)
            {
                gameLogics.GetComponent<GameLogics>().ResetVelocity(gameObject);
            }
            if (FindChild(FindChild(gameObject, "SpriteBlob"), "EyesWhite").activeSelf)
            {
                FindChild(FindChild(gameObject, "SpriteBlob"), "EyesWhite").SetActive(false);
                FindChild(FindChild(gameObject, "SpriteBlob"), "ClosedEyes").SetActive(true);
            }
            isDashing = true;
            r2d.rotation = 0;
            playerSounds.DashSound();
            r2d.AddForce(new Vector3(moveDirectionVector.x * dashDistance * 5000, 0, transform.position.z));
            if (moveDirectionVector.x > 0)
            {
                dashLefttAnimation.SetActive(true);
                StartCoroutine(DisableDash(0.45f, -70f));
            }
            else
            {
                dashRightAnimation.SetActive(true);
                StartCoroutine(DisableDash(0.45f, 70f));
            }
            PlayerPrefs.SetInt("hasDashed", 1);
            SkillAchievement();
        }
    };
    }

    void Jump()
    {
        if ((!isGrounded && !IsTouchingPlayer()) || !IsPlaying() || r2d == null || Time.timeScale == 0 || !chargingJump)
        {
            CancelCharge();
            return;
        }
        if (isDashing)
        {
            isJumpDashing = true;
            isDashing = false;
            StartCoroutine(DisableJumpDash(0.3f));
        }
        playerSounds.JumpSound();
        FindChild(gameObject, "Jump").SetActive(true);
        r2d.velocity = new Vector2(r2d.velocity.x, jumpSpeed);
        CancelCharge();
    }

    void SkillAchievement()
    {
        if (PlayerPrefs.GetInt("hasSmashed", 0) == 1 && PlayerPrefs.GetInt("hasBumped", 0) == 1 &&
            PlayerPrefs.GetInt("hasJumped", 0) == 1 && PlayerPrefs.GetInt("hasDashed", 0) == 1)
        {
            if (SteamManager.Initialized)
            {
                SteamUserStats.SetAchievement("SKILLED");
                SteamUserStats.StoreStats();
            }
        }
    }

    public void CancelCharge()
    {
        if(this == null)
        {
            return;
        }
        jumpSpeed = jumpHeight / 2.5f;
        chargingJump = false;
        FindChild(FindChild(FindChild(gameObject, "SpriteBlob"), "EyesWhite"), "eyes").GetComponent<SpriteRenderer>().color = Color.black;
        FindChild(gameObject, "Charge").SetActive(false);

    }


    // Update is called once per frame
    void Update()
    {
        if (gameLogics != null && !IsPlaying() || this == null)
        {
            return;
        }

        if (Time.timeScale == 0)
        {
            return;
        }

        // Moving
        moveDirectionVector = moveAction.ReadValue<Vector2>();
        moveDirection = moveDirectionVector.x;

        // Fast falling
        if (moveDirectionVector.y < -0.7 && !isGrounded && !isSmashing)
        {
            r2d.velocity = new Vector2(r2d.velocity.x, r2d.velocity.y - 0.2f);
        }
    }

    bool IsTouchingWall()
    {
        return gameObject.GetComponent<Collider2D>().IsTouching(GameObject.Find("Left Wall Jump").GetComponent<Collider2D>()) || gameObject.GetComponent<Collider2D>().IsTouching(GameObject.Find("Right Wall Jump").GetComponent<Collider2D>());
    }


    bool IsTouchingPlayer()
    {
        if (transform.position.y > 0)
        {
            return false;
        }
        if (GameObject.Find("Blob 1(Clone)") && gameObject.name != "Blob 1(Clone)" && gameObject.GetComponent<CapsuleCollider2D>().IsTouching(GameObject.Find("Blob 1(Clone)").GetComponent<CircleCollider2D>()))
        {
            return true;
        }
        if (GameObject.Find("Blob 2(Clone)") && gameObject.name != "Blob 2(Clone)" && gameObject.GetComponent<CapsuleCollider2D>().IsTouching(GameObject.Find("Blob 2(Clone)").GetComponent<CircleCollider2D>()))
        {
            return true;
        }
        if (GameObject.Find("Blob 3(Clone)") && gameObject.name != "Blob 3(Clone)" && gameObject.GetComponent<CapsuleCollider2D>().IsTouching(GameObject.Find("Blob 3(Clone)").GetComponent<CircleCollider2D>()))
        {
            return true;
        }
        if (GameObject.Find("Blob 4(Clone)") && gameObject.name != "Blob 4(Clone)" && gameObject.GetComponent<CapsuleCollider2D>().IsTouching(GameObject.Find("Blob 4(Clone)").GetComponent<CircleCollider2D>()))
        {
            return true;
        }
        if (GameObject.Find("Cup") && gameObject.GetComponent<CapsuleCollider2D>().IsTouching(GameObject.Find("Cup").GetComponent<EdgeCollider2D>()))
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
        isGrounded = transform.position.y < -6f;

        if (isGrounded)
        {
            isJumpDashing = false;
            hasWallJumped = false;
        }

        // Apply movement velocity
        if (!isDashing && !isSmashing && !isWallJumping)
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
            if (isJumpDashing)
            {
                r2d.velocity = new Vector2((moveDirection) * maxSpeed * 3, r2d.velocity.y);
            }
            else
            {
                r2d.velocity = new Vector2((moveDirection) * maxSpeed, r2d.velocity.y);
            }
        }

        if (isDashing)
        {
            r2d.velocity = new Vector2(r2d.velocity.x, 0);
        }

        if (isSmashing && !isDashing)
        {
            r2d.velocity = new Vector2((moveDirection / 2) * maxSpeed, r2d.velocity.y);
        }

        if (chargingJump)
        {
            if (jumpSpeed < jumpHeight)
            {
                jumpSpeed += 7f;
                if (Time.timeScale < 0.1f && Time.timeScale > 0.01f)
                {
                    jumpSpeed = jumpHeight;
                }
                FindChild(FindChild(FindChild(gameObject, "SpriteBlob"), "EyesWhite"), "eyes").GetComponent<EyeLogics>().ChangeEyeColor(jumpSpeed / jumpHeight, Color.black, eyeChargeColor);
            }
            else
            {
                FindChild(gameObject, "Charge").SetActive(true);
                Jump();
            }
        }
        if (IsTouchingWall() && !isGrounded && !hasWallJumped)
        {
            float rotationX = -40;
            if (transform.position.x > 0)
            {
                rotationX = 40;
            }
            FindChild(gameObject, "SpriteBlob").transform.rotation = Quaternion.Euler(0, 0, rotationX);
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
        if (FindChild(FindChild(gameObject, "SpriteBlob"), "ClosedEyes").activeSelf)
        {
            FindChild(FindChild(gameObject, "SpriteBlob"), "EyesWhite").SetActive(true);
            FindChild(FindChild(gameObject, "SpriteBlob"), "ClosedEyes").SetActive(false);
        }
        isDashing = false;
    }
    IEnumerator DisableJumpDash(float duration)
    {
        yield return new WaitForSeconds(duration);
        isJumpDashing = false;
    }

    IEnumerator DisableWallJump()
    {
        float rotationX = -20;
        if (transform.position.x > 0)
        {
            rotationX = 20;
        }
        float startRotation = transform.eulerAngles.y;
        float endRotation = startRotation + rotationX;
        float t = 0.0f;
        float zRotation;
        float duration = 0.6f;
        while (t < 0.6f)
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
        isWallJumping = false;
    }

    IEnumerator Smash(float duration)
    {
        float t = 0.0f;
        int angle;
        if (transform.position.x > 0)
        {
            angle = -30;
        }
        else
        {
            angle = 30;
        }
        while (t < duration)
        {
            if(isGrounded)
            {
                break;
            }
            if (t < duration / 2)
            {
                r2d.rotation = Mathf.Lerp(0, angle, 2 * t / duration);
                smashCollider.GetComponent<CapsuleCollider2D>().size = new Vector2(Mathf.SmoothStep(smashRadius / 1.5f, smashRadius, 2 * t / duration), smashCollider.GetComponent<CapsuleCollider2D>().size.y);
            }
            else
            {
                r2d.rotation = Mathf.Lerp(angle, 0, 2 * t / duration - 1);
                smashCollider.GetComponent<CapsuleCollider2D>().size = new Vector2(Mathf.SmoothStep(smashRadius, smashRadius / 1.5f, 2 * t / duration - 1), smashCollider.GetComponent<CapsuleCollider2D>().size.y);
            }
            t += Time.deltaTime;
            yield return null;
        }
        if (FindChild(FindChild(gameObject, "SpriteBlob"), "ClosedEyes").activeSelf)
        {
            FindChild(FindChild(gameObject, "SpriteBlob"), "EyesWhite").SetActive(true);
            FindChild(FindChild(gameObject, "SpriteBlob"), "ClosedEyes").SetActive(false);
        }
        smashCollider.GetComponent<CapsuleCollider2D>().size = new Vector2(0.00001f, smashCollider.GetComponent<CapsuleCollider2D>().size.y);
        smashCollider.SetActive(false);
        r2d.rotation = 0;
        yield return new WaitForSeconds(0.15f);
        isSmashing = false;
    }

    IEnumerator Bump(float duration, int invert)
    {
        float t = 0.0f;
        int angle;
        if (transform.position.x > 0)
        {
            angle = -30 * invert;
        }
        else
        {
            angle = 30 * invert;
        }
        while (t < duration)
        {
            if (t < duration / 2)
            {
                r2d.rotation = Mathf.Lerp(0, angle, 2 * t / duration);
                bumpCollider.GetComponent<CapsuleCollider2D>().size = new Vector2(Mathf.SmoothStep(bumpRadius / 1.5f, bumpRadius, 2 * t / duration), bumpCollider.GetComponent<CapsuleCollider2D>().size.y);
            }
            else
            {
                r2d.rotation = Mathf.Lerp(angle, 0, 2 * t / duration - 1);
                bumpCollider.GetComponent<CapsuleCollider2D>().size = new Vector2(Mathf.SmoothStep(bumpRadius, bumpRadius / 1.5f, 2 * t / duration - 1), bumpCollider.GetComponent<CapsuleCollider2D>().size.y);
            }
            t += Time.deltaTime;
            yield return null;
        }
        bumpCollider.GetComponent<CapsuleCollider2D>().size = new Vector2(0.00001f, bumpCollider.GetComponent<CapsuleCollider2D>().size.y);
        bumpCollider.SetActive(false);
        isBumping = false;
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