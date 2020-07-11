using System.Collections;
using UnityEngine;

public class BallCollision : MonoBehaviour
{
    public GameObject ball;
    public GameObject gameLogics;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.name == "Left Ground" && gameLogics.GetComponent<GameLogics>().isPlaying)
        {
            gameLogics.GetComponent<GameLogics>().playerWins("Blob 2");
        }

        if(collision.gameObject.name == "Right Ground" && gameLogics.GetComponent<GameLogics>().isPlaying)
        {
            gameLogics.GetComponent<GameLogics>().playerWins("Blob 1");
        }
    }
}
