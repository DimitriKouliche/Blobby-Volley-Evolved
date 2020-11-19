using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using Cubequad.Tentacles2D;
using System;
using UnityEngine.SceneManagement;
using Steamworks;

public class GameLogics : MonoBehaviour
{
    public GameObject uiMessage;
    public GameObject uiScoreLeft;
    public GameObject uiScoreRight;
    public GameObject gameOver;
    public GameObject blobPrefab;
    public GameObject AIBlob;
    public GameObject playerSelectionPrefab;
    public GameObject ball;
    public GameObject ballAnimation;
    public GameObject flipGAnimation;
    public GameObject flipDAnimation;
    public GameObject matchMessage;
    public GameObject pointMessage;
    public GameObject readyMessage;
    public GameObject setMessage;
    public GameObject goMessage;
    public GameObject level;
    public GameObject ballSupport;
    public GameObject selectionMenu;
    public GameObject startAnimation;
    public GameObject UISound;
    public GameObject musicMixerPrefab;
    public bool isStarting = false;
    public bool isPlaying = false;
    public bool isOnline = true;
    public int maxPlayers = 2;
    public int winningScore = 15;
    public int[] teamBallTouches = new int[2];

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
    int[] playerBallTouches = new int[4];
    private bool serve = true;
    bool[] playerReady = { false, false, false, false };
    Color[] blobColor = new Color[4];
    Sprite[] blobSprite = new Sprite[4];
    GameObject musicMixer;
    bool backgroundHasChanged = false;
    float roundTime = 0;

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
        if (maxPlayers == 2 || maxPlayers == 1)
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
        if (maxPlayers == 1)
        {
        }
        else if (maxPlayers == 2)
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
        return;
    }

    public void DisplayScore()
    {
        uiScoreLeft.GetComponent<Text>().text = blob1Score.ToString();
        uiScoreRight.GetComponent<Text>().text = blob2Score.ToString();
    }

    public void PlayerWins(string player)
    {
        if (roundTime > 300)
        {
            if (SteamManager.Initialized)
            {
                SteamUserStats.SetAchievement("LONG_PLAY");
                SteamUserStats.StoreStats();
            }
        }
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
            flipGAnimation.SetActive(true);
            blob1Score++;
        }
        else
        {
            flipDAnimation.SetActive(true);
            blob2Score++;
        }
        GameObject.Find("Music(Clone)").GetComponent<MusicMixer>().SwitchMusic(Mathf.Max(blob1Score, blob2Score));
        DisplayScore();
        if (blob1Score >= winningScore || blob2Score >= winningScore)
        {
            if (SteamManager.Initialized)
            {
                if(maxPlayers == 4)
                {
                    SteamUserStats.SetAchievement("TEAMWORK");
                } else if(maxPlayers == 2)
                {
                    SteamUserStats.SetAchievement("DUEL");
                } else if (blob1Score >= winningScore)
                {
                    SteamUserStats.SetAchievement("TERMINATOR");
                }
                SteamUserStats.StoreStats();
            }

            if(blob1Score == 0 || blob2Score == 0)
            {
                if (SteamManager.Initialized)
                {
                    SteamUserStats.SetAchievement("NO_MERCY");
                    SteamUserStats.StoreStats();
                }
            }
            StartCoroutine(GameOver(player));
            return;
        }
        if (maxPlayers == 4)
        {
            if (blob1Score == winningScore - 1)
            {
                UpdateMessage("Team " + (GetTeamFromPlayer(ExtractIDFromName(player)) + 1) + " wins the round! Match point for team 1!");
            } else if (blob2Score == winningScore - 1)
            {
                UpdateMessage("Team " + (GetTeamFromPlayer(ExtractIDFromName(player)) + 1) + " wins the round! Match point for team 2!");
            } else
            {
                UpdateMessage("Team " + (GetTeamFromPlayer(ExtractIDFromName(player)) + 1) + " wins the round");
            }
        }
        else
        {
            if (blob1Score == winningScore - 1)
            {
                UpdateMessage(player + " wins the round! Match point for player 1!");
            }
            else if (blob2Score == winningScore - 1)
            {
                UpdateMessage(player + " wins the round! Match point for player 2!");
            }
            else
            {
                UpdateMessage(player + " wins the round");
            }
        }

        if (blob1Score == winningScore - 1 || blob2Score == winningScore - 1)
        {
            StartCoroutine(MatchPoint(player));
            FindChild(level, "Background").SetActive(false);
            FindChild(level, "RedBackground").SetActive(true);
        }
        StartCoroutine(PlayerWinsCorountine(player));
    }

    public void ChangeBackground()
    {
        if ((blob1Score < winningScore/2 && blob2Score < winningScore/2) || backgroundHasChanged)
        {
            return;
        }
        int i = 0;
        Transform[] childs = new Transform[50];
        Transform alternativeBackground = FindChild(FindChild(level, "Background"), "BackgroundAlt").transform;
        foreach (Transform child in alternativeBackground)
        {

            childs[i] = child;
            i++;
        }
        int randomBackground = UnityEngine.Random.Range(0, i);
        PlayerPrefs.SetInt("Background" + randomBackground, 1);
        bool sightseer = true;
        for (int j = 0; j < i; j++)
        {
            if (PlayerPrefs.GetInt("Background" + j, 0) == 0)
            {
                sightseer = false;
                break;
            }
        }
        if(sightseer)
        {
            if (SteamManager.Initialized)
            {
                SteamUserStats.SetAchievement("SIGHTSEEING");
                SteamUserStats.StoreStats();
            }
        }
        childs[randomBackground].gameObject.SetActive(true);
        StartCoroutine(FadeBackground());
        backgroundHasChanged = true;
    }

    IEnumerator FadeBackground()
    {
        Material mat = FindChild(level, "Background").GetComponent<SpriteRenderer>().material;
        float t = 0.0f;
        while (t < 2f)
        {
            mat.SetFloat("_Fade", Mathf.Lerp(1, 0, t / 2));
            t += Time.deltaTime;
            yield return null;
        }
    }

    public void RestartGame()
    {
        Transform alternativeBackground = FindChild(FindChild(level, "Background"), "BackgroundAlt").transform;
        foreach (Transform child in alternativeBackground)
        {
            child.gameObject.SetActive(false);
        }
        backgroundHasChanged = false;
        FindChild(level, "Background").GetComponent<SpriteRenderer>().material.SetFloat("_Fade", 1);
        GameObject.Find("Music(Clone)").GetComponent<MusicMixer>().RestartGameMusic();
        FindChild(gameOver, "Cup").GetComponent<Rigidbody2D>().gravityScale = 0.05f;
        FindChild(gameOver, "Cup").SetActive(false);
        FindChild(gameOver, "Confetti").GetComponent<ParticleSystem>().Play();
        FindChild(gameOver, "Confetti").SetActive(false);
        FindChild(gameOver, "Cup").transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        FindChild(FindChild(blob1, "SpriteBlob"), "EyesWhite").SetActive(true);
        FindChild(FindChild(blob2, "SpriteBlob"), "EyesWhite").SetActive(true);
        FindChild(FindChild(blob1, "SpriteBlob"), "SadEyes").SetActive(false);
        FindChild(FindChild(blob2, "SpriteBlob"), "SadEyes").SetActive(false);
        FindChild(FindChild(blob1, "SpriteBlob"), "HappyEyes").SetActive(false);
        FindChild(FindChild(blob2, "SpriteBlob"), "HappyEyes").SetActive(false);
        if (maxPlayers == 4)
        {
            FindChild(FindChild(blob3, "SpriteBlob"), "EyesWhite").SetActive(true);
            FindChild(FindChild(blob4, "SpriteBlob"), "EyesWhite").SetActive(true);
            FindChild(FindChild(blob3, "SpriteBlob"), "SadEyes").SetActive(false);
            FindChild(FindChild(blob4, "SpriteBlob"), "SadEyes").SetActive(false);
            FindChild(FindChild(blob3, "SpriteBlob"), "HappyEyes").SetActive(false);
            FindChild(FindChild(blob4, "SpriteBlob"), "HappyEyes").SetActive(false);
        }

        FindChild(level, "Ceiling").SetActive(false);
        FindChild(level, "Background").SetActive(true);
        FindChild(gameOver, "PressToContinue").SetActive(false);
        FindChild(level, "RedBackground").SetActive(false);
        FindChild(FindChild(gameOver, "Blackout"), "Spotlight").SetActive(false);
        FindChild(level, "PlayerBounds").SetActive(true);
        ResetPositions("Blob 1");
        ResetBlobPositions();
        isStarting = false;
        isPlaying = false;
        ball.SetActive(true);
        gameOver.SetActive(false);
        matchMessage.SetActive(false);
        pointMessage.SetActive(false);
        ball.GetComponent<BallLogics>().UpdateBall(2);
        ball.GetComponent<BallLogics>().canHit = true;
        FindChild(gameOver, "Cup").GetComponent<BallLogics>().victoryMusicIsPlayed = false;
        ball.GetComponent<SpriteRenderer>().material.SetColor("_Color", new Color(1, 1, 1, 1));
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
            if (maxPlayers == 2 || maxPlayers == 1)
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

    private void Update()
    {
        if (isPlaying)
        {
            roundTime += Time.deltaTime;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject uiSound = GameObject.Find("UISound(Clone)");
        if (uiSound == null)
        {
            GameObject.Instantiate(UISound);
        }
        GameObject musicMixer = GameObject.Find("Music(Clone)");
        if (musicMixer == null)
        {
            musicMixer = GameObject.Instantiate(musicMixerPrefab);
        }
        musicMixer.GetComponent<MusicMixer>().MenuMusic();
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
                    if(maxPlayers == 1)
                    {
                        InstantiateSelectionMenu(1, player1Device, new Vector3(0, 0, 0), blob1);
                    }
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
        if (blob.GetComponent<PlayerController>() != null)
        {
            blob.GetComponent<PlayerController>().gameLogics = gameObject;
        }
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
        if(maxPlayers == 1)
        {
            while (!playerReady[0])
            {
                yield return null;
            }
        }

        for(int cpt = 0; cpt < maxPlayers; cpt++)
        {
            if (blobColor[cpt].g == 0.827451f && blobSprite[cpt].name == "SquidH")
            {
                if (SteamManager.Initialized)
                {
                    SteamUserStats.SetAchievement("PINEAPPLE");
                    SteamUserStats.StoreStats();
                }
            }
        }

        if(maxPlayers == 4 && blobSprite[0].name == "SquidE" && blobSprite[1].name == "SquidE" && blobSprite[2].name == "SquidE" && blobSprite[3].name == "SquidE")
        {
            if (SteamManager.Initialized)
            {
                SteamUserStats.SetAchievement("PACMAN");
                SteamUserStats.StoreStats();
            }
        }

        GameObject.Find("Music(Clone)").GetComponent<MusicMixer>().GameMusic();
        startAnimation.SetActive(true);
        FindChild(level, "Pole").SetActive(true);
        selectionMenu.SetActive(false);

        yield return new WaitForSeconds(1.8f);

        if(maxPlayers == 1)
        {
            blob2 = GameObject.Instantiate(AIBlob);
            blob2.name = "Blob 2(Clone)";
            blob2.GetComponent<AIController>().gameLogics = gameObject;
            UIController uiController = playerSelectionPrefab.GetComponent<UIController>();
            blobSprite[1] = uiController.shapes[UnityEngine.Random.Range(0, uiController.shapes.Length)];
            blobColor[1] = uiController.colorPool[UnityEngine.Random.Range(0, uiController.colorPool.Length)];
        }

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
        if(maxPlayers == 1)
        {
            blob2.GetComponent<AIController>().enabled = false;
        }
        if(maxPlayers == 2)
        {
            blob2.GetComponent<PlayerController>().enabled = false;
        }
        if (maxPlayers == 4)
        {
            InitBlob(blob3, 2);
            InitBlob(blob4, 3);
            blob3.GetComponent<PlayerController>().enabled = false;
            blob4.GetComponent<PlayerController>().enabled = false;
        }
        if (maxPlayers == 2 || maxPlayers == 1)
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
            blob1.transform.Rotate(Vector3.Lerp(new Vector3(0,0,0), new Vector3(0,0,-20), t / 0.5f));
            blob2.transform.Rotate(Vector3.Lerp(new Vector3(0,0,0), new Vector3(0,0,-20), t / 0.5f));
            if (maxPlayers == 4)
            {
                blob3.transform.Rotate(Vector3.Lerp(new Vector3(0, 0, 0), new Vector3(0, 0, -20), t / 0.5f));
                blob4.transform.Rotate(Vector3.Lerp(new Vector3(0, 0, 0), new Vector3(0, 0, -20), t / 0.5f));
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
        if (maxPlayers == 1)
        {
            blob2.GetComponent<AIController>().enabled = true;
        }
        if (maxPlayers == 2)
        {
            blob2.GetComponent<PlayerController>().enabled = true;
        }
        if (maxPlayers == 4)
        {
            blob3.GetComponent<PlayerController>().enabled = true;
            blob4.GetComponent<PlayerController>().enabled = true;
        }
        startAnimation.SetActive(false);

        ballAnimation.SetActive(true);
        yield return new WaitForSeconds(0.9f);
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
        readyMessage.SetActive(true);
        yield return new WaitForSeconds(0.9f);
        setMessage.SetActive(true);
        yield return new WaitForSeconds(0.9f);
        goMessage.SetActive(true);
    }

    IEnumerator MatchPoint(string player)
    {
        float xMatch = -11.76f, xPoint = -8.8f;
        if (player == "Blob 2" || player == "Blob 2")
        {
            xMatch = 8.8f;
            xPoint = 11.76f;
        }
        matchMessage.GetComponent<MessageAnimation>().startingPosition = new Vector3(xMatch, 20, 0);
        pointMessage.GetComponent<MessageAnimation>().startingPosition = new Vector3(xPoint, 20, 0);
        matchMessage.GetComponent<MessageAnimation>().middleFirstPosition = new Vector3(xMatch, 3, 0);
        pointMessage.GetComponent<MessageAnimation>().middleFirstPosition = new Vector3(xPoint, 3, 0);
        matchMessage.GetComponent<MessageAnimation>().middleLastPosition = new Vector3(xMatch, -3, 0);
        pointMessage.GetComponent<MessageAnimation>().middleLastPosition = new Vector3(xPoint, -3, 0);
        matchMessage.GetComponent<MessageAnimation>().endingPosition = new Vector3(xMatch, -20, 0);
        pointMessage.GetComponent<MessageAnimation>().endingPosition = new Vector3(xPoint, -20, 0);
        matchMessage.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        pointMessage.SetActive(true);
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
        Time.timeScale = 1.2f;
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
        if(blob2 != null && blob2.GetComponent<PlayerController>() != null)
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
        ball.GetComponent<BallLogics>().UpdateBall(2);
        roundTime = 0;
    }

    IEnumerator GameOver(string player)
    {
        int gamesPlayed = PlayerPrefs.GetInt("GamesPlayed", 0);
        gamesPlayed++;
        PlayerPrefs.SetInt("GamesPlayed", gamesPlayed);
        if (gamesPlayed == 100)
        {
            if (SteamManager.Initialized)
            {
                SteamUserStats.SetAchievement("PLAYER");
                SteamUserStats.StoreStats();
            }
        }
        GameObject.Find("Music(Clone)").GetComponent<MusicMixer>().StopMusic();
        Time.timeScale = 0.5f;
        yield return new WaitForSeconds(0.5f);
        Time.timeScale = 1.2f;
        ball.SetActive(false);
        FindChild(gameObject, "Ball Indicator").SetActive(false);
        gameOver.SetActive(true);
        SpriteRenderer blackSpriteRenderer = FindChild(FindChild(gameOver, "Blackout"), "Black").GetComponent<SpriteRenderer>();
        float t = 0.0f;
        while (t < 2f)
        {
            blackSpriteRenderer.color = new Color(blackSpriteRenderer.color.r, blackSpriteRenderer.color.g, blackSpriteRenderer.color.b, Mathf.Lerp(0, 0.85f, t * 2));
            t += Time.deltaTime;
            yield return null;
        }
        if (player == "Blob 1" || player == "Blob 3")
        {
            FindChild(FindChild(gameOver, "Blackout"), "Spotlight").transform.position = new Vector3(-6f, -1f, 0);
        }
        else
        {
            FindChild(FindChild(gameOver, "Blackout"), "Spotlight").transform.position = new Vector3(6f, -1f, 0);
        }
        FindChild(FindChild(gameOver, "Blackout"), "Spotlight").SetActive(true);
        yield return new WaitForSeconds(2f);
        FindChild(FindChild(blob1, "SpriteBlob"), "EyesWhite").SetActive(false);
        FindChild(FindChild(blob2, "SpriteBlob"), "EyesWhite").SetActive(false);
        FindChild(gameOver, "Cup").SetActive(true);
        FindChild(gameOver, "Confetti").SetActive(true);
        if (maxPlayers == 4)
        {
            FindChild(FindChild(blob3, "SpriteBlob"), "EyesWhite").SetActive(false);
            FindChild(FindChild(blob4, "SpriteBlob"), "EyesWhite").SetActive(false);
        }
        if (player == "Blob 1" || player == "Blob 3")
        {
            FindChild(gameOver, "Cup").transform.position = new Vector3(-6, 5, 0);
            FindChild(gameOver, "Confetti").transform.position = new Vector3(-9, 12, -20);
            FindChild(FindChild(blob1, "SpriteBlob"), "HappyEyes").SetActive(true);
            FindChild(FindChild(blob2, "SpriteBlob"), "SadEyes").SetActive(true);
            if(maxPlayers== 4)
            {
                FindChild(FindChild(blob3, "SpriteBlob"), "HappyEyes").SetActive(true);
                FindChild(FindChild(blob4, "SpriteBlob"), "SadEyes").SetActive(true);
            }
        }
        else
        {
            FindChild(gameOver, "Cup").transform.position = new Vector3(6, 5, 0);
            FindChild(gameOver, "Confetti").transform.position = new Vector3(9, 12, -20);
            FindChild(FindChild(blob2, "SpriteBlob"), "HappyEyes").SetActive(true);
            FindChild(FindChild(blob1, "SpriteBlob"), "SadEyes").SetActive(true);
            if (maxPlayers == 4)
            {
                FindChild(FindChild(blob4, "SpriteBlob"), "HappyEyes").SetActive(true);
                FindChild(FindChild(blob3, "SpriteBlob"), "SadEyes").SetActive(true);
            }
        }
        isPlaying = true;
        GameObject.Find("Music(Clone)").GetComponent<MusicMixer>().gameOver = true;
        FindChild(gameOver, "PressToContinue").SetActive(true);
        FindChild(level, "PlayerBounds").SetActive(false);
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
            ball.GetComponent<BallLogics>().TooManyTouchesSound();
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
        if(blob.GetComponent<PlayerController>() == null)
        {
            FindChild(blob, "SpriteBlob").GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);

            foreach (Transform child in FindChild(FindChild(blob, "SpriteBlob"), "Tentacles").transform)
            {
                child.gameObject.GetComponent<Tentacle>().Color = new Color(1, 1, 1, 1);
            }
            FindChild(FindChild(blob, "Smash"), "SmashAnimation").GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 1);
            FindChild(FindChild(blob, "Bump"), "BumpAnimation").GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 1);
            FindChild(blob, "SmashFreezeFrame").GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 1);

            ParticleSystem.MainModule settings = FindChild(blob, "ParticleContact").GetComponent<ParticleSystem>().main;
            settings.startColor = new ParticleSystem.MinMaxGradient(new Color(0, 0, 0, 1));
            settings = FindChild(blob, "InkStains").GetComponent<ParticleSystem>().main;
            settings.startColor = new ParticleSystem.MinMaxGradient(new Color(0, 0, 0, 1));
        } else
        {
            FindChild(blob, "SpriteBlob").GetComponent<SpriteRenderer>().color = blobColor[id - 1];

            foreach (Transform child in FindChild(FindChild(blob, "SpriteBlob"), "Tentacles").transform)
            {
                child.gameObject.GetComponent<Tentacle>().Color = blobColor[id - 1];
            }
            FindChild(FindChild(blob, "Smash"), "SmashAnimation").GetComponent<SpriteRenderer>().color = blobColor[id - 1];
            FindChild(FindChild(blob, "Bump"), "BumpAnimation").GetComponent<SpriteRenderer>().color = blobColor[id - 1];
            FindChild(blob, "SmashFreezeFrame").GetComponent<SpriteRenderer>().color = blobColor[id - 1];

            ParticleSystem.MainModule settings = FindChild(blob, "ParticleContact").GetComponent<ParticleSystem>().main;
            settings.startColor = new ParticleSystem.MinMaxGradient(blobColor[id - 1]);
            settings = FindChild(blob, "InkStains").GetComponent<ParticleSystem>().main;
            settings.startColor = new ParticleSystem.MinMaxGradient(blobColor[id - 1]);
        }
    }

    void ApplyShape(int id, GameObject blob)
    {
        FindChild(blob, "SpriteBlob").GetComponent<SpriteRenderer>().sprite = blobSprite[id-1];
    }
}
