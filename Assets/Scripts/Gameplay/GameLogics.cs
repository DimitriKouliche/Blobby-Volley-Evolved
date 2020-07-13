using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.Controls;
using Photon.Pun;

public class GameLogics : MonoBehaviourPun
{
    public GameObject uiMessage;
    public GameObject uiScore;
    public GameObject gameOver;
    public GameObject blob1Prefab;
    public GameObject blob2Prefab;
    public GameObject ball;
    public bool isStarting = false;
    public bool isPlaying = false;
    public bool isOnline = true;

    GameObject blob1;
    GameObject blob2;
    Vector3[] blobPosition;
    Vector3[] blobScale;
    Vector3 ballPosition;
    int blob1Score = 0;
    int blob2Score = 0;
    int nbPlayer = 0;
    bool applicationQuit = false;

    public void ResetVelocity(GameObject target)
    {
        target.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        target.GetComponent<Rigidbody2D>().angularVelocity = 0;
    }


    public void ResetPositions(string winner)
    {
        ResetVelocity(blob1);
        ResetVelocity(blob2);
        ResetVelocity(ball);
        blob1.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.black;
        blob2.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.black;
        if (winner == "Blob 2")
        {
            ball.transform.position = new Vector3(-ballPosition.x, ballPosition.y, -2);
        }
        else
        {
            ball.transform.position = ballPosition;
        }
        blob1.transform.position = blobPosition[0];
        blob2.transform.position = blobPosition[1];
        blob1.transform.localScale = blobScale[0];
        blob2.transform.localScale = blobScale[1];
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
        if (player == "Blob 1")
        {
            blob1Score++;
        }
        else
        {
            blob2Score++;
        }
        ResetVelocity(blob1);
        ResetVelocity(blob2);
        DisplayScore();
        isStarting = false;
        isPlaying = false;
        if (blob1Score >= 15 || blob2Score >= 15)
        {
            UpdateMessage("CONGRATULATIONS, " + player + ", you have won the GAME");
            StartCoroutine(GameOverCoroutine());
            return;
        }
        UpdateMessage(player + " wins the round");
        StartCoroutine(PlayerWinsCorountine(player));
    }

    void OnApplicationQuit()
    {
        applicationQuit = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        blobPosition = new Vector3[2];
        blobScale = new Vector3[2];
        PhotonNetwork.MinimalTimeScaleToDispatchInFixedUpdate = 0;
        if(!isOnline)
        {
            PhotonNetwork.OfflineMode = true;
        }
        Time.timeScale = 0;

        if (!isOnline)
        {
            ++InputUser.listenForUnpairedDeviceActivity;
            // Example of how to spawn a new player automatically when a button
            // is pressed on an unpaired device.
            InputUser.onUnpairedDeviceUsed +=
                (control, eventPtr) =>
                {
                    if (applicationQuit || isStarting)
                    {
                        return;
                    }

                    // Ignore anything but button presses.
                    if (!(control is ButtonControl))
                        return;

                    // Spawn player and pair device. If the player's actions have control schemes
                    // defined in them, PlayerInput will look for a compatible scheme automatically.
                    if (nbPlayer == 0)
                    {
                        blob1 = PlayerInput.Instantiate(blob1Prefab, pairWithDevice: control.device).gameObject;
                    }
                    else if (nbPlayer == 1)
                    {
                        blob2 = PlayerInput.Instantiate(blob2Prefab, pairWithDevice: control.device).gameObject;
                    }
                    nbPlayer++;
                };
        }
        else
        {
            if (PlayerController.LocalPlayerInstance != null)
            {
                return;
            }
            if (PhotonNetwork.IsMasterClient)
            {
                blob1 = PhotonNetwork.Instantiate(blob1Prefab.name, blob1Prefab.transform.position, blob1Prefab.transform.rotation, 0);
            }
            else
            {
                blob2 = PhotonNetwork.Instantiate(blob2Prefab.name, blob2Prefab.transform.position, blob2Prefab.transform.rotation, 0);
            }
        }
        ballPosition = ball.transform.position;
        StartCoroutine(PlayersReady());
    }


    void InitBlob(GameObject blob, int id)
    {
        blob.GetComponent<PlayerController>().gameLogics = gameObject;
        blob.transform.GetChild(0).GetComponent<EyeLogics>().ball = ball;
        blobPosition[id] = blob.transform.position;
        blobScale[id] = blob.transform.localScale;
    }

    IEnumerator PlayersReady()
    {
        while (GameObject.Find("Blob 1(Clone)") == null || GameObject.Find("Blob 2(Clone)") == null)
        {
            yield return null;
        }
        blob1 = GameObject.Find("Blob 1(Clone)");
        InitBlob(blob1, 0);
        blob2 = GameObject.Find("Blob 2(Clone)");
        InitBlob(blob2, 1);
        ball.SetActive(true);
        BeginGame();
    }

    public void SendStartRoundMessage()
    {
        if(!isOnline)
        {
            StartRound();
        }
        photonView.RPC("StartRound", RpcTarget.All);
    }

    [PunRPC]
    public void StartRound()
    {
        Time.timeScale = 1;
        isPlaying = true;
        uiMessage.SetActive(false);
    }

    void BeginGame()
    {
        Debug.Log("Beginning Game");
        Time.timeScale = 0;
        isStarting = true;
        UpdateMessage("Press Enter / Start to begin round");
    }

    void EraseMessage()
    {
        uiMessage.SetActive(false);
    }

    IEnumerator PlayerWinsCorountine(string player)
    {
        yield return new WaitForSeconds(3);
        ResetPositions(player);
        EraseMessage();
        BeginGame();
    }

    IEnumerator GameOverCoroutine()
    {
        yield return new WaitForSeconds(3);
        EraseMessage();
        gameOver.SetActive(true);
    }
}
