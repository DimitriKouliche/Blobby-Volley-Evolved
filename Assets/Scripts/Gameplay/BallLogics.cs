using System.Collections;
using UnityEngine;

public class BallLogics : MonoBehaviour
{
    public GameObject ballIndicator;
    public GameObject gameLogics;
    public float dashUpwardForce = 9000;
    public float serviceForce = 2000;
    public float smashDownwardForce = 7000;
    Rigidbody2D rigidBody;

    bool service = true;

    void OnCollisionEnter2D(Collision2D collision)
    {
        Collider2D collider = collision.contacts[0].collider;
        if (gameLogics != null && collision.gameObject.name == "Left Ground" && gameLogics.GetComponent<GameLogics>().isStarting)
        {
            gameLogics.GetComponent<GameLogics>().PlayerWins("Blob 2");
            service = true;
        }

        if (gameLogics != null && collision.gameObject.name == "Right Ground" && gameLogics.GetComponent<GameLogics>().isStarting)
        {
            gameLogics.GetComponent<GameLogics>().PlayerWins("Blob 1");
            service = true;
            Debug.Log(service);
        }

        if (collision.gameObject.name == "Blob 1(Clone)" || collision.gameObject.name == "Blob 2(Clone)" || collision.gameObject.name == "Blob 3(Clone)" || collision.gameObject.name == "Blob 4(Clone)")
        {
            Debug.Log(service);
            if (gameLogics != null)
            {
                gameLogics.GetComponent<GameLogics>().PlayerServes(collision.gameObject);
                gameLogics.GetComponent<GameLogics>().PlayerTouchesBall(collision.gameObject);
            }
            if (collision.gameObject.GetComponent<PlayerController>().isDashing)
            {
                rigidBody.AddForce(new Vector2(0, dashUpwardForce));
            }
            if(service)
            {
                rigidBody.AddForce(new Vector2(serviceForce, serviceForce));
            }
            if (collider.name != "Smash")
            {
                service = false;
            }
        }
        if (collider.name == "Smash")
        {
            StartCoroutine(SmashFreeze(0.7f, collider.gameObject));
            rigidBody.velocity = Vector3.zero;
            if (collision.gameObject.transform.position.x < 0)
            {
                if (service)
                {
                    rigidBody.AddForce(new Vector2(smashDownwardForce, serviceForce));

                } else
                {
                    rigidBody.AddForce(new Vector2(smashDownwardForce, -smashDownwardForce));
                }
            }
            else
            {
                if (service)
                {
                    rigidBody.AddForce(new Vector2(-smashDownwardForce, serviceForce));

                }
                else
                {
                    rigidBody.AddForce(new Vector2(-smashDownwardForce, -smashDownwardForce));
                }
            }
            collider.gameObject.SetActive(false);
            service = false;
        }
    }

    private void Start()
    {
        rigidBody = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ballIndicator!= null && ballIndicator.activeSelf)
        {
            ballIndicator.transform.position = new Vector3(transform.position.x, 7.5f, -2);
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
        Camera.main.transform.position = new Vector3(gameObject.transform.position.x / 17, 0, -10);
        if(transform.position.y > 10)
        {
            rigidBody.AddForce(new Vector2(0, -5));
        }
    }


    IEnumerator SmashFreeze(float duration, GameObject smash)
    {
        Time.timeScale = 0;
        FindChild(smash.transform.parent.gameObject, "SmashFreezeFrame").SetActive(true);
        yield return StartCoroutine(WaitForRealSeconds(duration));
        FindChild(smash.transform.parent.gameObject, "SmashFreezeFrame").SetActive(false);
        Time.timeScale = 1;
        Camera.main.GetComponent<CameraShake>().Shake();
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
