using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using Cubequad.Tentacles2D;
using System;
using UnityEngine.SceneManagement;

public class GameLogics : MonoBehaviour
{
    public GameObject uiMessage;
    public GameObject uiScore;
    public GameObject gameOver;
    public GameObject blobPrefab;
    public GameObject playerSelectionPrefab;
    public GameObject ball;
    public GameObject level;
    public GameObject ballSupport;
    public GameObject selectionMenu;
    public GameObject startAnimation;
    public GameObject UISound;
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
    bool[] playerReady = { false, false, false, false };
    Color[] blobColor = new Color[4];
    Sprite[] blobSprite = new Sprite[4];

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
    }
    

    public void ResetBlobPositions()
    {
        blob1.transform.position = new Vector3(-10, -6.3f, 2);
        if (maxPlayers == 2)
        {
            blob2.transform.position = new Vector3(10, -6.3f, 2);
        } else
        {
            blob2.transform.position = new Vector3(-5, -6.3f, 2);
            blob3.transform.position = new Vector3(5, -6.3f, 2);
            blob4.transform.position = new Vector3(10, -6.3f, 2);
        }
    }

    public void ResetBlobs()
    {
        Destroy(blob1);
        blob1 = PlayerInput.Instantiate(blobPrefab, pairWithDevice: player1Device).gameObject;
        if(blob1.GetComponent<PlayerInput>().currentControlScheme == null)
        {
            blob1.GetComponent<PlayerInput>().SwitchCurrentControlScheme("Keyboard&Mouse");
        }
        InitBlob(blob1, 0);
        if (maxPlayers == 2)
        {
            Destroy(blob2);
            blob2 = PlayerInput.Instantiate(blobPrefab, pairWithDevice: player2Device).gameObject;
            if (blob2.GetComponent<PlayerInput>().currentControlScheme == null)
            {
                blob2.GetComponent<PlayerInput>().SwitchCurrentControlScheme("Keyboard&Mouse");
            }
            InitBlob(blob2, 1);
        }
        else
        {
            Destroy(blob2);
            blob2 = PlayerInput.Instantiate(blobPrefab, pairWithDevice: player2Device).gameObject;
            if (blob2.GetComponent<PlayerInput>().currentControlScheme == null)
            {
                blob2.GetComponent<PlayerInput>().SwitchCurrentControlScheme("Keyboard&Mouse");
            }
            InitBlob(blob2, 1);
            Destroy(blob3);
            blob3 = PlayerInput.Instantiate(blobPrefab, pairWithDevice: player3Device).gameObject;
            if (blob3.GetComponent<PlayerInput>().currentControlScheme == null)
            {
                blob3.GetComponent<PlayerInput>().SwitchCurrentControlScheme("Keyboard&Mouse");
            }
            InitBlob(blob3, 2);
            Destroy(blob4);
            blob4 = PlayerInput.Instantiate(blobPrefab, pairWithDevice: player4Device).gameObject;
            if (blob4.GetComponent<PlayerInput>().currentControlScheme == null)
            {
                blob4.GetComponent<PlayerInput>().SwitchCurrentControlScheme("Keyboard&Mouse");
            }
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
        if (blob1Score >= 1 || blob2Score >= 1)
        {
            GameOver(player);
            return;
        }
        if (maxPlayers == 4)
        {
            if (blob1Score == 14)
            {
                UpdateMessage("Team " + (GetTeamFromPlayer(ExtractIDFromName(player)) + 1) + " wins the round! Match point for team 1!");
            } else if (blob2Score == 14)
            {
                UpdateMessage("Team " + (GetTeamFromPlayer(ExtractIDFromName(player)) + 1) + " wins the round! Match point for team 2!");
            } else
            {
                UpdateMessage("Team " + (GetTeamFromPlayer(ExtractIDFromName(player)) + 1) + " wins the round");
            }
        }
        else
        {
            if (blob1Score == 14)
            {
                UpdateMessage(player + " wins the round! Match point for player 1!");
            }
            else if (blob2Score == 14)
            {
                UpdateMessage(player + " wins the round! Match point for player 2!");
            }
            else
            {
                UpdateMessage(player + " wins the round");
            }
        }

        if (blob1Score == 14 || blob2Score == 14)
        {
            FindChild(level, "Background").SetActive(false);
            FindChild(level, "RedBackground").SetActive(true);
        }
        StartCoroutine(PlayerWinsCorountine(player));
        ball.GetComponent<BallLogics>().UpdateBall(2);
    }

    public void RestartGame()
    {
        ResetPositions("Blob 1");
        ResetBlobPositions();
        isStarting = false;
        isPlaying = false;
        ball.GetComponent<BallLogics>().UpdateBall(2);
        blob1Score = 0;
        blob2Score = 0;
        DisplayScore();
        EraseMessage();
        BeginGame();
    }

    void OnApplicationQuit()
    {
        applicationQuit = true;
    }

    void ReplaceBlobs()
    {
        blob1.transform.position = new Vector3(-9.5f, blob1.transform.position.y, blob1.transform.position.z);
        if (blob2 != null)
        {
            if (maxPlayers == 2)
            {
                blob2.transform.position = new Vector3(9.5f, blob2.transform.position.y, blob2.transform.position.z);
                InvertSmash(blob2);
            }
            else
            {
                blob2.transform.position = new Vector3(-5, blob2.transform.position.y, blob2.transform.position.z);
            }
        }
        if (blob3 != null)
        {
            blob3.transform.position = new Vector3(5, blob3.transform.position.y, blob3.transform.position.z);
            InvertSmash(blob3);
        }
        if (blob4 != null)
        {
            blob4.transform.position = new Vector3(9.5f, blob4.transform.position.y, blob4.transform.position.z);
            InvertSmash(blob4);
        }
    }

    void InvertSmash(GameObject blob)
    {
        Transform smash;
        smash = blob.transform.Find("Smash").transform;
        smash.localPosition = new Vector3(-smash.localPosition.x, smash.localPosition.y, smash.localPosition.z);
        smash.localScale = new Vector3(-smash.localScale.x, smash.localScale.y, smash.localScale.z);
        smash = blob.transform.Find("SmashFreezeFrame").transform;
        smash.localPosition = new Vector3(-smash.localPosition.x, smash.localPosition.y, smash.localPosition.z);
        smash.localScale = new Vector3(-smash.localScale.x, smash.localScale.y, smash.localScale.z);
        smash = blob.transform.Find("SmashFreezeFrameWhite").transform;
        smash.localPosition = new Vector3(-smash.localPosition.x, smash.localPosition.y, smash.localPosition.z);
        smash.localScale = new Vector3(-smash.localScale.x, smash.localScale.y, smash.localScale.z);
        smash = blob.transform.Find("SmashImpact").transform;
        smash.localPosition = new Vector3(-smash.localPosition.x, smash.localPosition.y, smash.localPosition.z);
        smash.localScale = new Vector3(-smash.localScale.x, smash.localScale.y, smash.localScale.z);
        smash = blob.transform.Find("Bump").transform;
        smash.localPosition = new Vector3(-smash.localPosition.x, smash.localPosition.y, smash.localPosition.z);
        smash.localScale = new Vector3(-smash.localScale.x, smash.localScale.y, smash.localScale.z);
    }

    GameObject FindChild(GameObject parent, string name)
    {
        foreach (Transform t in parent.transform)
        {
            if (t.name == name)
            {
                return t.gameObject;
            }
        }
        return null;
    }

    void InstantiateSelectionMenu(int id, InputDevice device, Vector3 position, GameObject blob)
    {
        GameObject playerSelection = PlayerInput.Instantiate(playerSelectionPrefab, pairWithDevice: device).gameObject;
        FindChild(selectionMenu, "PlayerPressLabel" + id).transform.localScale = new Vector3(0, 0, 0);
        FindChild(playerSelection, "SquidCustomizer").SetActive(true);
        playerSelection.transform.position = position;
        playerSelection.GetComponent<UIController>().id = id;
        playerSelection.GetComponent<UIController>().blob = blob;
        playerSelection.GetComponent<UIController>().gameLogics = gameObject;
        playerSelection.transform.parent = selectionMenu.transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject uiSound = GameObject.Find("UISound(Clone)");
        if (uiSound == null)
        {
            GameObject.Instantiate(UISound);
        }
        Application.targetFrameRate = -1;
        QualitySettings.vSyncCount = 0;
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
                if (control.name == "buttonEast" || control.name == "escape")
                {
                    SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
                }
                if (control.name != "enter" && control.name != "start")
                {
                    return;
                }

                if (nbPlayer == maxPlayers)
                    return;

                if(this == null)
                {
                    return;
                }
                // Spawn player and pair device. If the player's actions have control schemes
                // defined in them, PlayerInput will look for a compatible scheme automatically.
                if (nbPlayer == 0)
                {
                    blob1 = PlayerInput.Instantiate(blobPrefab, pairWithDevice: control.device).gameObject;
                    blob1.SetActive(false);
                    player1Device = control.device;
                    if(maxPlayers == 2)
                    {
                        InstantiateSelectionMenu(1, player1Device, new Vector3(-5f, 0, 0), blob1);
                    }
                    if(maxPlayers == 4)
                    {
                        InstantiateSelectionMenu(1, player1Device, new Vector3(-10, 0, 0), blob1);
                    }
                }
                else if (nbPlayer == 1)
                {
                    blob2 = PlayerInput.Instantiate(blobPrefab, pairWithDevice: control.device).gameObject;
                    blob2.SetActive(false);
                    player2Device = control.device;
                    if (maxPlayers == 2)
                    {
                        InstantiateSelectionMenu(2, player2Device, new Vector3(4f, 0, 0), blob2);
                    }
                    if (maxPlayers == 4)
                    {
                        InstantiateSelectionMenu(2, player2Device, new Vector3(-4f, 0, 0), blob2);
                    }
                }
                else if (nbPlayer == 2)
                {
                    blob3 = PlayerInput.Instantiate(blobPrefab, pairWithDevice: control.device).gameObject;
                    blob3.SetActive(false);
                    player3Device = control.device;
                    InstantiateSelectionMenu(3, player3Device, new Vector3(2.25f, 0, 0), blob3);
                }
                else if (nbPlayer == 3)
                {
                    blob4 = PlayerInput.Instantiate(blobPrefab, pairWithDevice: control.device).gameObject;
                    blob4.SetActive(false);
                    player4Device = control.device;
                    InstantiateSelectionMenu(4, player4Device, new Vector3(8.5f, 0, 0), blob4);
                }
                nbPlayer++;
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
        ApplyColor(id + 1, blob);
        ApplyShape(id + 1, blob);
    }

    IEnumerator PlayersReady()
    {
        if(maxPlayers == 4)
        {
            while (!playerReady[0] || !playerReady[1] || !playerReady[2] || !playerReady[3])
            {
                yield return null;
            }
        }
        if(maxPlayers == 2)
        {
            while (!playerReady[0] || !playerReady[1])
            {
                yield return null;
            }
        }

        startAnimation.SetActive(true);
        FindChild(level, "Pole").SetActive(true);
        selectionMenu.SetActive(false);

        yield return new WaitForSeconds(1);

        blob1.SetActive(true);
        blob2.SetActive(true);
        if (maxPlayers == 4)
        {
            blob3.SetActive(true);
            blob4.SetActive(true);
            InitBlob(blob3, 2);
            InitBlob(blob4, 3);
        }
        ResetBlobs();
        InitBlob(blob1, 0);
        InitBlob(blob2, 1);
        blob1.GetComponent<PlayerController>().enabled = false;
        blob2.GetComponent<PlayerController>().enabled = false;
        if (maxPlayers == 4)
        {
            InitBlob(blob3, 2);
            InitBlob(blob4, 3);
            blob3.GetComponent<PlayerController>().enabled = false;
            blob4.GetComponent<PlayerController>().enabled = false;
        }
        if (maxPlayers == 2)
        {
            blob1.transform.position = FindChild(startAnimation, "CanonRight").transform.position;
            blob2.transform.position = FindChild(startAnimation, "CanonLeft").transform.position;
            blob1.transform.Rotate(new Vector3(0, 0, 0));
            blob2.transform.Rotate(new Vector3(0, 0, -90));
            blob1.GetComponent<Rigidbody2D>().AddForce(new Vector2(-160000, 160000));
            blob2.GetComponent<Rigidbody2D>().AddForce(new Vector2(160000, 160000));
        } else
        {
            blob1.transform.position = FindChild(startAnimation, "CanonRight").transform.position;
            blob2.transform.position = FindChild(startAnimation, "CanonRight").transform.position;
            blob3.transform.position = FindChild(startAnimation, "CanonLeft").transform.position;
            blob4.transform.position = FindChild(startAnimation, "CanonLeft").transform.position;
            blob1.transform.Rotate(new Vector3(0, 0, 0));
            blob2.transform.Rotate(new Vector3(0, 0, 0));
            blob3.transform.Rotate(new Vector3(0, 0, -90));
            blob4.transform.Rotate(new Vector3(0, 0, -90));
            blob1.GetComponent<Rigidbody2D>().AddForce(new Vector2(-160000, 160000));
            blob2.GetComponent<Rigidbody2D>().AddForce(new Vector2(-120000, 190000));
            blob3.GetComponent<Rigidbody2D>().AddForce(new Vector2(160000, 160000));
            blob4.GetComponent<Rigidbody2D>().AddForce(new Vector2(120000, 190000));
        }

        yield return new WaitForSeconds(0.5f);
        float t = 0.0f;
        while (t < 1.6f)
        {
            blob1.transform.Rotate(Vector3.Lerp(new Vector3(0,0,0), new Vector3(0,0,-2), t / 0.5f));
            blob2.transform.Rotate(Vector3.Lerp(new Vector3(0,0,0), new Vector3(0,0,-2), t / 0.5f));
            if (maxPlayers == 4)
            {
                blob3.transform.Rotate(Vector3.Lerp(new Vector3(0, 0, 0), new Vector3(0, 0, -2), t / 0.5f));
                blob4.transform.Rotate(Vector3.Lerp(new Vector3(0, 0, 0), new Vector3(0, 0, -2), t / 0.5f));
            }
            t += Time.deltaTime;
            yield return null;
        }

        blob1.transform.rotation = Quaternion.identity;
        blob2.transform.rotation = Quaternion.identity;
        if (maxPlayers == 4)
        {
            blob3.transform.rotation = Quaternion.identity;
            blob4.transform.rotation = Quaternion.identity;
        }
        FindChild(level, "PlayerBounds").SetActive(true);
        blob1.GetComponent<PlayerController>().enabled = true;
        blob2.GetComponent<PlayerController>().enabled = true;
        if (maxPlayers == 4)
        {
            blob3.GetComponent<PlayerController>().enabled = true;
            blob4.GetComponent<PlayerController>().enabled = true;
        }
        startAnimation.SetActive(false);

        ball.SetActive(true);

        uiMessage.transform.parent.parent.gameObject.SetActive(true);

        if (maxPlayers == 4)
        {
            blob3.SetActive(true);
            blob4.SetActive(true);
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
        ball.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        FindChild(ball, "Serve").SetActive(true);
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
        ResetBlobCharge();
        ToggleMovement(false);
        serve = true;
        ballSupport.SetActive(true);
        isStarting = true;
        ball.GetComponent<BallLogics>().service = true;
        SendStartRoundMessage();
    }

    void ResetBlobCharge()
    {
        if(blob1 != null)
        {
            blob1.GetComponent<PlayerController>().CancelCharge();
        }
        if(blob2 != null)
        {
            blob2.GetComponent<PlayerController>().CancelCharge();
        }
        if(blob3 != null)
        {
            blob3.GetComponent<PlayerController>().CancelCharge();
        }
        if(blob4 != null)
        {
            blob4.GetComponent<PlayerController>().CancelCharge();
        }
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

    void GameOver(string player)
    {
        Destroy(ball);
        gameOver.SetActive(true);
        if (player == "Blob 1" || player == "Blob 3")
        {
            FindChild(gameOver, "Cup").transform.position = new Vector3(-5, 5, 0);
        }
        else
        {
            FindChild(gameOver, "Cup").transform.position = new Vector3(5, 5, 0);
        }
        isPlaying = true;
    }

    public int GetTeamFromPlayer(int playerId)
    {
        if (maxPlayers == 4)
        {
            return playerId < 2 ? 0 : 1;
        }
        else
        {
            return playerId;
        }
    }

    public int PlayerTouchesBall(GameObject player)
    {
        if (!isStarting || !isPlaying)
        {
            return 0;
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
        return teamBallTouches[teamId];
    }
    public void PlayerServes(GameObject player)
    {
        if (!isStarting || !isPlaying || !serve)
        {
            return;
        }
        FindChild(ball, "Serve").SetActive(false);
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

    public void PlayerReady(int id, Color c, Sprite s)
    {
        playerReady[id - 1] = true;
        blobSprite[id - 1] = s;
        blobColor[id - 1] = c;
    }

    void ApplyColor(int id, GameObject blob)
    {
        FindChild(blob, "SpriteBlob").GetComponent<SpriteRenderer>().color = blobColor[id-1];

        foreach (Transform child in FindChild(FindChild(blob, "SpriteBlob"), "Tentacles").transform)
        {
            child.gameObject.GetComponent<Tentacle>().Color = blobColor[id-1];
        }
        FindChild(FindChild(blob, "Smash"), "SmashAnimation").GetComponent<SpriteRenderer>().color = blobColor[id - 1];
        FindChild(FindChild(blob, "Bump"), "BumpAnimation").GetComponent<SpriteRenderer>().color = blobColor[id - 1];
        FindChild(blob, "SmashFreezeFrame").GetComponent<SpriteRenderer>().color = blobColor[id - 1];

        ParticleSystem.MainModule settings = FindChild(blob, "ParticleContact").GetComponent<ParticleSystem>().main;
        settings.startColor = new ParticleSystem.MinMaxGradient(blobColor[id - 1]);
        settings = FindChild(blob, "InkStains").GetComponent<ParticleSystem>().main;
        settings.startColor = new ParticleSystem.MinMaxGradient(blobColor[id - 1]);
    }

    void ApplyShape(int id, GameObject blob)
    {
        FindChild(blob, "SpriteBlob").GetComponent<SpriteRenderer>().sprite = blobSprite[id-1];
    }
}
