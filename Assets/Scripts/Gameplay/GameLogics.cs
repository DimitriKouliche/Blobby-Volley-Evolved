using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.Controls;
using Photon.Pun;
using Photon.Realtime;
using System;

public class GameLogics : MonoBehaviourPun
{
    public GameObject uiMessage;
    public GameObject uiScore;
    public GameObject gameOver;
    public GameObject blobPrefab;
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
    InputDevice player1Device;
    InputDevice player2Device;
    InputDevice player3Device;
    InputDevice player4Device;
    int[] teamBallTouches = new int[2];
    int[] playerBallTouches = new int[4];

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

        Camera.main.transform.position = new Vector3(ball.transform.position.x / 17, 0, -10);
    }

    public void ResetBlobs()
    {
        Destroy(blob1);
        blob1 = PlayerInput.Instantiate(blobPrefab, pairWithDevice: player1Device).gameObject;
        InitBlob(blob1, 0);
        if (maxPlayers == 2)
        {
            Destroy(blob2);
            blob2 = PlayerInput.Instantiate(blobPrefab, pairWithDevice: player2Device).gameObject;
            InitBlob(blob2, 1);
        }
        else
        {
            Destroy(blob2);
            blob2 = PlayerInput.Instantiate(blobPrefab, pairWithDevice: player2Device).gameObject;
            InitBlob(blob2, 1);
            Destroy(blob3);
            blob3 = PlayerInput.Instantiate(blobPrefab, pairWithDevice: player3Device).gameObject;
            InitBlob(blob3, 2);
            Destroy(blob4);
            blob4 = PlayerInput.Instantiate(blobPrefab, pairWithDevice: player4Device).gameObject;
            InitBlob(blob4, 3);
        }
        ReplaceBlobs();
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
        isStarting = false;
        isPlaying = false;
        for (int i = 0; i < 4; i++)
        {
            playerBallTouches[i] = 0;
        }
        for (int i = 0; i < 2; i++)
        {
            teamBallTouches[i] = 0;
        }
        if (player == "Blob 1" || player == "Blob 3")
        {
            blob1Score++;
        }
        else
        {
            blob2Score++;
        }
        DisplayScore();
        if (blob1Score >= 15 || blob2Score >= 15)
        {
            if (maxPlayers == 4)
            {
                UpdateMessage("CONGRATULATIONS, Team " + ((ExtractIDFromName(player) + 1) % 2 + 1) + ", you have won the GAME");

            }
            else
            {
                UpdateMessage("CONGRATULATIONS, " + player + ", you have won the GAME");
            }
            StartCoroutine(GameOverCoroutine());
            return;
        }
        if(maxPlayers == 4)
        {
            UpdateMessage("Team " + ((ExtractIDFromName(player) + 1) % 2 + 1) + " wins the round");

        } else
        {
            UpdateMessage(player + " wins the round");
        }
        StartCoroutine(PlayerWinsCorountine(player));
    }

    void OnApplicationQuit()
    {
        applicationQuit = true;
    }

    void ReplaceBlobs()
    {
        blob1.transform.position = new Vector3(-6, blob1.transform.position.y, blob1.transform.position.z);
        if (blob2 != null)
        {
            if (maxPlayers == 2)
            {
                blob2.transform.position = new Vector3(6, blob2.transform.position.y, blob2.transform.position.z);
                Transform smash = blob2.transform.Find("Smash").transform;
                smash.localPosition = new Vector3(-smash.localPosition.x, smash.localPosition.y, smash.localPosition.z);
                smash.localScale = new Vector3(-smash.localScale.x, smash.localScale.y, smash.localScale.z);
            }
            else
            {
                blob2.transform.position = new Vector3(-3, blob2.transform.position.y, blob2.transform.position.z);
            }

        }
        if (blob3 != null)
        {
            blob3.transform.position = new Vector3(3, blob3.transform.position.y, blob3.transform.position.z);
            Transform smash = blob3.transform.Find("Smash").transform;
            smash.localPosition = new Vector3(-smash.localPosition.x, smash.localPosition.y, smash.localPosition.z);
            smash.localScale = new Vector3(-smash.localScale.x, smash.localScale.y, smash.localScale.z);
        }
        if (blob4 != null)
        {
            blob4.transform.position = new Vector3(6, blob4.transform.position.y, blob4.transform.position.z);
            Transform smash = blob4.transform.Find("Smash").transform;
            smash.localPosition = new Vector3(-smash.localPosition.x, smash.localPosition.y, smash.localPosition.z);
            smash.localScale = new Vector3(-smash.localScale.x, smash.localScale.y, smash.localScale.z);
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        blobPosition = new Vector3[4];
        blobScale = new Vector3[4];
        PhotonNetwork.MinimalTimeScaleToDispatchInFixedUpdate = 0;
        PhotonNetwork.SendRate = 15;
        PhotonNetwork.SerializationRate = 15;
        PhotonNetwork.OfflineMode = true;
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

                if (nbPlayer == maxPlayers)
                    return;

                // Spawn player and pair device. If the player's actions have control schemes
                // defined in them, PlayerInput will look for a compatible scheme automatically.
                if (nbPlayer == 0)
                {
                    blob1 = PlayerInput.Instantiate(blobPrefab, pairWithDevice: control.device).gameObject;
                    player1Device = control.device;
                }
                else if (nbPlayer == 1)
                {
                    blob2 = PlayerInput.Instantiate(blobPrefab, pairWithDevice: control.device).gameObject;
                    player2Device = control.device;
                }
                else if (nbPlayer == 2)
                {
                    blob3 = PlayerInput.Instantiate(blobPrefab, pairWithDevice: control.device).gameObject;
                    player3Device = control.device;
                }
                else if (nbPlayer == 3)
                {
                    blob4 = PlayerInput.Instantiate(blobPrefab, pairWithDevice: control.device).gameObject;
                    player4Device = control.device;
                }
                nbPlayer++;
                ReplaceBlobs();
            };
        ballPosition = ball.transform.position;
        StartCoroutine(PlayersReady());
    }


    void InitBlob(GameObject blob, int id)
    {
        blob.name = "Blob " + (id+1) + "(Clone)";
        blob.GetComponent<PlayerController>().gameLogics = gameObject;
        blob.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<EyeLogics>().ball = ball;
        blobPosition[id] = blob.transform.position;
        blobScale[id] = blob.transform.localScale;
    }

    IEnumerator PlayersReady()
    {
        while (GameObject.FindGameObjectsWithTag("Player").Length < maxPlayers)
        {
            yield return null;
        }

        InitBlob(blob1, 0);
        InitBlob(blob2, 1);

        if (maxPlayers == 4)
        {
            InitBlob(blob3, 2);
            InitBlob(blob4, 3);
        }
        ball.SetActive(true);
        BeginGame();
    }

    public void SendStartRoundMessage()
    {
        if(nbPlayer != maxPlayers)
        {
            return;
        }
        StartRound();
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

    public void PlayerTouchesBall(GameObject player)
    {
        if(!isStarting || !isPlaying)
        {
            return;
        }
        int playerId = ExtractIDFromName(player.name) - 1;
        int teamId = playerId % 2 == 0 ? 0 : 1;
        playerBallTouches[playerId]++;
        teamBallTouches[teamId]++;
        if(maxPlayers == 4)
        {
            if (playerBallTouches[playerId] > 1)
            {
                if (teamId == 0)
                {
                    PlayerWins("Blob 2");
                }
                else
                {
                    PlayerWins("Blob 1");
                }
            }
        }
        if (teamBallTouches[teamId] > 3) {
            if(teamId == 0)
            {
                PlayerWins("Blob 2");
            } else
            {
                PlayerWins("Blob 1");
            }
        }
    }

    int ExtractIDFromName(string name)
    {
        return Convert.ToInt32(new string(name[5], 1));
    }
}
