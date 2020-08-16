using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public GameObject[] arrows;
    public string[] scenes;
    int menuId = 0;
    PlayerInput playerInput;
    InputAction moveAction;
    InputAction confirmAction;
    bool inputOnCooldown = false;

    // Use this for initialization
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        confirmAction = playerInput.actions["Confirm"];

        confirmAction.started += ctx =>
        {
            if (inputOnCooldown)
            {
                return;
            }
            inputOnCooldown = true;
            StartCoroutine(EnableInput(0.2f));
            if(FindChild(gameObject, "StartScreen").activeSelf)
            {
                FindChild(gameObject, "StartScreen").SetActive(false);
                FindChild(gameObject, "MenuContent").SetActive(true);
                return;
            }
            if (menuId == 5)
            {
                Application.Quit();
            } else
            {
                SceneManager.LoadScene(scenes[menuId], LoadSceneMode.Single);
            }
        };
    }

    void Update()
    {
        if (inputOnCooldown)
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
        if (menuId >= 5)
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
