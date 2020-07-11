using System.Collections;
using UnityEngine;

public class BallLogics : MonoBehaviour
{
    public GameObject ball;
    public GameObject ballIndicator;
    public GameObject gameLogics;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.name == "Left Ground" && gameLogics.GetComponent<GameLogics>().isStarting)
        {
            gameLogics.GetComponent<GameLogics>().PlayerWins("Blob 2");
        }

        if(collision.gameObject.name == "Right Ground" && gameLogics.GetComponent<GameLogics>().isStarting)
        {
            gameLogics.GetComponent<GameLogics>().PlayerWins("Blob 1");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(ballIndicator.activeSelf)
        {
            ballIndicator.transform.position = new Vector3 (ball.transform.position.x, 4.5f, -2);
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
}
