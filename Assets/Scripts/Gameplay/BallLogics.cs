﻿using System.Collections;
using Steamworks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
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
    public bool isFreezed = false;
    public AudioClip[] ballHitClips;
    [Range(0f, 5f)]
    public float ballHitVolume = 1f;
    public AudioClip[] smashImpactClips;
    [Range(0f, 5f)]
    public float smashImpactVolume = 1f;
    public AudioClip[] ballHardHitClips;
    [Range(0f, 5f)]
    public float ballHardHitVolume = 1f;
    public AudioClip[] tooManyTouchesClips;
    [Range(0f, 5f)]
    public float tooManyTouchesVolume = 1f;
    public float chromaticAberrationThreshold = 0.8f;

    public Color firstHitColor = new Color(233f / 255f, 208f / 255f, 118f / 255f);
    public Color secondHitColor = new Color(203f / 255f, 119f / 255f, 52f / 255f);
    public Color thirdHitColor = new Color(172f / 255f, 49f / 255f, 39f / 255f);
    public bool victoryMusicIsPlayed = false;

    bool follow = false;
    public bool canHit = true;
    Rigidbody2D rigidBody;
    AudioSource audioSource;
    float timeBetweenCollisions = 0;
    string lastPlayerTouch = "";



    void OnCollisionEnter2D(Collision2D collision)
    {
        float intensity = Mathf.Min((rigidBody.velocity.magnitude) / 10, 2);
        BallHitSound(intensity);
        FindChild(gameObject, "ParticleTrail03").GetComponent<ParticleSystem>().Stop();
        Collider2D collider = collision.contacts[0].collider;
        if((collision.gameObject.name == "Left Ground" || collision.gameObject.name == "Right Ground") && gameObject.name == "Cup")
        {
            StartCoroutine(VictoryMusic());
        }
        if (gameLogics != null && collision.gameObject.name == "Left Ground" && gameLogics.GetComponent<GameLogics>().isStarting)
        {
            if(gameObject.name != "Cup")
            {
                BallAchievement();
            }
            gameLogics.GetComponent<GameLogics>().PlayerWins("Blob 2");
            FindChild(gameObject, "ParticleStars").GetComponent<ParticleSystem>().Play();
            FindChild(gameObject, "ParticleStars").transform.position = transform.position;
            if(intensity > chromaticAberrationThreshold)
            {
                GameObject.Find("Global Volume").GetComponent<ChromaticAberrationEffect>().IntenseEffect();
                BallHardHitSound();
            }
        }

        if (gameLogics != null && collision.gameObject.name == "Right Ground" && gameLogics.GetComponent<GameLogics>().isStarting)
        {
            if (gameObject.name != "Cup")
            {
                BallAchievement();
            }
            gameLogics.GetComponent<GameLogics>().PlayerWins("Blob 1");
            FindChild(gameObject, "ParticleStars").GetComponent<ParticleSystem>().Play();
            FindChild(gameObject, "ParticleStars").transform.position = transform.position;
            if (intensity > chromaticAberrationThreshold)
            {
                if (SteamManager.Initialized)
                {
                    SteamUserStats.SetAchievement("RAINBOW");
                    SteamUserStats.StoreStats();
                }
                GameObject.Find("Global Volume").GetComponent<ChromaticAberrationEffect>().IntenseEffect();
                BallHardHitSound();
            }
        }
        if (!canHit && lastPlayerTouch == collision.gameObject.name)
        {
            return;
        }

        if(canHit)
        {
            StartCoroutine(EnableHit(0.25f));
        }

        if (collision.gameObject.name == "Blob 1(Clone)" || collision.gameObject.name == "Blob 2(Clone)" || collision.gameObject.name == "Blob 3(Clone)" || collision.gameObject.name == "Blob 4(Clone)")
        {
            lastPlayerTouch = collision.gameObject.name;
            if (gameObject.name != "Cup")
            {
                BallAchievement();
            }
            if (gameObject.name == "Cup")
            {
                rigidBody.gravityScale = 1.6f;
                FindChild(GameObject.Find("Level"), "Ceiling").SetActive(true);
                StartCoroutine(VictoryMusic());
            }
            canHit = false;
            if (gameLogics != null)
            {
                gameLogics.GetComponent<GameLogics>().PlayerServes(collision.gameObject);
                int touches = gameLogics.GetComponent<GameLogics>().PlayerTouchesBall(collision.gameObject);
                UpdateBall(touches);
            }
            if (collision.gameObject.GetComponent<PlayerController>() != null && collision.gameObject.GetComponent<PlayerController>().isDashing)
            {
                rigidBody.AddForce(new Vector2(0, dashUpwardForce));
            }
            if (collision.gameObject.GetComponent<AIController>() != null && collision.gameObject.GetComponent<AIController>().isDashing)
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
            if (!service)
            {
                intensity = Mathf.Min((collision.gameObject.transform.position.y - transform.position.y + 2) / 2, 3);
                SmashImpactSound(intensity);
            } else
            {
                collision.gameObject.GetComponent<PlayerSounds>().SmashServiceSound();
            }
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
            collision.gameObject.GetComponent<PlayerSounds>().BumpImpactSound();
            rigidBody.AddForce(new Vector2(0, bumpUpwardForce));
        }
    }

    void BallAchievement()
    {
        if (timeBetweenCollisions > 5.9f)
        {
            if (SteamManager.Initialized)
            {
                SteamUserStats.SetAchievement("BALL");
                SteamUserStats.StoreStats();
            }
        }
        timeBetweenCollisions = 0;
    }

    private void Start()
    {
        rigidBody = gameObject.GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!service)
        {
            timeBetweenCollisions += Time.deltaTime;
        }
        float opacity = Mathf.Min((rigidBody.velocity.magnitude - 10) / 20, 1);
        if(opacity < 0)
        {
            opacity = 0;
        }
        MainModule particles = FindChild(gameObject, "ParticleTrail02").GetComponent<ParticleSystem>().main;
        particles.startColor = new Color(1, 1, 1, opacity);
        if (ballIndicator!= null && ballIndicator.activeSelf)
        {
            ballIndicator.transform.position = new Vector3(transform.position.x, 8f, -2);
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

    public void BallHitSound(float intensity)
    {
        float sfxVolume = PlayerPrefs.GetFloat("sfxVolume", 100f);
        int index = Random.Range(0, ballHitClips.Length);
        audioSource.PlayOneShot(ballHitClips[index], ballHitVolume * intensity * sfxVolume / 100);
    }

    public void TooManyTouchesSound()
    {
        float sfxVolume = PlayerPrefs.GetFloat("sfxVolume", 100f);
        int index = Random.Range(0, tooManyTouchesClips.Length);
        audioSource.PlayOneShot(tooManyTouchesClips[index], tooManyTouchesVolume * sfxVolume / 100);
        StartCoroutine(BallFlash());
    }

    public void BallHardHitSound()
    {
        float sfxVolume = PlayerPrefs.GetFloat("sfxVolume", 100f);
        int index = Random.Range(0, ballHardHitClips.Length);
        audioSource.PlayOneShot(ballHardHitClips[index], ballHitVolume * sfxVolume / 100);
    }

    void SmashImpactSound(float intensity)
    {
        float sfxVolume = PlayerPrefs.GetFloat("sfxVolume", 100f);
        int index = Random.Range(0, smashImpactClips.Length);
        audioSource.PlayOneShot(smashImpactClips[index], smashImpactVolume * intensity * sfxVolume / 100);
    }

    public void UpdateBall(int touches)
    {
        if(touches == 1)
        {
            MainModule particles = FindChild(gameObject, "ParticleBall").GetComponent<ParticleSystem>().main;
            particles.startColor = firstHitColor;
        }
        if(touches == 2)
        {
            MainModule particles = FindChild(gameObject, "ParticleBall").GetComponent<ParticleSystem>().main;
            particles.startColor = secondHitColor;
        }
        if(touches == 3)
        {
            MainModule particles = FindChild(gameObject, "ParticleBall").GetComponent<ParticleSystem>().main;
            particles.startColor = thirdHitColor;
        }
        FindChild(gameObject, "ParticleBall").GetComponent<ParticleSystem>().Stop();
        FindChild(gameObject, "ParticleBall").GetComponent<ParticleSystem>().Play();
    }
    IEnumerator BallFlash()
    {
        GetComponent<Rigidbody2D>().velocity /= 3;
        float t;
        Material mat = GetComponent<SpriteRenderer>().material;
        for (int i = 0; i < 3; i++)
        {
            t = 0.0f;
            while (t < 0.2f)
            {
                mat.SetColor("_Color", new Color(1, 1, 1, Mathf.Lerp(0.75f, 0, t * 5)));
                t += Time.deltaTime;
                yield return null;
            }
            t = 0.0f;
            while (t < 0.2f)
            {
                mat.SetColor("_Color", new Color(1, 1, 1, Mathf.Lerp(0, 1, t * 5)));
                t += Time.deltaTime;
                yield return null;
            }
        }
        mat.SetColor("_Color", new Color(1, 1, 1, 1));
    }

    IEnumerator SmoothFollow(float duration, float originX)
    {
        float targetX = gameObject.transform.position.x / 17;
        float t = 0.0f;
        while (t < duration)
        {
            Camera.main.transform.position = new Vector3(Mathf.Lerp(originX, targetX, t / duration), Camera.main.transform.position.y, -10);
            t += Time.deltaTime;
            yield return null;
        }
        follow = false;
    }
    
    IEnumerator VictoryMusic()
    {
        if (victoryMusicIsPlayed)
        {
            yield break;
        }
        victoryMusicIsPlayed = true;
        GameObject gameOver = GameObject.Find("GameOver");
        FindChild(gameOver, "Confetti").GetComponent<ParticleSystem>().Stop();
        SpriteRenderer blackSpriteRenderer = FindChild(FindChild(GameObject.Find("GameOver"), "Blackout"), "Black").GetComponent<SpriteRenderer>();
        float t = 0.0f;
        if(blackSpriteRenderer.color.a == 0)
        {
            yield return new WaitForSeconds(2);
        }
        else
        {
            while (t < 2f)
            {
                blackSpriteRenderer.color = new Color(blackSpriteRenderer.color.r, blackSpriteRenderer.color.g, blackSpriteRenderer.color.b, Mathf.Lerp(0.85f, 0, t * 2));
                t += Time.deltaTime;
                yield return null;
            }
        }
        yield return new WaitForSeconds(1);
        GameObject.Find("Music(Clone)").GetComponent<MusicMixer>().VictoryMusic();
        yield return new WaitForSeconds(300);
        if(gameOver.activeSelf)
        {
            if (SteamManager.Initialized)
            {
                SteamUserStats.SetAchievement("JANITOR");
                SteamUserStats.StoreStats();
            }
        }
    }

    IEnumerator SmashFreeze(float duration, GameObject smash)
    {
        isFreezed = true;
        Time.timeScale = 0.0001f;
        FindChild(smash.transform.parent.gameObject, "SmashFreezeFrame").SetActive(true);
        FindChild(smash.transform.parent.gameObject, "SmashFreezeFrameWhite").SetActive(true);
        FindChild(smash.transform.parent.gameObject, "SmashImpact").transform.position = transform.position;
        FindChild(smash.transform.parent.gameObject, "SmashImpact").SetActive(false);
        FindChild(smash.transform.parent.gameObject, "InkStains").transform.position = transform.position;
        FindChild(smash.transform.parent.gameObject, "InkStains").GetComponent<ParticleSystem>().Play();
        yield return StartCoroutine(WaitForRealSeconds(duration));
        FindChild(smash.transform.parent.gameObject, "SmashFreezeFrame").SetActive(false);
        FindChild(smash.transform.parent.gameObject, "SmashFreezeFrameWhite").SetActive(false);
        Time.timeScale = 1.2f;
        isFreezed = false;
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
