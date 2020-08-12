using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.Controls;
using System;

public class GameLogics : MonoBehaviour
{
    public GameObject uiMessage;
    public GameObject uiScore;
    public GameObject gameOver;
    public GameObject blobPrefab;
    public GameObject ball;
    public GameObject ballSupport;
    public bool isStarting = false;
    public bool isPlaying = false;
    public bool isOnline = true;
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
    private bool serve = true;

    public void ResetVelocity(GameObject target)
    {
        target.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        target.GetComponent<Rigidbody2D>().angularVelocity = 0;
    }


    public void ResetPositions(string winner)
    {
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
        Debug.Log("Resetting player 1");
        Debug.Log(player1Device);
        InitBlob(blob1, 0);
        if (maxPlayers == 2)
        {
            Destroy(blob2);
            blob2 = PlayerInput.Instantiate(blobPrefab, pairWithDevice: player2Device).gameObject;
            Debug.Log("Resetting player 2");
            Debug.Log(player2Device);
            InitBlob(blob2, 1);
        }
        else
        {
            Destroy(blob2);
            blob2 = PlayerInput.Instantiate(blobPrefab, pairWithDevice: player2Device).gameObject;
            Debug.Log("Resetting player 2");
            Debug.Log(player2Device);
            InitBlob(blob2, 1);
            Destroy(blob3);
            blob3 = PlayerInput.Instantiate(blobPrefab, pairWithDevice: player3Device).gameObject;
            Debug.Log("Resetting player 3");
            Debug.Log(player3Device);
            InitBlob(blob3, 2);
            Destroy(blob4);
            blob4 = PlayerInput.Instantiate(blobPrefab, pairWithDevice: player4Device).gameObject;
            Debug.Log("Resetting player 4");
            Debug.Log(player4Device);
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
                UpdateMessage("CONGRATULATIONS, Team " + (GetTeamFromPlayer(ExtractIDFromName(player)) + 1) + ", you have won the GAME");

            }
            else
            {
                UpdateMessage("CONGRATULATIONS, " + player + ", you have won the GAME");
            }
            StartCoroutine(GameOverCoroutine());
            return;
        }
        if (maxPlayers == 4)
        {
            UpdateMessage("Team " + (GetTeamFromPlayer(ExtractIDFromName(player)) + 1) + " wins the round");

        }
        else
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
        blob1.transform.position = new Vector3(-10, blob1.transform.position.y, blob1.transform.position.z);
        if (blob2 != null)
        {
            if (maxPlayers == 2)
            {
                blob2.transform.position = new Vector3(10, blob2.transform.position.y, blob2.transform.position.z);
                Transform smash = blob2.transform.Find("Smash").transform;
                smash.localPosition = new Vector3(-smash.localPosition.x, smash.localPosition.y, smash.localPosition.z);
                smash.localScale = new Vector3(-smash.localScale.x, smash.localScale.y, smash.localScale.z);
            }
            else
            {
                blob2.transform.position = new Vector3(-5, blob2.transform.position.y, blob2.transform.position.z);
            }

        }
        if (blob3 != null)
        {
            blob3.transform.position = new Vector3(5, blob3.transform.position.y, blob3.transform.position.z);
            Transform smash = blob3.transform.Find("Smash").transform;
            smash.localPosition = new Vector3(-smash.localPosition.x, smash.localPosition.y, smash.localPosition.z);
            smash.localScale = new Vector3(-smash.localScale.x, smash.localScale.y, smash.localScale.z);
        }
        if (blob4 != null)
        {
            blob4.transform.position = new Vector3(10, blob4.transform.position.y, blob4.transform.position.z);
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
        ToggleMovement(false);

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


                if (nbPlayer == maxPlayers)
                    return;

                // Spawn player and pair device. If the player's actions have control schemes
                // defined in them, PlayerInput will look for a compatible scheme automatically.
                if (nbPlayer == 0)
                {
                    blob1 = PlayerInput.Instantiate(blobPrefab, pairWithDevice: control.device).gameObject;
                    Debug.Log("Spawning player 1");
                    Debug.Log(control.device);
                    player1Device = control.device;
                }
                else if (nbPlayer == 1)
                {
                    blob2 = PlayerInput.Instantiate(blobPrefab, pairWithDevice: control.device).gameObject;
                    Debug.Log("Spawning player 2");
                    Debug.Log(control.device);
                    player2Device = control.device;
                }
                else if (nbPlayer == 2)
                {
                    blob3 = PlayerInput.Instantiate(blobPrefab, pairWithDevice: control.device).gameObject;
                    Debug.Log("Spawning player 3");
                    Debug.Log(control.device);
                    player3Device = control.device;
                }
                else if (nbPlayer == 3)
                {
                    blob4 = PlayerInput.Instantiate(blobPrefab, pairWithDevice: control.device).gameObject;
                    Debug.Log("Spawning player 4");
                    Debug.Log(control.device);
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
        blob.name = "Blob " + (id + 1) + "(Clone)";
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
        if (nbPlayer != maxPlayers)
        {
            return;
        }
        StartRound();
    }

    public void StartRound()
    {
        isPlaying = true;
        uiMessage.SetActive(false);
    }

    void ToggleMovement(bool shouldMove)
    {
        ball.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
    }

    void BeginGame()
    {
        Debug.Log("Beginning Game");
        ToggleMovement(false);
        serve = true;
        ballSupport.SetActive(true);
        isStarting = true;
        SendStartRoundMessage();
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

    public int GetTeamFromPlayer(int playerId)
    {
        if(maxPlayers == 4)
        {
            return playerId < 2 ? 0 : 1;
        } else
        {
            return playerId;
        }
    }

    public void PlayerTouchesBall(GameObject player)
    {
        if (!isStarting || !isPlaying)
        {
            return;
        }
        int playerId = ExtractIDFromName(player.name) - 1;
        int teamId = GetTeamFromPlayer(playerId);
        int otherTeamId = teamId == 0 ? 1 : 0;
        playerBallTouches[playerId]++;
        teamBallTouches[teamId]++;
        teamBallTouches[otherTeamId] = 0;
        if (maxPlayers == 4)
        {
            for (int i = 0; i < 4; i++)
            {
                if (i != playerId)
                {
                    playerBallTouches[i] = 0;
                }
            }
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
        if (teamBallTouches[teamId] > 3)
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
    public void PlayerServes(GameObject player)
    {
        if (!isStarting || !isPlaying || !serve)
        {
            return;
        }
        int playerId = ExtractIDFromName(player.name) - 1;
        int teamId = GetTeamFromPlayer(playerId);
        if (serve)
        {
            teamBallTouches[teamId] = 2;
        }
        serve = false;
        ballSupport.SetActive(false);
    }

    int ExtractIDFromName(string name)
    {
        return Convert.ToInt32(new string(name[5], 1));
    }
}
