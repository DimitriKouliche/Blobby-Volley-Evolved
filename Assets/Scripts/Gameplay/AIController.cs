using System;
using System.Collections;
using UnityEngine;

public class AIController : MonoBehaviour
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
    public GameObject smashAnimation;
    public GameObject bumpAnimation;
    public GameObject smashAnimationWhite;
    public GameObject bumpAnimationWhite;
    public GameObject smashCollider;
    public GameObject bumpCollider;
    public Color eyeChargeColor = new Color(233, 208, 118);

    float moveDirection = 0;
    bool isGrounded = false;
    Rigidbody2D r2d;
    Collider2D mainCollider;
    float jumpSpeed = 0;
    bool chargingJump = false;
    PlayerSounds playerSounds;
    Vector2 moveDirectionVector;
    bool isJumpDashing = false;
    float target = 0;
    GameObject ball;
    GameObject blob1;

    bool IsPlaying()
    {
        if (this == null || !gameObject.activeSelf)
        {
            return false;
        }
        return (gameLogics == null || gameLogics.GetComponent<GameLogics>().isPlaying) && !FindChild(GameObject.Find("UI"), "MenuContent").activeSelf;
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
        blob1 = GameObject.Find("Blob 1(Clone)");
    }

    void Jump(float height)
    {
        if ((!isGrounded && !IsTouchingPlayer()) || !IsPlaying() || r2d == null || Time.timeScale == 0)
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
        r2d.velocity = new Vector2(r2d.velocity.x, height);
        CancelCharge();
    }


    public void CancelCharge()
    {
        jumpSpeed = jumpHeight / 2.5f;
        chargingJump = false;
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

        // Fast falling
        if (moveDirectionVector.y < -0.7 && !isGrounded && !isSmashing)
        {
            r2d.velocity = new Vector2(r2d.velocity.x, r2d.velocity.y - 0.2f);
        }

        if(ball == null || ball.activeSelf == false)
        {
            ball = null;
            if (GameObject.Find("Ball") != null)
            {
                ball = GameObject.Find("Ball");
            }

            if (GameObject.Find("Cup") != null)
            {
                ball = GameObject.Find("Cup");
            }
        }

        if (ball == null)
        {
            return;
        }

        if(target < 4 || target > 8)
        {
            target = UnityEngine.Random.Range(4, 8);
        } 
        if (ball.transform.position.x > 0.5f)
        {
            target = ball.transform.position.x;
        } else if (ball.transform.position.x > -1 && ball.transform.position.x < 0f)
        {
            target = 0.5f;
        }
        if (ball.transform.position.y < 1.5f - ball.GetComponent<Rigidbody2D>().velocity.y / 10 && ball.transform.position.y > -2 && ball.transform.position.x > -1f && ball.transform.position.x < 3f &&
            transform.position.x - ball.transform.position.x > -0.5f && Math.Abs(ball.transform.position.x - transform.position.x) < 2.5f && gameLogics.GetComponent<GameLogics>().teamBallTouches[1] < 3
            && ball.GetComponent<Rigidbody2D>().velocity.magnitude < 20f && ball.GetComponent<Rigidbody2D>().velocity.y > -15 && ball.GetComponent<Rigidbody2D>().velocity.y < 5)
        {
            moveDirection = 0f;
            Jump(jumpHeight);
        }
        if (ball.transform.position.y < 0.5f && ball.transform.position.y > 0 && ball.transform.position.x > 0 && ball.transform.position.x < 7 && 
            Math.Abs(ball.transform.position.x - transform.position.x) < 2.5f && gameLogics.GetComponent<GameLogics>().teamBallTouches[1] < 3 && 
            gameLogics.GetComponent<GameLogics>().teamBallTouches[1] > 0 && ball.GetComponent<Rigidbody2D>().velocity.y < 0)
        {
            moveDirection = 0f;
            Jump(jumpHeight);
        }
        else if(target - transform.position.x > 3)
        {
            moveDirection = 1f;
        }
        else if(target - transform.position.x < -3)
        {
            moveDirection = -1f;
        }
        else if(target > transform.position.x - 0.25f)
        {
            if(!isGrounded)
            {
                moveDirection += 0.8f;
            } else
            {
                moveDirection += 0.7f;
            }
        }
        else if (target < transform.position.x - 0.45f)
        {
            if (!isGrounded)
            {
                moveDirection -= 0.8f;
            }
            else
            {
                moveDirection -= 0.7f;
            }
        } else if(ball.transform.position.y < -3.2 && ball.transform.position.x > 0)
        {
            if(gameLogics.GetComponent<GameLogics>().teamBallTouches[1] < 2 && (ball.transform.position.y < -4.1 || ball.transform.position.y > -3.9))
            {
                Jump(UnityEngine.Random.Range(jumpHeight / 1.5f, jumpHeight));
            } else
            {
                Jump(jumpHeight);
            }
        } else
        {
            moveDirection = 0;
        }
        if(moveDirection > 1)
        {
            moveDirection = 1;
        }
        if(moveDirection < -1)
        {
            moveDirection = -1;
        }
        if(isSmashing)
        {
            moveDirection = 0.5f;
        }
        if (transform.position.y > -2 && Math.Abs(ball.transform.position.x - transform.position.x) < 3f  && ball.transform.position.x < transform.position.x + 0.3f
            && Math.Abs(ball.transform.position.y - transform.position.y) < 1.6f && gameLogics.GetComponent<GameLogics>().teamBallTouches[1] < 3 && !isSmashing 
            && ball.transform.position.x < 7 && ball.transform.position.y > -3 + transform.position.x / 10 && 
            (blob1.transform.position.x + transform.position.x > 1.2f || transform.position.y - 0.5f > blob1.transform.position.y) )
        {
            playerSounds.SmashSound();
            if (FindChild(FindChild(gameObject, "SpriteBlob"), "EyesWhite").activeSelf)
            {
                FindChild(FindChild(gameObject, "SpriteBlob"), "EyesWhite").SetActive(false);
                FindChild(FindChild(gameObject, "SpriteBlob"), "ClosedEyes").SetActive(true);
            }
            smashAnimation.SetActive(true);
            smashAnimationWhite.SetActive(true);
            isSmashing = true;
            smashCollider.SetActive(true);
            StartCoroutine(Smash(0.7f));
        }
        if (Math.Abs(ball.transform.position.y - transform.position.y) < 1.5f && ball.transform.position.x > 0 && transform.position.x - ball.transform.position.x > 1.5f 
            && gameLogics.GetComponent<GameLogics>().teamBallTouches[1] < 3 && !isBumping)
        { 
            playerSounds.BumpSound();
            bumpAnimation.SetActive(true);
            bumpAnimationWhite.SetActive(true);
            bumpCollider.SetActive(true);
            isBumping = true;
            StartCoroutine(Bump(0.5f, 1));
        }

        if ((ball.transform.position.y < -5.8f + ball.GetComponent<Rigidbody2D>().velocity.magnitude / 50 + Math.Abs(ball.transform.position.x - transform.position.x) / 10 && ball.transform.position.x > 0 && Math.Abs(ball.transform.position.x - transform.position.x) > 0.8f 
            && !isDashing && isGrounded && gameLogics.GetComponent<GameLogics>().teamBallTouches[1] < 3) || 
            ((ball.transform.position.y < -6 && ball.transform.position.x > 2) 
            && !isDashing && isGrounded))
        {
            if (FindChild(FindChild(gameObject, "SpriteBlob"), "EyesWhite").activeSelf)
            {
                FindChild(FindChild(gameObject, "SpriteBlob"), "EyesWhite").SetActive(false);
                FindChild(FindChild(gameObject, "SpriteBlob"), "ClosedEyes").SetActive(true);
            }
            isDashing = true;
            playerSounds.DashSound();
            if (ball.transform.position.x < transform.position.x)
            {
                moveDirection = -1f;
            }
            else
            {
                moveDirection = 1f;
            }
            r2d.AddForce(new Vector3(moveDirection * dashDistance * 5000, 0, transform.position.z));
            if (moveDirection > 0)
            {
                dashLefttAnimation.SetActive(true);
                StartCoroutine(DisableDash(0.45f, -70f));
            }
            else
            {
                dashRightAnimation.SetActive(true);
                StartCoroutine(DisableDash(0.45f, 70f));
            }
        }
        // Fast falling
        if (!isGrounded && !isSmashing)
        {
            r2d.velocity = new Vector2(r2d.velocity.x, r2d.velocity.y - 0.5f);
        }
    }


    bool IsTouchingPlayer()
    {
        if (transform.position.y > 0)
        {
            return false;
        }
        if (GameObject.Find("Blob 1(Clone)") && gameObject.name != "Blob 1(Clone)" && gameObject.GetComponent<CapsuleCollider2D>().IsTouching(
            GameObject.Find("Blob 1(Clone)").GetComponent<CircleCollider2D>()))
        {
            return true;
        }
        if (GameObject.Find("Blob 2(Clone)") && gameObject.name != "Blob 2(Clone)" && gameObject.GetComponent<CapsuleCollider2D>().IsTouching(
            GameObject.Find("Blob 2(Clone)").GetComponent<CircleCollider2D>()))
        {
            return true;
        }
        if (GameObject.Find("Blob 3(Clone)") && gameObject.name != "Blob 3(Clone)" && gameObject.GetComponent<CapsuleCollider2D>().IsTouching(
            GameObject.Find("Blob 3(Clone)").GetComponent<CircleCollider2D>()))
        {
            return true;
        }
        if (GameObject.Find("Blob 4(Clone)") && gameObject.name != "Blob 4(Clone)" && gameObject.GetComponent<CapsuleCollider2D>().IsTouching(
            GameObject.Find("Blob 4(Clone)").GetComponent<CircleCollider2D>()))
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
        isGrounded = transform.position.y < -6.1;

        if (isGrounded)
        {
            isJumpDashing = false;
        }

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

        if (isSmashing)
        {
            r2d.velocity = new Vector2((moveDirection / 2) * maxSpeed, r2d.velocity.y);
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
            if (t < duration / 2)
            {
                r2d.rotation = Mathf.Lerp(0, angle, 2 * t / duration);
                smashCollider.GetComponent<CapsuleCollider2D>().size = new Vector2(Mathf.SmoothStep(
                    smashRadius / 1.5f, smashRadius, 2 * t / duration), smashCollider.GetComponent<CapsuleCollider2D>().size.y);
            }
            else
            {
                r2d.rotation = Mathf.Lerp(angle, 0, 2 * t / duration - 1);
                smashCollider.GetComponent<CapsuleCollider2D>().size = new Vector2(Mathf.SmoothStep(
                    smashRadius, smashRadius / 1.5f, 2 * t / duration - 1), smashCollider.GetComponent<CapsuleCollider2D>().size.y);
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
        yield return new WaitForSeconds(0.15f);
        isSmashing = false;
        isDashing = false;
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
                bumpCollider.GetComponent<CapsuleCollider2D>().size = new Vector2(
                    Mathf.SmoothStep(bumpRadius / 1.5f, bumpRadius, 2 * t / duration), bumpCollider.GetComponent<CapsuleCollider2D>().size.y);
            }
            else
            {
                r2d.rotation = Mathf.Lerp(angle, 0, 2 * t / duration - 1);
                bumpCollider.GetComponent<CapsuleCollider2D>().size = new Vector2(
                    Mathf.SmoothStep(bumpRadius, bumpRadius / 1.5f, 2 * t / duration - 1), bumpCollider.GetComponent<CapsuleCollider2D>().size.y);
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
