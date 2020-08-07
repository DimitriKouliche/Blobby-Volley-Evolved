using System.Collections;
using Photon.Pun;
using UnityEngine;

public class BallLogics : MonoBehaviourPunCallbacks
{
    public GameObject ballIndicator;
    public GameObject gameLogics;
    public float dashUpwardForce = 9000;
    public float smashDownwardForce = 7000;
    bool isScaling;
    Vector3 initialScale = new Vector3(-1, -1, -1);
    Rigidbody2D rigidBody;
    Vector3 networkPosition;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (gameLogics != null && collision.gameObject.name == "Left Ground" && gameLogics.GetComponent<GameLogics>().isStarting)
        {
            gameLogics.GetComponent<GameLogics>().PlayerWins("Blob 2");
        }

        if (gameLogics != null && collision.gameObject.name == "Right Ground" && gameLogics.GetComponent<GameLogics>().isStarting)
        {
            gameLogics.GetComponent<GameLogics>().PlayerWins("Blob 1");
        }

        if (collision.gameObject.name == "Blob 1(Clone)" || collision.gameObject.name == "Blob 2(Clone)" || collision.gameObject.name == "Blob 3(Clone)" || collision.gameObject.name == "Blob 4(Clone)")
        {
            if(gameLogics != null)
            {
                gameLogics.GetComponent<GameLogics>().PlayerTouchesBall(collision.gameObject);
            }
            if (collision.gameObject.GetComponent<PlayerController>().isDashing)
            {
                GetComponent<Rigidbody2D>().AddForce(new Vector2(0, dashUpwardForce));
            }
        }
        Collider2D collider = collision.contacts[0].collider;
        if (collider.name == "Smash")
        {
            StartCoroutine(SmashFreeze(0.7f));
            if (transform.position.x > collider.transform.position.x)
            {
                GetComponent<Rigidbody2D>().AddForce(new Vector2(smashDownwardForce, -smashDownwardForce));
            }
            else
            {
                GetComponent<Rigidbody2D>().AddForce(new Vector2(-smashDownwardForce, -smashDownwardForce));
            }
            collider.gameObject.SetActive(false);
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
            ballIndicator.transform.position = new Vector3(transform.position.x, 4.5f, -2);
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
        if (!photonView.IsMine)
        {
            rigidBody.position = Vector3.MoveTowards(rigidBody.position, networkPosition, Time.fixedDeltaTime);
        }
        Camera.main.transform.position = new Vector3(gameObject.transform.position.x / 17, 0, -10);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(rigidBody.position);
            stream.SendNext(rigidBody.rotation);
            stream.SendNext(rigidBody.velocity);
        }
        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            rigidBody.velocity = (Vector3)stream.ReceiveNext();

            float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTimestamp));
            Vector3 velocity3D = new Vector3(rigidBody.velocity.x, rigidBody.velocity.y, 0);
            networkPosition += (velocity3D * lag);
        }
    }

    IEnumerator SmashFreeze(float duration)
    {
        Time.timeScale = 0;
        yield return StartCoroutine(WaitForRealSeconds(duration));
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
}
