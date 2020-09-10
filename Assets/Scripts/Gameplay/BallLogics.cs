using System.Collections;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class BallLogics : MonoBehaviour
{
    public GameObject ballIndicator;
    public GameObject gameLogics;
    public float dashUpwardForce = 9000;
    public float bumpUpwardForce = 9000;
    public float serviceForce = 2000;
    public float smashDownwardForce = 7000;
    public bool service = true;
    bool follow = false;

    bool canHit = true;
    Rigidbody2D rigidBody;



    void OnCollisionEnter2D(Collision2D collision)
    {
        FindChild(gameObject, "ParticleTrail03").GetComponent<ParticleSystem>().Stop();
        if (!canHit)
        {
            return;
        }
        StartCoroutine(EnableHit(0.1f));
        Collider2D collider = collision.contacts[0].collider;
        if (gameLogics != null && collision.gameObject.name == "Left Ground" && gameLogics.GetComponent<GameLogics>().isStarting)
        {
            gameLogics.GetComponent<GameLogics>().PlayerWins("Blob 2");
            FindChild(gameObject, "ParticleStars").GetComponent<ParticleSystem>().Play();
            FindChild(gameObject, "ParticleStars").transform.position = transform.position;
        }

        if (gameLogics != null && collision.gameObject.name == "Right Ground" && gameLogics.GetComponent<GameLogics>().isStarting)
        {
            gameLogics.GetComponent<GameLogics>().PlayerWins("Blob 1");
            FindChild(gameObject, "ParticleStars").GetComponent<ParticleSystem>().Play();
            FindChild(gameObject, "ParticleStars").transform.position = transform.position;
        }

        if (collision.gameObject.name == "Blob 1(Clone)" || collision.gameObject.name == "Blob 2(Clone)" || collision.gameObject.name == "Blob 3(Clone)" || collision.gameObject.name == "Blob 4(Clone)")
        {
            canHit = false;
            if (gameLogics != null)
            {
                gameLogics.GetComponent<GameLogics>().PlayerServes(collision.gameObject);
                int touches = gameLogics.GetComponent<GameLogics>().PlayerTouchesBall(collision.gameObject);
                UpdateBall(touches);
            }
            if (collision.gameObject.GetComponent<PlayerController>().isDashing)
            {
                rigidBody.AddForce(new Vector2(0, dashUpwardForce));
            }
            if(service)
            {
                if(collision.gameObject.transform.position.x < 0)
                {
                    rigidBody.AddForce(new Vector2(serviceForce, serviceForce));
                } else
                {
                    rigidBody.AddForce(new Vector2(-serviceForce, serviceForce));
                }
            }
            if (collider.name != "Smash" && gameLogics.GetComponent<GameLogics>().isStarting)
            {
                service = false;
            }
            if (collider.name != "Smash")
            {
                collision.gameObject.GetComponent<PlayerSounds>().BallContactSound();
            }

            FindChild(collision.gameObject, "ParticleContact").GetComponent<ParticleSystem>().Play();
            FindChild(collision.gameObject, "ParticleContact").transform.position = collision.gameObject.transform.position;

        }
        if (collider.name == "Smash")
        {
            StartCoroutine(SmashFreeze(0.5f, collider.gameObject));
            rigidBody.velocity = Vector3.zero;
            float positionFactor = transform.position.y - collision.gameObject.transform.position.y;
            float yForce = positionFactor * smashDownwardForce / 3;
            float xForce = (1 - Mathf.Abs(positionFactor)) * smashDownwardForce / 2;
            if (collision.gameObject.transform.position.x < 0)
            {
                if (service)
                {
                    rigidBody.AddForce(new Vector2(smashDownwardForce + xForce, serviceForce + yForce));

                } else
                {
                    rigidBody.AddForce(new Vector2(smashDownwardForce + xForce, -smashDownwardForce + yForce*2));
                }
            }
            else
            {
                if (service)
                {
                    rigidBody.AddForce(new Vector2(-smashDownwardForce - xForce, serviceForce + yForce));

                }
                else
                {
                    rigidBody.AddForce(new Vector2(-smashDownwardForce - xForce, -smashDownwardForce + yForce*2));
                }
            }
            collider.gameObject.SetActive(false);
            service = false;
        }

        if (collider.name == "Bump")
        {
            rigidBody.AddForce(new Vector2(0, bumpUpwardForce));
        }
    }

    private void Start()
    {
        rigidBody = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float opacity = Mathf.Min((rigidBody.velocity.magnitude - 10) / 20, 1);
        if(opacity < 0)
        {
            opacity = 0;
        }
        MainModule particles = FindChild(gameObject, "ParticleTrail02").GetComponent<ParticleSystem>().main;
        particles.startColor = new Color(1, 1, 1, opacity);
        if (ballIndicator!= null && ballIndicator.activeSelf)
        {
            ballIndicator.transform.position = new Vector3(transform.position.x, 7f, -2);
        }
    }

    void OnBecameInvisible()
    {
        if (ballIndicator != null)
          ballIndicator.SetActive(true);
    }

    void OnBecameVisible()
    {
        if (ballIndicator != null)
          ballIndicator.SetActive(false);
    }

    public void FixedUpdate()
    {
        if(!follow)
        {
            float originX = Camera.main.transform.position.x;
            StartCoroutine(SmoothFollow(0.3f, originX));
            follow = true;
        }
        if (transform.position.y > 10)
        {
            rigidBody.AddForce(new Vector2(0, -5));
        }
    }

    public void UpdateBall(int touches)
    {
        if(touches == 1)
        {
            MainModule particles = FindChild(gameObject, "ParticleBall").GetComponent<ParticleSystem>().main;
            particles.startColor = new Color(233f / 255f, 208f / 255f, 118f / 255f);
        }
        if(touches == 2)
        {
            MainModule particles = FindChild(gameObject, "ParticleBall").GetComponent<ParticleSystem>().main;
            particles.startColor = new Color(203f / 255f, 119f / 255f, 52f / 255f);
        }
        if(touches == 3)
        {
            MainModule particles = FindChild(gameObject, "ParticleBall").GetComponent<ParticleSystem>().main;
            particles.startColor = new Color(172f / 255f, 49f / 255f, 39f / 255f);
        }
        FindChild(gameObject, "ParticleBall").GetComponent<ParticleSystem>().Stop();
        FindChild(gameObject, "ParticleBall").GetComponent<ParticleSystem>().Play();
    }

    IEnumerator SmoothFollow(float duration, float originX)
    {
        float targetX = gameObject.transform.position.x / 17;
        float t = 0.0f;
        while (t < duration)
        {
            Camera.main.transform.position = new Vector3(Mathf.Lerp(originX, targetX, t / duration), 0, -10);
            t += Time.deltaTime;
            yield return null;
        }
        follow = false;
    }

        IEnumerator SmashFreeze(float duration, GameObject smash)
    {
        Time.timeScale = 0;
        FindChild(smash.transform.parent.gameObject, "SmashFreezeFrame").SetActive(true);
        FindChild(smash.transform.parent.gameObject, "SmashFreezeFrameWhite").SetActive(true);
        FindChild(smash.transform.parent.gameObject, "SmashImpact").transform.position = transform.position;
        FindChild(smash.transform.parent.gameObject, "SmashImpact").SetActive(false);
        FindChild(smash.transform.parent.gameObject, "InkStains").transform.position = transform.position;
        FindChild(smash.transform.parent.gameObject, "InkStains").GetComponent<ParticleSystem>().Play();
        yield return StartCoroutine(WaitForRealSeconds(duration));
        FindChild(smash.transform.parent.gameObject, "SmashFreezeFrame").SetActive(false);
        FindChild(smash.transform.parent.gameObject, "SmashFreezeFrameWhite").SetActive(false);
        Time.timeScale = 1;
        Camera.main.GetComponent<CameraShake>().Shake();
        StartCoroutine(SmashParticleTrail(0.7f));
    }

    IEnumerator SmashParticleTrail(float duration)
    {
        FindChild(gameObject, "ParticleTrail03").GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(duration);
        FindChild(gameObject, "ParticleTrail03").GetComponent<ParticleSystem>().Stop();
    }

    IEnumerator EnableHit(float duration)
    {
        yield return new WaitForSeconds(duration);
        canHit = true;
    }

    IEnumerator WaitForRealSeconds(float seconds)
    {
        float startTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - startTime < seconds)
        {
            yield return null;
        }
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
