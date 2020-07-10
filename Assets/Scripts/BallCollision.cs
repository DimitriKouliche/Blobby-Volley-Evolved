using System.Collections;
using UnityEngine;

public class BallCollision : MonoBehaviour
{
    public GameObject gameOver;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.name == "Ground")
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
