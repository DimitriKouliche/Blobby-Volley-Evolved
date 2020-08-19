using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public PlayerInput playerInput;
    public GameObject[] arrows;
    public string[] scenes;
    int menuId = 0;
    InputAction moveAction;
    bool inputOnCooldown = false;
    bool justWokeUp = true;


    // Use this for initialization
    void OnEnable()
    {
        justWokeUp = true;
        moveAction = playerInput.actions["Move"];

        playerInput.actions["Charge Jump"].started += ctx =>
        {
            if (playerInput == null)
            {
                return;
            }
            if (menuId == 3)
            {
                Application.Quit();
            }
            if (menuId == 1)
            {
                gameObject.SetActive(false);
                Time.timeScale = 1;
            }
        };

        playerInput.actions["Start"].started += ctx =>
        {
            if (playerInput == null)
            {
                return;
            }
            playerInput = null;
            if(justWokeUp)
            {
                justWokeUp = false;
                return;
            }
            StartCoroutine(EnableInput(0.2f));
            gameObject.SetActive(false);
            Time.timeScale = 1;
        };
    }

    void Update()
    {
        if (moveAction == null)
        {
            return;
        }
        // Moving
        var moveDirectionVector = moveAction.ReadValue<Vector2>();
        if (moveDirectionVector.y > 0.9f)
        {
            MoveToPrevious();
            inputOnCooldown = true;
            StartCoroutine(EnableInput(0.2f));
        }
        if (moveDirectionVector.y < -0.9f)
        {
            MoveToNext();
            inputOnCooldown = true;
            StartCoroutine(EnableInput(0.2f));
        }
    }

    void MoveToNext()
    {
        if (menuId >= 3)
        {
            return;
        }
        for (int i = 0; i < arrows.Length; i++)
        {
            arrows[i].SetActive(false);
        }
        menuId++;
        transform.GetChild(menuId).GetChild(0).gameObject.SetActive(true);
    }

    void MoveToPrevious()
    {
        if (menuId <= 0)
        {
            return;
        }
        for (int i = 0; i < arrows.Length; i++)
        {
            arrows[i].SetActive(false);
        }
        menuId--;
        transform.GetChild(menuId).GetChild(0).gameObject.SetActive(true);
    }

    IEnumerator EnableInput(float duration)
    {
        yield return StartCoroutine(WaitForRealSeconds(duration));
        inputOnCooldown = false;
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
    IEnumerator WaitForRealSeconds(float seconds)
    {
        float startTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - startTime < seconds)
        {
            yield return null;
        }
    }
}
