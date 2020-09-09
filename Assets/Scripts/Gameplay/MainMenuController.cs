using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public GameObject[] arrows;
    public GameObject soundManager;
    public GameObject credits;
    public string[] scenes;
    int menuId = 0;
    PlayerInput playerInput;
    InputAction moveAction;
    InputAction confirmAction;
    bool inputOnCooldown = false;
    bool splashScreen = true;
    bool isOnControl = false;
    bool isOnSound = false;

    // Use this for initialization
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        confirmAction = playerInput.actions["Confirm"];

        confirmAction.started += ctx =>
        {
            if (this == null || inputOnCooldown)
            {
                return;
            }
            inputOnCooldown = true;
            StartCoroutine(EnableInput(0.15f));
            if(FindChild(gameObject, "StartScreen").activeSelf)
            {
                FindChild(gameObject, "StartScreen").SetActive(false);
                FindChild(gameObject, "MenuContent").SetActive(true);
                splashScreen = false;
                return;
            }
            if(splashScreen)
            {
                return;
            }
            switch (menuId)
            {
                case 0:
                    SceneManager.LoadScene("Local4", LoadSceneMode.Single);
                    break;
                case 1:
                    SceneManager.LoadScene("Local2", LoadSceneMode.Single);
                    break;
                case 4:
                    Application.Quit();
                    break;
            }
        };
    }

    void Update()
    {
        if (inputOnCooldown || splashScreen)
        {
            return;
        }
        // Moving
        var moveDirectionVector = moveAction.ReadValue<Vector2>();
        if (moveDirectionVector.y > 0.9f)
        {
            MoveToPrevious();
            inputOnCooldown = true;
            StartCoroutine(EnableInput(0.15f));
        }
        if (moveDirectionVector.y < -0.9f)
        {
            MoveToNext();
            inputOnCooldown = true;
            StartCoroutine(EnableInput(0.15f));
        }
    }

    void DisplayRightPanel()
    {
        soundManager.SetActive(false);
        credits.SetActive(false);
        if (menuId == 3)
        {
            soundManager.SetActive(true);
        }
        if (menuId == 4)
        {
            credits.SetActive(true);
        }
    }

    void MoveToNext()
    {
        if (menuId >= 5)
        {
            return;
        }
        for (int i = 0; i < arrows.Length; i++)
        {
            arrows[i].SetActive(false);
        }
        menuId++;
        DisplayRightPanel();
        transform.GetChild(0).GetChild(menuId).GetChild(0).gameObject.SetActive(true);
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
        DisplayRightPanel();
        transform.GetChild(0).GetChild(menuId).GetChild(0).gameObject.SetActive(true);
    }

    IEnumerator EnableInput(float duration)
    {
        yield return new WaitForSeconds(duration);
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
}
