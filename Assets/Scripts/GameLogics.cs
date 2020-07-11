using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class GameLogics : MonoBehaviour
{
    public GameObject uiMessage;
    public GameObject uiScore;
    public GameObject gameOver;
    public GameObject blob1;
    public GameObject blob2;
    public GameObject ball;
    public bool isPlaying = false;
    
    Vector3 blob1Position;
    Vector3 blob2Position;
    Vector3 blob1Scale;
    Vector3 blob2Scale;
    Vector3 ballPosition;
    int blob1Score = 0;
    int blob2Score = 0;

    // Start is called before the first frame update
    void Start()
    {
        blob1Position = blob1.transform.position;
        blob2Position = blob2.transform.position;
        blob1Scale = blob1.transform.localScale;
        blob2Scale = blob2.transform.localScale;
        ballPosition = ball.transform.position;
        BeginGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && isPlaying == true)
        {
            Time.timeScale = 1;
            uiMessage.SetActive(false);
        }
    }

    void BeginGame()
    {
        Time.timeScale = 0;
        isPlaying = true;
        UpdateMessage("Press Enter to begin round");
    }

    public void ResetPositions(string winner)
    {
        ball.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        ball.GetComponent<Rigidbody2D>().angularVelocity = 0;
        if (winner == "Blob 1")
        {
            ball.transform.position = new Vector3(-ballPosition.x, ballPosition.y);
        } else
        {
            ball.transform.position = ballPosition;
        }
        blob1.transform.position = blob1Position;
        blob2.transform.position = blob2Position;
        blob1.transform.localScale = blob1Scale;
        blob2.transform.localScale = blob2Scale;
    }

    public void UpdateMessage(string message)
    {
        uiMessage.SetActive(true);
        uiMessage.GetComponent<Text>().text = message;
    }

    public void DisplayScore()
    {
        uiScore.GetComponent<Text>().text = blob1Score + " - " + blob2Score;
    }

    public void PlayerWins(string player)
    {
        if (player == "Blob 1") {
            blob1Score++;
        } else
        {
            blob2Score++;
        }
        DisplayScore();
        isPlaying = false;
        if(blob1Score >= 15 || blob2Score >= 15)
        {
            UpdateMessage("CONGRATULATIONS, " + player + ", you have won the GAME");
            StartCoroutine(GameOverCoroutine());
            return;
        }
        UpdateMessage(player + " wins the round");
        StartCoroutine(PlayerWinsCorountine(player));
    }

    IEnumerator PlayerWinsCorountine(string player)
    {
        yield return new WaitForSeconds(3);
        ResetPositions(player);
        eraseMessage();
        BeginGame();
    }

    IEnumerator GameOverCoroutine()
    {
        yield return new WaitForSeconds(3);
        eraseMessage();
        gameOver.SetActive(true);
    }

    void eraseMessage()
    {
        uiMessage.SetActive(false);
    }
}
