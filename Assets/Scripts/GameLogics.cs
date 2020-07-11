using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class GameLogics : MonoBehaviour
{
    public GameObject uiMessage;
    public GameObject gameOver;
    public GameObject blob1;
    public GameObject blob2;
    public GameObject ball;
    public bool isPlaying = false;
    
    Vector3 blob1Position;
    Vector3 blob2Position;
    Vector3 ballPosition;

    // Start is called before the first frame update
    void Start()
    {
        blob1Position = blob1.transform.position;
        blob2Position = blob2.transform.position;
        ballPosition = ball.transform.position;
        beginGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Time.timeScale = 1;
            uiMessage.SetActive(false);
        }
    }

    void beginGame()
    {
        Time.timeScale = 0;
        isPlaying = true;
        updateMessage("Press Enter to begin round");
    }

    public void resetPositions(string winner)
    {
        if(winner == "Blob 1")
        {
            ball.transform.position = new Vector3(-ballPosition.x, ballPosition.y);
        } else
        {
            ball.transform.position = ballPosition;
        }
        blob1.transform.position = blob1Position;
        blob2.transform.position = blob2Position;
    }

    public void updateMessage(string message)
    {
        uiMessage.SetActive(true);
        uiMessage.transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<Text>().text = message;
    }

    public void playerWins(string player)
    {
        isPlaying = false;
        updateMessage(player + " wins the round");
        StartCoroutine(PlayerWinsCorountine(player));
    }

    IEnumerator PlayerWinsCorountine(string player)
    {
        yield return new WaitForSeconds(3);
        resetPositions(player);
        eraseMessage();
        beginGame();
    }

    IEnumerator GameOverCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        gameOver.SetActive(true);
    }

    void eraseMessage()
    {
        uiMessage.SetActive(false);
    }
}
