using System.Collections;
using UnityEngine;

public class BallCollision : MonoBehaviour
{
    public GameObject gameOver;
    public GameObject ball;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.name == "Left Ground")
        {
            StartCoroutine(GameOverCoroutine());
        }

        if(collision.gameObject.name == "Right Ground")
        {
            StartCoroutine(GameOverCoroutine());
        }
    }

    IEnumerator GameOverCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        gameOver.SetActive(true);
    }
}
