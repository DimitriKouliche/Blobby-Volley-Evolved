using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.Controls;
using Photon.Pun;
using Photon.Realtime;

public class GameLogics : MonoBehaviourPun
{
    public GameObject uiMessage;
    public GameObject uiScore;
    public GameObject gameOver;
    public GameObject blob1Prefab;
    public GameObject blob2Prefab;
    public GameObject blob3Prefab;
    public GameObject blob4Prefab;
    public GameObject blobOnline1Prefab;
    public GameObject blobOnline2Prefab;
    public GameObject ball;
    public bool isStarting = false;
    public bool isPlaying = false;
    public bool isOnline = true;
    public float timeScale = 0.5f;
    public int maxPlayers = 2;

    GameObject blob1;
    GameObject blob2;
    GameObject blob3;
    GameObject blob4;
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
        ResetBlobs();
        ResetVelocity(ball);
        if (winner == "Blob 2")
        {
            ball.transform.position = new Vector3(-ballPosition.x, ballPosition.y, -2);
        }
        else
        {
            ball.transform.position = ballPosition;
        }
    }

    public void ResetBlobs()
    {
        blob1.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.black;
        blob2.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.black;
        ResetVelocity(blob1);
        ResetVelocity(blob2);
        blob1.transform.position = blobPosition[0];
        blob2.transform.position = blobPosition[1];
        blob1.transform.localScale = blobScale[0];
        blob2.transform.localScale = blobScale[1];
        if (maxPlayers == 4)
        {
            ResetVelocity(blob3);
            ResetVelocity(blob4);
            blob3.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.black;
            blob4.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.black;
            blob3.transform.position = blobPosition[2];
            blob4.transform.position = blobPosition[3];
            blob3.transform.localScale = blobScale[2];
            blob4.transform.localScale = blobScale[3];
        }

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

        if (player == "Blob 1" || player == "Blob 3")
        {
            blob1Score++;
        }
        else
        {
            blob2Score++;
        }
        ResetBlobs();
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
        blobPosition = new Vector3[4];
        blobScale = new Vector3[4];
        PhotonNetwork.MinimalTimeScaleToDispatchInFixedUpdate = 0;
        PhotonNetwork.SendRate = 15;
        PhotonNetwork.SerializationRate = 15;
        if (!isOnline)
        {
            PhotonNetwork.OfflineMode = true;
        }
        Time.timeScale = 0;

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

                if (isOnline && GameObject.Find("Blob 1(Clone)") != null)
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        blob1.GetComponent<PlayerInput>().SwitchCurrentControlScheme("GamePad", control.device);
                    }
                    else
                    {
                        blob2.GetComponent<PlayerInput>().SwitchCurrentControlScheme("GamePad", control.device);
                    }
                    return;
                }
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
                else if (nbPlayer == 2)
                {
                    blob3 = PlayerInput.Instantiate(blob3Prefab, pairWithDevice: control.device).gameObject;
                }
                else if (nbPlayer == 3)
                {
                    blob4 = PlayerInput.Instantiate(blob4Prefab, pairWithDevice: control.device).gameObject;
                }
                nbPlayer++;
            };

        if (isOnline)
        {
            if (PlayerController.LocalPlayerInstance != null)
            {
                return;
            }
            if (PhotonNetwork.IsMasterClient)
            {
                blob1 = PhotonNetwork.Instantiate(blobOnline1Prefab.name, blob1Prefab.transform.position, blob1Prefab.transform.rotation, 0);
            }
            else
            {
                blob2 = PhotonNetwork.Instantiate(blobOnline2Prefab.name, blob2Prefab.transform.position, blob2Prefab.transform.rotation, 0);
            }
        }
        ballPosition = ball.transform.position;
        StartCoroutine(PlayersReady());
    }


    void InitBlob(GameObject blob, int id)
    {
        if (!isOnline)
        {
            blob.GetComponent<PlayerController>().gameLogics = gameObject;
        }
        else
        {
            blob.GetComponent<OnlinePlayerController>().gameLogics = gameObject;
        }
        blob.transform.GetChild(0).GetComponent<EyeLogics>().ball = ball;
        blobPosition[id] = blob.transform.position;
        blobScale[id] = blob.transform.localScale;
    }

    IEnumerator PlayersReady()
    {
        if (!isOnline)
        {
            if (maxPlayers == 2)
            {
                while (GameObject.Find("Blob 1(Clone)") == null || GameObject.Find("Blob 2(Clone)") == null)
                {
                    yield return null;
                }
            }
            if (maxPlayers == 4)
            {
                while (GameObject.Find("Blob 1(Clone)") == null || GameObject.Find("Blob 2(Clone)") == null || GameObject.Find("Blob 3(Clone)") == null || GameObject.Find("Blob 4(Clone)") == null)
                {
                    yield return null;
                }
            }
        }
        else
        {
            while (GameObject.Find("BlobOnline 1(Clone)") == null || GameObject.Find("BlobOnline 2(Clone)") == null)
            {
                yield return null;
            }
            GameObject.Find("BlobOnline 1(Clone)").name = "Blob 1(Clone)";
            GameObject.Find("BlobOnline 2(Clone)").name = "Blob 2(Clone)";
        }

        blob1 = GameObject.Find("Blob 1(Clone)");
        blob2 = GameObject.Find("Blob 2(Clone)");
        InitBlob(blob1, 0);
        InitBlob(blob2, 1);

        if (maxPlayers == 4)
        {
            blob3 = GameObject.Find("Blob 3(Clone)");
            blob4 = GameObject.Find("Blob 4(Clone)");
            InitBlob(blob3, 2);
            InitBlob(blob4, 3);
        }
        ball.SetActive(true);
        BeginGame();
    }

    public void SendStartRoundMessage()
    {
        if (!isOnline)
        {
            StartRound();
            return;
        }
        photonView.RPC("StartRound", RpcTarget.All);
    }

    [PunRPC]
    public void StartRound()
    {
        Time.timeScale = timeScale;
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
