﻿using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.Controls;

public class GameLogics : MonoBehaviour
{
    public InputAction startAction;
    public GameObject uiMessage;
    public GameObject uiScore;
    public GameObject gameOver;
    public GameObject blob1Prefab;
    public GameObject blob2Prefab;
    public GameObject ball;
    public bool isStarting = false;
    public bool isPlaying = false;

    GameObject blob1;
    GameObject blob2;
    Vector3 blob1Position;
    Vector3 blob2Position;
    Vector3 blob1Scale;
    Vector3 blob2Scale;
    Vector3 ballPosition;
    int blob1Score = 0;
    int blob2Score = 0;
    int nbPlayer = 0;

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

    // Start is called before the first frame update
    void Start()
    {

        Time.timeScale = 0;
        ++InputUser.listenForUnpairedDeviceActivity;

        // Example of how to spawn a new player automatically when a button
        // is pressed on an unpaired device.
        InputUser.onUnpairedDeviceUsed +=
            (control, eventPtr) =>
            {
                // Ignore anything but button presses.
                if (!(control is ButtonControl))
                    return;

                // Spawn player and pair device. If the player's actions have control schemes
                // defined in them, PlayerInput will look for a compatible scheme automatically.
                if (nbPlayer == 0)
                {
                    blob1 = PlayerInput.Instantiate(blob1Prefab, pairWithDevice: control.device).gameObject;
                    blob1.GetComponent<PlayerController>().gameLogics = gameObject;
                    blob1.transform.GetChild(0).GetComponent<EyeLogics>().ball = ball;
                    blob1Position = blob1.transform.position;
                    blob1Scale = blob1.transform.localScale;
                }
                else if (nbPlayer == 1)
                {
                    blob2 = PlayerInput.Instantiate(blob2Prefab, pairWithDevice: control.device).gameObject;
                    blob2.GetComponent<PlayerController>().gameLogics = gameObject;
                    blob2.transform.GetChild(0).GetComponent<EyeLogics>().ball = ball;
                    blob2Position = blob2.transform.position;
                    blob2Scale = blob2.transform.localScale;
                }
                nbPlayer++;
            };
        ballPosition = ball.transform.position;
        StartCoroutine(PlayersReady());
    }

    IEnumerator PlayersReady()
    {
        while (nbPlayer < 2)
        {
            yield return null;
        }
        ball.SetActive(true);
        startAction.Enable();
        BeginGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (startAction.triggered && isStarting == true)
        {
            Time.timeScale = 1;
            isPlaying = true;
            uiMessage.SetActive(false);
        }
    }

    void BeginGame()
    {
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
