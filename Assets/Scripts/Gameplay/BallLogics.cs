using System.Collections;
using Photon.Pun;
using UnityEngine;

public class BallLogics : MonoBehaviourPunCallbacks
{
    public GameObject ballIndicator;
    public GameObject gameLogics;
    public float dashUpwardForce = 250;
    bool isScaling;
    Vector3 initialScale = new Vector3(-1, -1, -1);
    Rigidbody2D rigidBody;
    Vector3 networkPosition;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Left Ground" && gameLogics.GetComponent<GameLogics>().isStarting)
        {
            gameLogics.GetComponent<GameLogics>().PlayerWins("Blob 2");
        }

        if (collision.gameObject.name == "Right Ground" && gameLogics.GetComponent<GameLogics>().isStarting)
        {
            gameLogics.GetComponent<GameLogics>().PlayerWins("Blob 1");
        }

        if (!photonView.IsMine)
        {
            return;
        }
        if (collision.gameObject.name == "Blob 1(Clone)" || collision.gameObject.name == "Blob 2(Clone)")
        {
            if ((!gameLogics.GetComponent<GameLogics>().isOnline && collision.gameObject.GetComponent<PlayerController>().isDashing) ||
                (gameLogics.GetComponent<GameLogics>().isOnline && collision.gameObject.GetComponent<OnlinePlayerController>().isDashing))
            {
                GetComponent<Rigidbody2D>().AddForce(new Vector2(0, dashUpwardForce));
            }
            if (initialScale.x == -1)
            {
                initialScale = collision.gameObject.transform.localScale;
            }
            StartCoroutine(SquashAnimation(collision.gameObject.transform,
                new Vector3(collision.gameObject.transform.localScale.x * Mathf.Min(1, (1 - collision.relativeVelocity.x / 30)),
                collision.gameObject.transform.localScale.y * Mathf.Min(1, (1 - collision.relativeVelocity.y / 30)),
                collision.gameObject.transform.localScale.z),
                0.3f));
        }
    }

    IEnumerator SquashAnimation(Transform objectToScale, Vector3 toScale, float duration)
    {
        //Make sure there is only one instance of this function running
        if (isScaling)
        {
            yield break; ///exit if this is still running
        }
        isScaling = true;

        float counter = 0;

        //Get the current scale of the object to be moved
        Vector3 startScaleSize = objectToScale.localScale;


        while (counter < duration / 2)
        {
            counter += Time.deltaTime;
            objectToScale.localScale = Vector3.Lerp(startScaleSize, toScale, counter / duration);
            yield return null;
        }

        while (counter < duration)
        {
            counter += Time.deltaTime;
            objectToScale.localScale = Vector3.Lerp(toScale, startScaleSize, counter / duration);
            yield return null;
        }
        objectToScale.localScale = initialScale;
        isScaling = false;
    }

    private void Start()
    {
        rigidBody = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ballIndicator.activeSelf)
        {
            ballIndicator.transform.position = new Vector3(transform.position.x, 4.5f, -2);
        }
    }

    void OnBecameInvisible()
    {
        ballIndicator.SetActive(true);
    }

    void OnBecameVisible()
    {
        ballIndicator.SetActive(false);
    }

    public void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            rigidBody.position = Vector3.MoveTowards(rigidBody.position, networkPosition, Time.fixedDeltaTime);
        }
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
}
